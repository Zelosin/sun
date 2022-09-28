using System.Collections.Generic;
using Ink.Runtime;

namespace Ink.Parsed {
    public interface IWeavePoint {
        int indentationDepth { get; }
        Container runtimeContainer { get; }
        List<Object> content { get; }
        string name { get; }
        Identifier identifier { get; }
    }
}