namespace Ink.Runtime {
    public class Tag : Object {
        public Tag(string tagText) {
            text = tagText;
        }

        public string text { get; }

        public override string ToString() {
            return "# " + text;
        }
    }
}