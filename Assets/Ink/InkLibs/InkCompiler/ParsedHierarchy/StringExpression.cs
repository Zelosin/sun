using System.Collections.Generic;
using System.Text;
using Ink.Runtime;

namespace Ink.Parsed {
    public class StringExpression : Expression {
        public StringExpression(List<Object> content) {
            AddContent(content);
        }

        public bool isSingleString {
            get {
                if (content.Count != 1)
                    return false;

                var c = content[0];
                if (!(c is Text))
                    return false;

                return true;
            }
        }

        public override void GenerateIntoContainer(Container container) {
            container.AddContent(ControlCommand.BeginString());

            foreach (var c in content) container.AddContent(c.runtimeObject);

            container.AddContent(ControlCommand.EndString());
        }

        public override string ToString() {
            var sb = new StringBuilder();
            foreach (var c in content) sb.Append(c.ToString());
            return sb.ToString();
        }

        // Equals override necessary in order to check for CONST multiple definition equality
        public override bool Equals(object obj) {
            var otherStr = obj as StringExpression;
            if (otherStr == null) return false;

            // Can only compare direct equality on single strings rather than
            // complex string expressions that contain dynamic logic
            if (!isSingleString || !otherStr.isSingleString) return false;

            var thisTxt = ToString();
            var otherTxt = otherStr.ToString();
            return thisTxt.Equals(otherTxt);
        }

        public override int GetHashCode() {
            return ToString().GetHashCode();
        }
    }
}