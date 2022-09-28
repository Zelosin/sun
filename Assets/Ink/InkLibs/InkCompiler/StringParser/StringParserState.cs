using System;

namespace Ink {
    public class StringParserState {
        private readonly Element[] _stack;

        public StringParserState() {
            const int kExpectedMaxStackDepth = 200;
            _stack = new Element[kExpectedMaxStackDepth];

            for (var i = 0; i < kExpectedMaxStackDepth; ++i) _stack[i] = new Element();

            stackHeight = 1;
        }

        public int lineIndex {
            get => currentElement.lineIndex;
            set => currentElement.lineIndex = value;
        }

        public int characterIndex {
            get => currentElement.characterIndex;
            set => currentElement.characterIndex = value;
        }

        public int characterInLineIndex {
            get => currentElement.characterInLineIndex;
            set => currentElement.characterInLineIndex = value;
        }

        public uint customFlags {
            get => currentElement.customFlags;
            set => currentElement.customFlags = value;
        }

        public bool errorReportedAlreadyInScope => currentElement.reportedErrorInScope;

        public int stackHeight { get; private set; }

        protected Element currentElement => _stack[stackHeight - 1];

        public int Push() {
            if (stackHeight >= _stack.Length)
                throw new Exception("Stack overflow in parser state");

            var prevElement = _stack[stackHeight - 1];
            var newElement = _stack[stackHeight];
            stackHeight++;

            newElement.CopyFrom(prevElement);

            return newElement.uniqueId;
        }

        public void Pop(int expectedRuleId) {
            if (stackHeight == 1)
                throw new Exception(
                    "Attempting to remove final stack element is illegal! Mismatched Begin/Succceed/Fail?");

            if (currentElement.uniqueId != expectedRuleId)
                throw new Exception("Mismatched rule IDs - do you have mismatched Begin/Succeed/Fail?");

            // Restore state
            stackHeight--;
        }

        public Element Peek(int expectedRuleId) {
            if (currentElement.uniqueId != expectedRuleId)
                throw new Exception("Mismatched rule IDs - do you have mismatched Begin/Succeed/Fail?");

            return _stack[stackHeight - 1];
        }

        public Element PeekPenultimate() {
            if (stackHeight >= 2)
                return _stack[stackHeight - 2];
            return null;
        }

        // Reduce stack height while maintaining currentElement
        // Remove second last element: i.e. "squash last two elements together"
        // Used when succeeding from a rule (and ONLY when succeeding, since
        // the state of the top element is retained).
        public void Squash() {
            if (stackHeight < 2)
                throw new Exception(
                    "Attempting to remove final stack element is illegal! Mismatched Begin/Succceed/Fail?");

            var penultimateEl = _stack[stackHeight - 2];
            var lastEl = _stack[stackHeight - 1];

            penultimateEl.SquashFrom(lastEl);

            stackHeight--;
        }

        public void NoteErrorReported() {
            foreach (var el in _stack) el.reportedErrorInScope = true;
        }

        public class Element {
            private static int _uniqueIdCounter;
            public int characterIndex;
            public int characterInLineIndex;
            public uint customFlags;
            public int lineIndex;
            public bool reportedErrorInScope;
            public int uniqueId;

            public void CopyFrom(Element fromElement) {
                _uniqueIdCounter++;
                uniqueId = _uniqueIdCounter;
                characterIndex = fromElement.characterIndex;
                characterInLineIndex = fromElement.characterInLineIndex;
                lineIndex = fromElement.lineIndex;
                customFlags = fromElement.customFlags;
                reportedErrorInScope = false;
            }

            // Squash is used when succeeding from a rule,
            // so only the state information we wanted to carry forward is
            // retained. e.g. characterIndex and lineIndex are global,
            // however uniqueId is specific to the individual rule,
            // and likewise, custom flags are designed for the temporary
            // state of the individual rule too.
            public void SquashFrom(Element fromElement) {
                characterIndex = fromElement.characterIndex;
                characterInLineIndex = fromElement.characterInLineIndex;
                lineIndex = fromElement.lineIndex;
                reportedErrorInScope = fromElement.reportedErrorInScope;
            }
        }
    }
}