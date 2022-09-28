namespace Ink.Parsed {
    public class IncludedFile : Object {
        public IncludedFile(Story includedStory) {
            this.includedStory = includedStory;
        }

        public Story includedStory { get; }

        public override Runtime.Object GenerateRuntimeObject() {
            // Left to the main story to process
            return null;
        }
    }
}