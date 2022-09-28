using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Ink.Runtime {
    /// <summary>
    ///     Base class for all ink runtime content.
    /// </summary>
    public /* TODO: abstract */ class Object {
        // TODO: Come up with some clever solution for not having
        // to have debug metadata on the object itself, perhaps
        // for serialisation purposes at least.
        private Path _path;

        /// <summary>
        ///     Runtime.Objects can be included in the main Story as a hierarchy.
        ///     Usually parents are Container objects. (TODO: Always?)
        /// </summary>
        /// <value>The parent.</value>
        public Object parent { get; set; }

        public DebugMetadata debugMetadata {
            get {
                if (ownDebugMetadata == null)
                    if (parent)
                        return parent.debugMetadata;

                return ownDebugMetadata;
            }

            set => ownDebugMetadata = value;
        }

        public DebugMetadata ownDebugMetadata { get; private set; }

        public Path path {
            get {
                if (_path == null) {
                    if (parent == null) {
                        _path = new Path();
                    }
                    else {
                        // Maintain a Stack so that the order of the components
                        // is reversed when they're added to the Path.
                        // We're iterating up the hierarchy from the leaves/children to the root.
                        var comps = new Stack<Path.Component>();

                        var child = this;
                        var container = child.parent as Container;

                        while (container) {
                            var namedChild = child as INamedContent;
                            if (namedChild != null && namedChild.hasValidName)
                                comps.Push(new Path.Component(namedChild.name));
                            else
                                comps.Push(new Path.Component(container.content.IndexOf(child)));

                            child = container;
                            container = container.parent as Container;
                        }

                        _path = new Path(comps);
                    }
                }

                return _path;
            }
        }

        public Container rootContentContainer {
            get {
                var ancestor = this;
                while (ancestor.parent) ancestor = ancestor.parent;
                return ancestor as Container;
            }
        }

        public int? DebugLineNumberOfPath(Path path) {
            if (path == null)
                return null;

            // Try to get a line number from debug metadata
            var root = rootContentContainer;
            if (root) {
                var targetContent = root.ContentAtPath(path).obj;
                if (targetContent) {
                    var dm = targetContent.debugMetadata;
                    if (dm != null) return dm.startLineNumber;
                }
            }

            return null;
        }

        public SearchResult ResolvePath(Path path) {
            if (path.isRelative) {
                var nearestContainer = this as Container;
                if (!nearestContainer) {
                    Debug.Assert(parent != null, "Can't resolve relative path because we don't have a parent");
                    nearestContainer = parent as Container;
                    Debug.Assert(nearestContainer != null, "Expected parent to be a container");
                    Debug.Assert(path.GetComponent(0).isParent);
                    path = path.tail;
                }

                return nearestContainer.ContentAtPath(path);
            }

            return rootContentContainer.ContentAtPath(path);
        }

        public Path ConvertPathToRelative(Path globalPath) {
            // 1. Find last shared ancestor
            // 2. Drill up using ".." style (actually represented as "^")
            // 3. Re-build downward chain from common ancestor

            var ownPath = path;

            var minPathLength = Math.Min(globalPath.length, ownPath.length);
            var lastSharedPathCompIndex = -1;

            for (var i = 0; i < minPathLength; ++i) {
                var ownComp = ownPath.GetComponent(i);
                var otherComp = globalPath.GetComponent(i);

                if (ownComp.Equals(otherComp))
                    lastSharedPathCompIndex = i;
                else
                    break;
            }

            // No shared path components, so just use global path
            if (lastSharedPathCompIndex == -1)
                return globalPath;

            var numUpwardsMoves = ownPath.length - 1 - lastSharedPathCompIndex;

            var newPathComps = new List<Path.Component>();

            for (var up = 0; up < numUpwardsMoves; ++up)
                newPathComps.Add(Path.Component.ToParent());

            for (var down = lastSharedPathCompIndex + 1; down < globalPath.length; ++down)
                newPathComps.Add(globalPath.GetComponent(down));

            var relativePath = new Path(newPathComps, true);
            return relativePath;
        }

        // Find most compact representation for a path, whether relative or global
        public string CompactPathString(Path otherPath) {
            string globalPathStr = null;
            string relativePathStr = null;
            if (otherPath.isRelative) {
                relativePathStr = otherPath.componentsString;
                globalPathStr = path.PathByAppendingPath(otherPath).componentsString;
            }
            else {
                var relativePath = ConvertPathToRelative(otherPath);
                relativePathStr = relativePath.componentsString;
                globalPathStr = otherPath.componentsString;
            }

            if (relativePathStr.Length < globalPathStr.Length)
                return relativePathStr;
            return globalPathStr;
        }

        public virtual Object Copy() {
            throw new NotImplementedException(GetType().Name + " doesn't support copying");
        }

        public void SetChild<T>(ref T obj, T value) where T : Object {
            if (obj)
                obj.parent = null;

            obj = value;

            if (obj)
                obj.parent = this;
        }

        /// Allow implicit conversion to bool so you don't have to do:
        /// if( myObj != null ) ...
        public static implicit operator bool(Object obj) {
            var isNull = ReferenceEquals(obj, null);
            return !isNull;
        }

        /// Required for implicit bool comparison
        public static bool operator ==(Object a, Object b) {
            return ReferenceEquals(a, b);
        }

        /// Required for implicit bool comparison
        public static bool operator !=(Object a, Object b) {
            return !(a == b);
        }

        /// Required for implicit bool comparison
        public override bool Equals(object obj) {
            return ReferenceEquals(obj, this);
        }

        /// Required for implicit bool comparison
        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}