using System.Collections.Generic;

namespace Ink {
    /// <summary>
    ///     A class representing a character range. Allows for lazy-loading a corresponding
    ///     <see cref="CharacterSet">character set</see>.
    /// </summary>
    public sealed class CharacterRange {
        private readonly CharacterSet _correspondingCharSet = new();
        private readonly ICollection<char> _excludes;

        private CharacterRange(char start, char end, IEnumerable<char> excludes) {
            this.start = start;
            this.end = end;
            _excludes = excludes == null ? new HashSet<char>() : new HashSet<char>(excludes);
        }

        public char start { get; }

        public char end { get; }

        public static CharacterRange Define(char start, char end, IEnumerable<char> excludes = null) {
            return new CharacterRange(start, end, excludes);
        }

        /// <summary>
        ///     Returns a <see cref="CharacterSet">character set</see> instance corresponding to the character range
        ///     represented by the current instance.
        /// </summary>
        /// <remarks>
        ///     The internal character set is created once and cached in memory.
        /// </remarks>
        /// <returns>The char set.</returns>
        public CharacterSet ToCharacterSet() {
            if (_correspondingCharSet.Count == 0)
                for (var c = start; c <= end; c++)
                    if (!_excludes.Contains(c))
                        _correspondingCharSet.Add(c);
            return _correspondingCharSet;
        }
    }
}