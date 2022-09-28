﻿namespace Ink {
    public partial class InkParser {
        private readonly CharacterSet _inlineWhitespaceChars = new(" \t");

        // Handles both newline and endOfFile
        protected object EndOfLine() {
            return OneOf(Newline, EndOfFile);
        }

        // Allow whitespace before the actual newline
        protected object Newline() {
            Whitespace();

            var gotNewline = ParseNewline() != null;

            // Optional \r, definite \n to support Windows (\r\n) and Mac/Unix (\n)

            if (!gotNewline)
                return null;
            return ParseSuccess;
        }

        protected object EndOfFile() {
            Whitespace();

            if (!endOfInput)
                return null;

            return ParseSuccess;
        }


        // General purpose space, returns N-count newlines (fails if no newlines)
        protected object MultilineWhitespace() {
            var newlines = OneOrMore(Newline);
            if (newlines == null)
                return null;

            // Use content field of Token to say how many newlines there were
            // (in most circumstances it's unimportant)
            var numNewlines = newlines.Count;
            if (numNewlines >= 1)
                return ParseSuccess;
            return null;
        }

        protected object Whitespace() {
            if (ParseCharactersFromCharSet(_inlineWhitespaceChars) != null) return ParseSuccess;

            return null;
        }

        protected ParseRule Spaced(ParseRule rule) {
            return () => {
                Whitespace();

                var result = ParseObject(rule);
                if (result == null) return null;

                Whitespace();

                return result;
            };
        }

        protected object AnyWhitespace() {
            var anyWhitespace = false;
            while (OneOf(Whitespace, MultilineWhitespace) != null) anyWhitespace = true;
            return anyWhitespace ? ParseSuccess : null;
        }

        protected ParseRule MultiSpaced(ParseRule rule) {
            return () => {
                AnyWhitespace();

                var result = ParseObject(rule);
                if (result == null) return null;

                AnyWhitespace();

                return result;
            };
        }
    }
}