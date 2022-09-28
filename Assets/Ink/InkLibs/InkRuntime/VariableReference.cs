namespace Ink.Runtime {
    public class VariableReference : Object {
        public VariableReference(string name) {
            this.name = name;
        }

        // Require default constructor for serialisation
        public VariableReference() { }

        // Normal named variable
        public string name { get; set; }

        // Variable reference is actually a path for a visit (read) count
        public Path pathForCount { get; set; }

        public Container containerForCount => ResolvePath(pathForCount).container;

        public string pathStringForCount {
            get {
                if (pathForCount == null)
                    return null;

                return CompactPathString(pathForCount);
            }
            set {
                if (value == null)
                    pathForCount = null;
                else
                    pathForCount = new Path(value);
            }
        }

        public override string ToString() {
            if (name != null) {
                return string.Format("var({0})", name);
            }

            var pathStr = pathStringForCount;
            return string.Format("read_count({0})", pathStr);
        }
    }
}