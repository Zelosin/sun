using System.Collections.Generic;
using System.Linq;
using Ink.Runtime;

namespace Ink.Parsed {
    public class Conditional : Object {
        private ControlCommand _reJoinTarget;

        public Conditional(Expression condition, List<ConditionalSingleBranch> branches) {
            initialCondition = condition;
            if (initialCondition) AddContent(condition);

            this.branches = branches;
            if (this.branches != null) AddContent(this.branches.Cast<Object>().ToList());
        }

        public Expression initialCondition { get; }
        public List<ConditionalSingleBranch> branches { get; }

        public override Runtime.Object GenerateRuntimeObject() {
            var container = new Container();

            // Initial condition
            if (initialCondition) container.AddContent(initialCondition.runtimeObject);

            // Individual branches
            foreach (var branch in branches) {
                var branchContainer = (Container)branch.runtimeObject;
                container.AddContent(branchContainer);
            }

            // If it's a switch-like conditional, each branch
            // will have a "duplicate" operation for the original
            // switched value. If there's no final else clause
            // and we fall all the way through, we need to clean up.
            // (An else clause doesn't dup but it *does* pop)
            if (initialCondition != null && branches[0].ownExpression != null && !branches[branches.Count - 1].isElse)
                container.AddContent(ControlCommand.PopEvaluatedValue());

            // Target for branches to rejoin to
            _reJoinTarget = ControlCommand.NoOp();
            container.AddContent(_reJoinTarget);

            return container;
        }

        public override void ResolveReferences(Story context) {
            var pathToReJoin = _reJoinTarget.path;

            foreach (var branch in branches) branch.returnDivert.targetPath = pathToReJoin;

            base.ResolveReferences(context);
        }
    }
}