using Ink.Runtime;

namespace Ink.Parsed {
    public class VariableAssignment : Object {
        private Runtime.VariableAssignment _runtimeAssignment;

        public VariableAssignment(Identifier identifier, Expression assignedExpression) {
            variableIdentifier = identifier;

            // Defensive programming in case parsing of assignedExpression failed
            if (assignedExpression)
                expression = AddContent(assignedExpression);
        }

        public VariableAssignment(Identifier identifier, ListDefinition listDef) {
            variableIdentifier = identifier;

            if (listDef) {
                listDefinition = AddContent(listDef);
                listDefinition.variableAssignment = this;
            }

            // List definitions are always global
            isGlobalDeclaration = true;
        }

        public string variableName => variableIdentifier.name;

        public Identifier variableIdentifier { get; protected set; }
        public Expression expression { get; protected set; }
        public ListDefinition listDefinition { get; protected set; }

        public bool isGlobalDeclaration { get; set; }
        public bool isNewTemporaryDeclaration { get; set; }

        public bool isDeclaration => isGlobalDeclaration || isNewTemporaryDeclaration;


        public override string typeName {
            get {
                if (isNewTemporaryDeclaration) return "temp";
                if (isGlobalDeclaration) return "VAR";
                return "variable assignment";
            }
        }

        public override Runtime.Object GenerateRuntimeObject() {
            FlowBase newDeclScope = null;
            if (isGlobalDeclaration)
                newDeclScope = story;
            else if (isNewTemporaryDeclaration) newDeclScope = ClosestFlowBase();

            if (newDeclScope)
                newDeclScope.TryAddNewVariableDeclaration(this);

            // Global declarations don't generate actual procedural
            // runtime objects, but instead add a global variable to the story itself.
            // The story then initialises them all in one go at the start of the game.
            if (isGlobalDeclaration)
                return null;

            var container = new Container();

            // The expression's runtimeObject is actually another nested container
            if (expression != null)
                container.AddContent(expression.runtimeObject);
            else if (listDefinition != null)
                container.AddContent(listDefinition.runtimeObject);

            _runtimeAssignment = new Runtime.VariableAssignment(variableName, isNewTemporaryDeclaration);
            container.AddContent(_runtimeAssignment);

            return container;
        }

        public override void ResolveReferences(Story context) {
            base.ResolveReferences(context);

            // List definitions are checked for conflicts separately
            if (isDeclaration && listDefinition == null)
                context.CheckForNamingCollisions(this, variableIdentifier,
                    isGlobalDeclaration ? Story.SymbolType.Var : Story.SymbolType.Temp);

            // Initial VAR x = [intialValue] declaration, not re-assignment
            if (isGlobalDeclaration) {
                var variableReference = expression as VariableReference;
                if (variableReference && !variableReference.isConstantReference &&
                    !variableReference.isListItemReference)
                    Error(
                        "global variable assignments cannot refer to other variables, only literal values, constants and list items");
            }

            if (!isNewTemporaryDeclaration) {
                var resolvedVarAssignment = context.ResolveVariableWithName(variableName, this);
                if (!resolvedVarAssignment.found) {
                    if (story.constants.ContainsKey(variableName))
                        Error(
                            "Can't re-assign to a constant (do you need to use VAR when declaring '" + variableName +
                            "'?)", this);
                    else
                        Error("Variable could not be found to assign to: '" + variableName + "'", this);
                }

                // A runtime assignment may not have been generated if it's the initial global declaration,
                // since these are hoisted out and handled specially in Story.ExportRuntime.
                if (_runtimeAssignment != null)
                    _runtimeAssignment.isGlobal = resolvedVarAssignment.isGlobal;
            }
        }
    }
}