using System.Collections.Generic;

namespace Ink {
    public class CharacterSet : HashSet<char> {
        public CharacterSet() { }

        public CharacterSet(string str) {
            AddCharacters(str);
        }

        public CharacterSet(CharacterSet charSetToCopy) {
            AddCharacters(charSetToCopy);
        }

        public static CharacterSet FromRange(char start, char end) {
            return new CharacterSet().AddRange(start, end);
        }

        public CharacterSet AddRange(char start, char end) {
            for (var c = start; c <= end; ++c) Add(c);
            return this;
        }

        public CharacterSet AddCharacters(IEnumerable<char> chars) {
            foreach (var c in chars) Add(c);
            return this;
        }

        public CharacterSet AddCharacters(string chars) {
            foreach (var c in chars) Add(c);
            return this;
        }
    }
}