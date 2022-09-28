using System;
using System.Collections.Generic;
using Ink.Runtime;

namespace Ink.Parsed {
    public class ListDefinition : Object {
        private Dictionary<string, ListElementDefinition> _elementsByName;
        public Identifier identifier;
        public List<ListElementDefinition> itemDefinitions;

        public VariableAssignment variableAssignment;

        public ListDefinition(List<ListElementDefinition> elements) {
            itemDefinitions = elements;

            var currentValue = 1;
            foreach (var e in itemDefinitions) {
                if (e.explicitValue != null)
                    currentValue = e.explicitValue.Value;

                e.seriesValue = currentValue;

                currentValue++;
            }

            AddContent(elements);
        }

        public Runtime.ListDefinition runtimeListDefinition {
            get {
                var allItems = new Dictionary<string, int>();
                foreach (var e in itemDefinitions)
                    if (!allItems.ContainsKey(e.name))
                        allItems.Add(e.name, e.seriesValue);
                    else
                        Error("List '" + identifier + "' contains dupicate items called '" + e.name + "'");

                return new Runtime.ListDefinition(identifier?.name, allItems);
            }
        }

        public override string typeName => "List definition";

        public ListElementDefinition ItemNamed(string itemName) {
            if (_elementsByName == null) {
                _elementsByName = new Dictionary<string, ListElementDefinition>();
                foreach (var el in itemDefinitions) _elementsByName[el.name] = el;
            }

            ListElementDefinition foundElement;
            if (_elementsByName.TryGetValue(itemName, out foundElement))
                return foundElement;

            return null;
        }

        public override Runtime.Object GenerateRuntimeObject() {
            var initialValues = new InkList();
            foreach (var itemDef in itemDefinitions)
                if (itemDef.inInitialList) {
                    var item = new InkListItem(identifier?.name, itemDef.name);
                    initialValues[item] = itemDef.seriesValue;
                }

            // Set origin name, so
            initialValues.SetInitialOriginName(identifier?.name);

            return new ListValue(initialValues);
        }

        public override void ResolveReferences(Story context) {
            base.ResolveReferences(context);

            context.CheckForNamingCollisions(this, identifier, Story.SymbolType.List);
        }
    }

    public class ListElementDefinition : Object {
        public int? explicitValue;
        public Identifier identifier;
        public bool inInitialList;
        public int seriesValue;

        public ListElementDefinition(Identifier identifier, bool inInitialList, int? explicitValue = null) {
            this.identifier = identifier;
            this.inInitialList = inInitialList;
            this.explicitValue = explicitValue;
        }

        public string name => identifier?.name;

        public string fullName {
            get {
                var parentList = parent as ListDefinition;
                if (parentList == null)
                    throw new Exception("Can't get full name without a parent list");

                return parentList.identifier + "." + name;
            }
        }

        public override string typeName => "List element";

        public override Runtime.Object GenerateRuntimeObject() {
            throw new NotImplementedException();
        }

        public override void ResolveReferences(Story context) {
            base.ResolveReferences(context);

            context.CheckForNamingCollisions(this, identifier, Story.SymbolType.ListItem);
        }
    }
}