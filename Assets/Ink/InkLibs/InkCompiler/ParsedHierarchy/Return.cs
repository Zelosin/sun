using Ink.Runtime;

namespace Ink.Parsed {
    public class Return : Object {
        public Return(Expression returnedExpression = null) {
            if (returnedExpression) this.returnedExpression = AddContent(returnedExpression);
        }

        public Expression returnedExpression { get; protected set; }

        public override Runtime.Object GenerateRuntimeObject() {
            var container = new Container();

            // Evaluate expression
            if (returnedExpression) {
                container.AddContent(returnedExpression.runtimeObject);
            }

            // Return Runtime.Void when there's no expression to evaluate
            // (This evaluation will just add the Void object to the evaluation stack)
            else {
                container.AddContent(ControlCommand.EvalStart());
                container.AddContent(new Void());
                container.AddContent(ControlCommand.EvalEnd());
            }

            // Then pop the call stack
            // (the evaluated expression will leave the return value on the evaluation stack)
            container.AddContent(ControlCommand.PopFunction());

            return container;
        }
    }
}