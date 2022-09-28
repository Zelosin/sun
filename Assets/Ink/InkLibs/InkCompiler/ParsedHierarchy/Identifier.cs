using Ink.Runtime;

namespace Ink.Parsed {
    public class Identifier {
        public static Identifier Done = new() { name = "DONE", debugMetadata = null };
        public DebugMetadata debugMetadata;
        public string name;

        public override string ToString() {
            return name;
        }
    }
}