using System.Collections.Generic;

namespace Ink.Runtime {
    public class Flow {
        public CallStack callStack;
        public List<Choice> currentChoices;
        public string name;
        public List<Object> outputStream;

        public Flow(string name, Story story) {
            this.name = name;
            callStack = new CallStack(story);
            outputStream = new List<Object>();
            currentChoices = new List<Choice>();
        }

        public Flow(string name, Story story, Dictionary<string, object> jObject) {
            this.name = name;
            callStack = new CallStack(story);
            callStack.SetJsonToken((Dictionary<string, object>)jObject["callstack"], story);
            outputStream = Json.JArrayToRuntimeObjList((List<object>)jObject["outputStream"]);
            currentChoices = Json.JArrayToRuntimeObjList<Choice>((List<object>)jObject["currentChoices"]);

            // choiceThreads is optional
            object jChoiceThreadsObj;
            jObject.TryGetValue("choiceThreads", out jChoiceThreadsObj);
            LoadFlowChoiceThreads((Dictionary<string, object>)jChoiceThreadsObj, story);
        }

        public void WriteJson(SimpleJson.Writer writer) {
            writer.WriteObjectStart();

            writer.WriteProperty("callstack", callStack.WriteJson);
            writer.WriteProperty("outputStream", w => Json.WriteListRuntimeObjs(w, outputStream));

            // choiceThreads: optional
            // Has to come BEFORE the choices themselves are written out
            // since the originalThreadIndex of each choice needs to be set
            var hasChoiceThreads = false;
            foreach (var c in currentChoices) {
                c.originalThreadIndex = c.threadAtGeneration.threadIndex;

                if (callStack.ThreadWithIndex(c.originalThreadIndex) == null) {
                    if (!hasChoiceThreads) {
                        hasChoiceThreads = true;
                        writer.WritePropertyStart("choiceThreads");
                        writer.WriteObjectStart();
                    }

                    writer.WritePropertyStart(c.originalThreadIndex);
                    c.threadAtGeneration.WriteJson(writer);
                    writer.WritePropertyEnd();
                }
            }

            if (hasChoiceThreads) {
                writer.WriteObjectEnd();
                writer.WritePropertyEnd();
            }


            writer.WriteProperty("currentChoices", w => {
                w.WriteArrayStart();
                foreach (var c in currentChoices)
                    Json.WriteChoice(w, c);
                w.WriteArrayEnd();
            });


            writer.WriteObjectEnd();
        }

        // Used both to load old format and current
        public void LoadFlowChoiceThreads(Dictionary<string, object> jChoiceThreads, Story story) {
            foreach (var choice in currentChoices) {
                var foundActiveThread = callStack.ThreadWithIndex(choice.originalThreadIndex);
                if (foundActiveThread != null) {
                    choice.threadAtGeneration = foundActiveThread.Copy();
                }
                else {
                    var jSavedChoiceThread =
                        (Dictionary<string, object>)jChoiceThreads[choice.originalThreadIndex.ToString()];
                    choice.threadAtGeneration = new CallStack.Thread(jSavedChoiceThread, story);
                }
            }
        }
    }
}