using System.Collections.Generic;

namespace Ink.Parsed {
    public class Knot : FlowBase {
        public Knot(Identifier name, List<Object> topLevelObjects, List<Argument> arguments, bool isFunction) : base(
            name, topLevelObjects, arguments, isFunction) { }

        public override FlowLevel flowLevel => FlowLevel.Knot;

        public override void ResolveReferences(Story context) {
            base.ResolveReferences(context);

            var parentStory = story;

            // Enforce rule that stitches must not have the same
            // name as any knots that exist in the story
            foreach (var stitchNamePair in subFlowsByName) {
                var stitchName = stitchNamePair.Key;

                var knotWithStitchName = parentStory.ContentWithNameAtLevel(stitchName, FlowLevel.Knot);
                if (knotWithStitchName) {
                    var stitch = stitchNamePair.Value;
                    var errorMsg = string.Format("Stitch '{0}' has the same name as a knot (on {1})", stitch.identifier,
                        knotWithStitchName.debugMetadata);
                    Error(errorMsg, stitch);
                }
            }
        }
    }
}