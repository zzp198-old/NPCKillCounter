using System.Collections.Generic;

namespace NPCKillCounter;

public class DefaultDictionary<TKey, TValue> : Dictionary<TKey, TValue> where TValue : new()
{
    public new TValue this[TKey key]
    {
        get
        {
            if (!TryGetValue(key, out var val))
            {
                Add(key, new TValue());
            }

            return val;
        }
        set => base[key] = value;
    }
}