using System.Collections.Generic;

namespace Ink.Parsed {
    public class ExternalDeclaration : Object, INamedContent {
        public ExternalDeclaration(Identifier identifier, List<string> argumentNames) {
            this.identifier = identifier;
            this.argumentNames = argumentNames;
        }

        public Identifier identifier { get; set; }
        public List<string> argumentNames { get; set; }

        public string name => identifier?.name;

        public override Runtime.Object GenerateRuntimeObject() {
            story.AddExternal(this);

            // No runtime code exists for an external, only metadata
            return null;
        }
    }
}