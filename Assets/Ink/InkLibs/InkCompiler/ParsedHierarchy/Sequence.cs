using System;
using System.Collections.Generic;
using Ink.Runtime;

namespace Ink.Parsed {
    [Flags]
    public enum SequenceType {
        Stopping = 1, // default
        Cycle = 2,
        Shuffle = 4,
        Once = 8
    }

    public class Sequence : Object {
        private List<SequenceDivertToResolve> _sequenceDivertsToResove;

        public List<Object> sequenceElements;
        public SequenceType sequenceType;

        public Sequence(List<ContentList> elementContentLists, SequenceType sequenceType) {
            this.sequenceType = sequenceType;
            sequenceElements = new List<Object>();

            foreach (var elementContentList in elementContentLists) {
                var contentObjs = elementContentList.content;

                Object seqElObject = null;

                // Don't attempt to create a weave for the sequence element 
                // if the content list is empty. Weaves don't like it!
                if (contentObjs == null || contentObjs.Count == 0)
                    seqElObject = elementContentList;
                else
                    seqElObject = new Weave(contentObjs);

                sequenceElements.Add(seqElObject);
                AddContent(seqElObject);
            }
        }

        // Generate runtime code that looks like:
        //
        //   chosenIndex = MIN(sequence counter, num elements) e.g. for "Stopping"
        //   if chosenIndex == 0, divert to s0
        //   if chosenIndex == 1, divert to s1  [etc]
        //
        //   - s0:
        //      <content for sequence element>
        //      divert to no-op
        //   - s1:
        //      <content for sequence element>
        //      divert to no-op
        //   - s2:
        //      empty branch if using "once"
        //      divert to no-op
        //
        //    no-op
        //
        public override Runtime.Object GenerateRuntimeObject() {
            var container = new Container();
            container.visitsShouldBeCounted = true;
            container.countingAtStartOnly = true;

            _sequenceDivertsToResove = new List<SequenceDivertToResolve>();

            // Get sequence read count
            container.AddContent(ControlCommand.EvalStart());
            container.AddContent(ControlCommand.VisitIndex());

            var once = (sequenceType & SequenceType.Once) > 0;
            var cycle = (sequenceType & SequenceType.Cycle) > 0;
            var stopping = (sequenceType & SequenceType.Stopping) > 0;
            var shuffle = (sequenceType & SequenceType.Shuffle) > 0;

            var seqBranchCount = sequenceElements.Count;
            if (once) seqBranchCount++;

            // Chosen sequence index:
            //  - Stopping: take the MIN(read count, num elements - 1)
            //  - Once: take the MIN(read count, num elements)
            //    (the last one being empty)
            if (stopping || once) {
                //var limit = stopping ? seqBranchCount-1 : seqBranchCount;
                container.AddContent(new IntValue(seqBranchCount - 1));
                container.AddContent(NativeFunctionCall.CallWithName("MIN"));
            }

            // - Cycle: take (read count % num elements)
            else if (cycle) {
                container.AddContent(new IntValue(sequenceElements.Count));
                container.AddContent(NativeFunctionCall.CallWithName("%"));
            }

            // Shuffle
            if (shuffle) {
                // Create point to return to when sequence is complete
                var postShuffleNoOp = ControlCommand.NoOp();

                // When visitIndex == lastIdx, we skip the shuffle
                if (once || stopping) {
                    // if( visitIndex == lastIdx ) -> skipShuffle
                    var lastIdx = stopping ? sequenceElements.Count - 1 : sequenceElements.Count;
                    container.AddContent(ControlCommand.Duplicate());
                    container.AddContent(new IntValue(lastIdx));
                    container.AddContent(NativeFunctionCall.CallWithName("=="));

                    var skipShuffleDivert = new Runtime.Divert();
                    skipShuffleDivert.isConditional = true;
                    container.AddContent(skipShuffleDivert);

                    AddDivertToResolve(skipShuffleDivert, postShuffleNoOp);
                }

                // This one's a bit more complex! Choose the index at runtime.
                var elementCountToShuffle = sequenceElements.Count;
                if (stopping) elementCountToShuffle--;
                container.AddContent(new IntValue(elementCountToShuffle));
                container.AddContent(ControlCommand.SequenceShuffleIndex());
                if (once || stopping) container.AddContent(postShuffleNoOp);
            }

            container.AddContent(ControlCommand.EvalEnd());

            // Create point to return to when sequence is complete
            var postSequenceNoOp = ControlCommand.NoOp();

            // Each of the main sequence branches, and one extra empty branch if 
            // we have a "once" sequence.
            for (var elIndex = 0; elIndex < seqBranchCount; elIndex++) {
                // This sequence element:
                //  if( chosenIndex == this index ) divert to this sequence element
                // duplicate chosen sequence index, since it'll be consumed by "=="
                container.AddContent(ControlCommand.EvalStart());
                container.AddContent(ControlCommand.Duplicate());
                container.AddContent(new IntValue(elIndex));
                container.AddContent(NativeFunctionCall.CallWithName("=="));
                container.AddContent(ControlCommand.EvalEnd());

                // Divert branch for this sequence element
                var sequenceDivert = new Runtime.Divert();
                sequenceDivert.isConditional = true;
                container.AddContent(sequenceDivert);

                Container contentContainerForSequenceBranch;

                // Generate content for this sequence element
                if (elIndex < sequenceElements.Count) {
                    var el = sequenceElements[elIndex];
                    contentContainerForSequenceBranch = (Container)el.runtimeObject;
                }

                // Final empty branch for "once" sequences
                else {
                    contentContainerForSequenceBranch = new Container();
                }

                contentContainerForSequenceBranch.name = "s" + elIndex;
                contentContainerForSequenceBranch.InsertContent(ControlCommand.PopEvaluatedValue(), 0);

                // When sequence element is complete, divert back to end of sequence
                var seqBranchCompleteDivert = new Runtime.Divert();
                contentContainerForSequenceBranch.AddContent(seqBranchCompleteDivert);
                container.AddToNamedContentOnly(contentContainerForSequenceBranch);

                // Save the diverts for reference resolution later (in ResolveReferences)
                AddDivertToResolve(sequenceDivert, contentContainerForSequenceBranch);
                AddDivertToResolve(seqBranchCompleteDivert, postSequenceNoOp);
            }

            container.AddContent(postSequenceNoOp);

            return container;
        }

        private void AddDivertToResolve(Runtime.Divert divert, Runtime.Object targetContent) {
            _sequenceDivertsToResove.Add(new SequenceDivertToResolve {
                divert = divert,
                targetContent = targetContent
            });
        }

        public override void ResolveReferences(Story context) {
            base.ResolveReferences(context);

            foreach (var toResolve in _sequenceDivertsToResove)
                toResolve.divert.targetPath = toResolve.targetContent.path;
        }

        private class SequenceDivertToResolve {
            public Runtime.Divert divert;
            public Runtime.Object targetContent;
        }
    }
}