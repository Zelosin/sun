using System.Collections.Generic;

namespace Ink.Parsed {
    public class Stitch : FlowBase {
        public Stitch(Identifier name, List<Object> topLevelObjects, List<Argument> arguments, bool isFunction) : base(
            name, topLevelObjects, arguments, isFunction) { }

        public override FlowLevel flowLevel => FlowLevel.Stitch;
    }
}