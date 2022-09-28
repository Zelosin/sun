using System.Text;

namespace Ink.Runtime {
    public class Divert : Object {
        private Path _targetPath;
        private Pointer _targetPointer;
        public PushPopType stackPushType;

        public Divert() {
            pushesToStack = false;
        }

        public Divert(PushPopType stackPushType) {
            pushesToStack = true;
            this.stackPushType = stackPushType;
        }

        public Path targetPath {
            get {
                // Resolve any relative paths to global ones as we come across them
                if (_targetPath != null && _targetPath.isRelative) {
                    var targetObj = targetPointer.Resolve();
                    if (targetObj) _targetPath = targetObj.path;
                }

                return _targetPath;
            }
            set {
                _targetPath = value;
                _targetPointer = Pointer.Null;
            }
        }

        public Pointer targetPointer {
            get {
                if (_targetPointer.isNull) {
                    var targetObj = ResolvePath(_targetPath).obj;

                    if (_targetPath.lastComponent.isIndex) {
                        _targetPointer.container = targetObj.parent as Container;
                        _targetPointer.index = _targetPath.lastComponent.index;
                    }
                    else {
                        _targetPointer = Pointer.StartOf(targetObj as Container);
                    }
                }

                return _targetPointer;
            }
        }


        public string targetPathString {
            get {
                if (targetPath == null)
                    return null;

                return CompactPathString(targetPath);
            }
            set {
                if (value == null)
                    targetPath = null;
                else
                    targetPath = new Path(value);
            }
        }

        public string variableDivertName { get; set; }
        public bool hasVariableTarget => variableDivertName != null;

        public bool pushesToStack { get; set; }

        public bool isExternal { get; set; }
        public int externalArgs { get; set; }

        public bool isConditional { get; set; }

        public override bool Equals(object obj) {
            var otherDivert = obj as Divert;
            if (otherDivert)
                if (hasVariableTarget == otherDivert.hasVariableTarget) {
                    if (hasVariableTarget)
                        return variableDivertName == otherDivert.variableDivertName;
                    return targetPath.Equals(otherDivert.targetPath);
                }

            return false;
        }

        public override int GetHashCode() {
            if (hasVariableTarget) {
                const int variableTargetSalt = 12345;
                return variableDivertName.GetHashCode() + variableTargetSalt;
            }

            const int pathTargetSalt = 54321;
            return targetPath.GetHashCode() + pathTargetSalt;
        }

        public override string ToString() {
            if (hasVariableTarget) return "Divert(variable: " + variableDivertName + ")";

            if (targetPath == null) {
                return "Divert(null)";
            }

            var sb = new StringBuilder();

            var targetStr = targetPath.ToString();
            var targetLineNum = DebugLineNumberOfPath(targetPath);
            if (targetLineNum != null) targetStr = "line " + targetLineNum;

            sb.Append("Divert");

            if (isConditional)
                sb.Append("?");

            if (pushesToStack) {
                if (stackPushType == PushPopType.Function)
                    sb.Append(" function");
                else
                    sb.Append(" tunnel");
            }

            sb.Append(" -> ");
            sb.Append(targetPathString);

            sb.Append(" (");
            sb.Append(targetStr);
            sb.Append(")");

            return sb.ToString();
        }
    }
}