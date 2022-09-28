using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Ink.Runtime {
    public class Path : IEquatable<Path> {
        private static readonly string parentId = "^";

        private readonly List<Component> _components;
        private string _componentsString;

        public Path() {
            _components = new List<Component>();
        }

        public Path(Component head, Path tail) : this() {
            _components.Add(head);
            _components.AddRange(tail._components);
        }

        public Path(IEnumerable<Component> components, bool relative = false) : this() {
            _components.AddRange(components);
            isRelative = relative;
        }

        public Path(string componentsString) : this() {
            this.componentsString = componentsString;
        }

        public bool isRelative { get; private set; }

        public Component head {
            get {
                if (_components.Count > 0)
                    return _components.First();
                return null;
            }
        }

        public Path tail {
            get {
                if (_components.Count >= 2) {
                    var tailComps = _components.GetRange(1, _components.Count - 1);
                    return new Path(tailComps);
                }

                return self;
            }
        }

        public int length => _components.Count;

        public Component lastComponent {
            get {
                var lastComponentIdx = _components.Count - 1;
                if (lastComponentIdx >= 0)
                    return _components[lastComponentIdx];
                return null;
            }
        }

        public bool containsNamedComponent {
            get {
                foreach (var comp in _components)
                    if (!comp.isIndex)
                        return true;
                return false;
            }
        }

        public static Path self {
            get {
                var path = new Path();
                path.isRelative = true;
                return path;
            }
        }

        public string componentsString {
            get {
                if (_componentsString == null) {
                    _componentsString = StringExt.Join(".", _components);
                    if (isRelative) _componentsString = "." + _componentsString;
                }

                return _componentsString;
            }
            private set {
                _components.Clear();

                _componentsString = value;

                // Empty path, empty components
                // (path is to root, like "/" in file system)
                if (string.IsNullOrEmpty(_componentsString))
                    return;

                // When components start with ".", it indicates a relative path, e.g.
                //   .^.^.hello.5
                // is equivalent to file system style path:
                //  ../../hello/5
                if (_componentsString[0] == '.') {
                    isRelative = true;
                    _componentsString = _componentsString.Substring(1);
                }
                else {
                    isRelative = false;
                }

                var componentStrings = _componentsString.Split('.');
                foreach (var str in componentStrings) {
                    int index;
                    if (int.TryParse(str, out index))
                        _components.Add(new Component(index));
                    else
                        _components.Add(new Component(str));
                }
            }
        }

        public bool Equals(Path otherPath) {
            if (otherPath == null)
                return false;

            if (otherPath._components.Count != _components.Count)
                return false;

            if (otherPath.isRelative != isRelative)
                return false;

            return otherPath._components.SequenceEqual(_components);
        }

        public Component GetComponent(int index) {
            return _components[index];
        }

        public Path PathByAppendingPath(Path pathToAppend) {
            var p = new Path();

            var upwardMoves = 0;
            for (var i = 0; i < pathToAppend._components.Count; ++i)
                if (pathToAppend._components[i].isParent)
                    upwardMoves++;
                else
                    break;

            for (var i = 0; i < _components.Count - upwardMoves; ++i) p._components.Add(_components[i]);

            for (var i = upwardMoves; i < pathToAppend._components.Count; ++i)
                p._components.Add(pathToAppend._components[i]);

            return p;
        }

        public Path PathByAppendingComponent(Component c) {
            var p = new Path();
            p._components.AddRange(_components);
            p._components.Add(c);
            return p;
        }

        public override string ToString() {
            return componentsString;
        }

        public override bool Equals(object obj) {
            return Equals(obj as Path);
        }

        public override int GetHashCode() {
            // TODO: Better way to make a hash code!
            return ToString().GetHashCode();
        }

        // Immutable Component
        public class Component : IEquatable<Component> {
            public Component(int index) {
                Debug.Assert(index >= 0);
                this.index = index;
                name = null;
            }

            public Component(string name) {
                Debug.Assert(name != null && name.Length > 0);
                this.name = name;
                index = -1;
            }

            public int index { get; }
            public string name { get; }
            public bool isIndex => index >= 0;

            public bool isParent => name == parentId;

            public bool Equals(Component otherComp) {
                if (otherComp != null && otherComp.isIndex == isIndex) {
                    if (isIndex)
                        return index == otherComp.index;
                    return name == otherComp.name;
                }

                return false;
            }

            public static Component ToParent() {
                return new Component(parentId);
            }

            public override string ToString() {
                if (isIndex)
                    return index.ToString();
                return name;
            }

            public override bool Equals(object obj) {
                return Equals(obj as Component);
            }

            public override int GetHashCode() {
                if (isIndex)
                    return index;
                return name.GetHashCode();
            }
        }
    }
}