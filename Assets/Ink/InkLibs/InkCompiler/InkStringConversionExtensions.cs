using System.Collections.Generic;

namespace Ink {
    public static class InkStringConversionExtensions {
        public static string[] ToStringsArray<T>(this List<T> list) {
            var count = list.Count;
            var strings = new string[count];

            for (var i = 0; i < count; i++) strings[i] = list[i].ToString();

            return strings;
        }
    }
}