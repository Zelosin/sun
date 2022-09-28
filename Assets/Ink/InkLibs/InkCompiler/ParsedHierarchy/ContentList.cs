using System.Collections.Generic;
using System.Text;
using Ink.Runtime;

namespace Ink.Parsed {
    public class ContentList : Object {
        public ContentList(List<Object> objects) {
            if (objects != null)
                AddContent(objects);
        }

        public ContentList(params Object[] objects) {
            if (objects != null) {
                var objList = new List<Object>(objects);
                AddContent(objList);
            }
        }

        public ContentList() { }

        public bool dontFlatten { get; set; }

        public Container runtimeContainer => (Container)runtimeObject;

        public void TrimTrailingWhitespace() {
            for (var i = content.Count - 1; i >= 0; --i) {
                var text = content[i] as Text;
                if (text == null)
                    break;

                text.text = text.text.TrimEnd(' ', '\t');
                if (text.text.Length == 0)
                    content.RemoveAt(i);
                else
                    break;
            }
        }

        public override Runtime.Object GenerateRuntimeObject() {
            var container = new Container();
            if (content != null)
                foreach (var obj in content) {
                    var contentObjRuntime = obj.runtimeObject;

                    // Some objects (e.g. author warnings) don't generate runtime objects
                    if (contentObjRuntime)
                        container.AddContent(contentObjRuntime);
                }

            if (dontFlatten)
                story.DontFlattenContainer(container);

            return container;
        }

        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append("ContentList(");
            sb.Append(string.Join(", ", content.ToStringsArray()));
            sb.Append(")");
            return sb.ToString();
        }
    }
}