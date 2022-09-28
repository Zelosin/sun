using Ink.Runtime;

namespace Ink.Parsed {
    public class Text : Object {
        public Text(string str) {
            text = str;
        }

        public string text { get; set; }

        public override Runtime.Object GenerateRuntimeObject() {
            return new StringValue(text);
        }

        public override string ToString() {
            return text;
        }
    }
}