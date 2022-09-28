namespace Ink.Runtime {
    /// <summary>
    ///     The ChoicePoint represents the point within the Story where
    ///     a Choice instance gets generated. The distinction is made
    ///     because the text of the Choice can be dynamically generated.
    /// </summary>
    public class ChoicePoint : Object {
        private Path _pathOnChoice;

        public ChoicePoint(bool onceOnly) {
            this.onceOnly = onceOnly;
        }

        public ChoicePoint() : this(true) { }

        public Path pathOnChoice {
            get {
                // Resolve any relative paths to global ones as we come across them
                if (_pathOnChoice != null && _pathOnChoice.isRelative) {
                    var choiceTargetObj = choiceTarget;
                    if (choiceTargetObj) _pathOnChoice = choiceTargetObj.path;
                }

                return _pathOnChoice;
            }
            set => _pathOnChoice = value;
        }

        public Container choiceTarget => ResolvePath(_pathOnChoice).container;

        public string pathStringOnChoice {
            get => CompactPathString(pathOnChoice);
            set => pathOnChoice = new Path(value);
        }

        public bool hasCondition { get; set; }
        public bool hasStartContent { get; set; }
        public bool hasChoiceOnlyContent { get; set; }
        public bool onceOnly { get; set; }
        public bool isInvisibleDefault { get; set; }

        public int flags {
            get {
                var flags = 0;
                if (hasCondition) flags |= 1;
                if (hasStartContent) flags |= 2;
                if (hasChoiceOnlyContent) flags |= 4;
                if (isInvisibleDefault) flags |= 8;
                if (onceOnly) flags |= 16;
                return flags;
            }
            set {
                hasCondition = (value & 1) > 0;
                hasStartContent = (value & 2) > 0;
                hasChoiceOnlyContent = (value & 4) > 0;
                isInvisibleDefault = (value & 8) > 0;
                onceOnly = (value & 16) > 0;
            }
        }

        public override string ToString() {
            var targetLineNum = DebugLineNumberOfPath(pathOnChoice);
            var targetString = pathOnChoice.ToString();

            if (targetLineNum != null) targetString = " line " + targetLineNum + "(" + targetString + ")";

            return "Choice: -> " + targetString;
        }
    }
}