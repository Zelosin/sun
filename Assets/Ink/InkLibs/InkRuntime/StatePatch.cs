using System.Collections.Generic;

namespace Ink.Runtime {
    public class StatePatch {
        public StatePatch(StatePatch toCopy) {
            if (toCopy != null) {
                globals = new Dictionary<string, Object>(toCopy.globals);
                changedVariables = new HashSet<string>(toCopy.changedVariables);
                visitCounts = new Dictionary<Container, int>(toCopy.visitCounts);
                turnIndices = new Dictionary<Container, int>(toCopy.turnIndices);
            }
            else {
                globals = new Dictionary<string, Object>();
                changedVariables = new HashSet<string>();
                visitCounts = new Dictionary<Container, int>();
                turnIndices = new Dictionary<Container, int>();
            }
        }

        public Dictionary<string, Object> globals { get; }

        public HashSet<string> changedVariables { get; } = new();

        public Dictionary<Container, int> visitCounts { get; } = new();

        public Dictionary<Container, int> turnIndices { get; } = new();

        public bool TryGetGlobal(string name, out Object value) {
            return globals.TryGetValue(name, out value);
        }

        public void SetGlobal(string name, Object value) {
            globals[name] = value;
        }

        public void AddChangedVariable(string name) {
            changedVariables.Add(name);
        }

        public bool TryGetVisitCount(Container container, out int count) {
            return visitCounts.TryGetValue(container, out count);
        }

        public void SetVisitCount(Container container, int count) {
            visitCounts[container] = count;
        }

        public void SetTurnIndex(Container container, int index) {
            turnIndices[container] = index;
        }

        public bool TryGetTurnIndex(Container container, out int index) {
            return turnIndices.TryGetValue(container, out index);
        }
    }
}