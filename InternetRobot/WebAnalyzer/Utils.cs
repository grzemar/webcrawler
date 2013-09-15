using System.Collections.Generic;

namespace WebAnalyzer
{
    public static class Utils
    {
        public static void MergeDictionaries<TKey, Tvalue>(this IDictionary<string, int> first, IDictionary<string, int> second)
        {
            if (second == null) return;
            if (first == null) first = new Dictionary<string, int>();
            foreach (var item in second)
                if (first.ContainsKey(item.Key))
                {
                    int firstValue;
                    first.TryGetValue(item.Key, out firstValue);
                    first.Remove(item.Key);
                    first.Add(item.Key, firstValue + item.Value);
                }
                else
                {
                    first.Add(item.Key, item.Value);
                }
        }

    }
}
