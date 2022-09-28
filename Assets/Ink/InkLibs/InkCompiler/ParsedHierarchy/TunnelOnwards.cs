using Ink.Runtime;

namespace Ink.Parsed {
    public class TunnelOnwards : Object {
        private Divert _divertAfter;

        private DivertTargetValue _overrideDivertTarget;

        public Divert divertAfter {
            get => _divertAfter;
            set {
                _divertAfter = value;
                if (_divertAfter) AddContent(_divertAfter);
            }
        }

        public override Runtime.Object GenerateRuntimeObject() {
            var container = new Container();

            // Set override path for tunnel onwards (or nothing)
            container.AddContent(ControlCommand.EvalStart());

            if (divertAfter) {
                // Generate runtime object's generated code and steal the arguments runtime code
                var returnRuntimeObj = divertAfter.GenerateRuntimeObject();
                var returnRuntimeContainer = returnRuntimeObj as Container;
                if (returnRuntimeContainer) {
                    // Steal all code for generating arguments from the divert
                    var args = divertAfter.arguments;
                    if (args != null && args.Count > 0) {
                        // Steal everything betwen eval start and eval end
                        var evalStart = -1;
                        var evalEnd = -1;
                        for (var i = 0; i < returnRuntimeContainer.content.Count; i++) {
                            var cmd = returnRuntimeContainer.content[i] as ControlCommand;
                            if (cmd) {
                                if (evalStart == -1 && cmd.commandType == ControlCommand.CommandType.EvalStart)
                                    evalStart = i;
                                else if (cmd.commandType == ControlCommand.CommandType.EvalEnd)
                                    evalEnd = i;
                            }
                        }

                        for (var i = evalStart + 1; i < evalEnd; i++) {
                            var obj = returnRuntimeContainer.content[i];
                            obj.parent = null; // prevent error of being moved between owners
                            container.AddContent(returnRuntimeContainer.content[i]);
                        }
                    }
                }

                // Finally, divert to the requested target 
                _overrideDivertTarget = new DivertTargetValue();
                container.AddContent(_overrideDivertTarget);
            }

            // No divert after tunnel onwards
            else {
                container.AddContent(new Void());
            }

            container.AddContent(ControlCommand.EvalEnd());

            container.AddContent(ControlCommand.PopTunnel());

            return container;
        }

        public override void ResolveReferences(Story context) {
            base.ResolveReferences(context);

            if (divertAfter && divertAfter.targetContent)
                _overrideDivertTarget.targetPath = divertAfter.targetContent.runtimePath;
        }
    }
}