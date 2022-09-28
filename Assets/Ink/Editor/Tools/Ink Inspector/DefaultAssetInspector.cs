using UnityEditor;
using UnityEngine;

namespace Ink.UnityIntegration {
    public abstract class DefaultAssetInspector {
        // Reference to the actual editor we draw to
        public Editor editor;

        // Shortcut to the serializedObject
        public SerializedObject serializedObject;

        // Shortcut to the target object
        public Object target;

        public abstract bool IsValid(string assetPath);
        public virtual void OnEnable() { }
        public virtual void OnDisable() { }
        public virtual void OnHeaderGUI() { }
        public virtual void OnInspectorGUI() { }
    }
}