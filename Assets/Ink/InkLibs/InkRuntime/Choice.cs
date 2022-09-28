namespace Ink.Runtime {
    /// <summary>
    ///     A generated Choice from the story.
    ///     A single ChoicePoint in the Story could potentially generate
    ///     different Choices dynamically dependent on state, so they're
    ///     separated.
    /// </summary>
    public class Choice : Object {
        public bool isInvisibleDefault;
        public int originalThreadIndex;

        /// <summary>
        ///     Get the path to the original choice point - where was this choice defined in the story?
        /// </summary>
        /// <value>A dot separated path into the story data.</value>
        public string sourcePath;

        public Path targetPath;

        /// <summary>
        ///     The main text to presented to the player for this Choice.
        /// </summary>
        public string text { get; set; }

        /// <summary>
        ///     The target path that the Story should be diverted to if
        ///     this Choice is chosen.
        /// </summary>
        public string pathStringOnChoice {
            get => targetPath.ToString();
            set => targetPath = new Path(value);
        }

        /// <summary>
        ///     The original index into currentChoices list on the Story when
        ///     this Choice was generated, for convenience.
        /// </summary>
        public int index { get; set; }

        public CallStack.Thread threadAtGeneration { get; set; }
    }
}