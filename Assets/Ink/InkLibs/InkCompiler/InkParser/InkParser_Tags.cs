using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ink.Parsed;

namespace Ink {
    public partial class InkParser {
        private readonly CharacterSet _endOfTagCharSet = new("#\n\r\\");

        protected Tag Tag() {
            Whitespace();

            if (ParseString("#") == null)
                return null;

            Whitespace();

            var sb = new StringBuilder();
            do {
                // Read up to another #, end of input or newline
                var tagText = ParseUntilCharactersFromCharSet(_endOfTagCharSet);
                sb.Append(tagText);

                // Escape character
                if (ParseString("\\") != null) {
                    var c = ParseSingleCharacter();
                    if (c != (char)0) sb.Append(c);
                    continue;
                }

                break;
            } while (true);

            var fullTagText = sb.ToString().Trim();

            return new Tag(new Runtime.Tag(fullTagText));
        }

        protected List<Tag> Tags() {
            var tags = OneOrMore(Tag);
            if (tags == null) return null;

            return tags.Cast<Tag>().ToList();
        }
    }
}