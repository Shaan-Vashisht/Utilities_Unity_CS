using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Objects
{
    /// <summary>
    /// A class that allows the serialization of a key-value pair based collection.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    public class SerializableDictionary<TKey, TValue>
    {
        [SerializeField] List<ValuePair<TKey, TValue>> pairs;

        public TValue this[TKey key]
        {
            get => GetPair(key).Item2;
            set => GetPair(key).Item2 = value;
        }

        private ValuePair<TKey, TValue> GetPair(TKey key)
        {
            for (int i = 0; i < pairs.Count; i++)
            {
                if (key.Equals(pairs[i].Item1))
                    return pairs[i];
            }

            return null;
        }

        public void Add(TKey key, TValue value)
        {
            if (!HasKey(key))
            {
                pairs.Add(new ValuePair<TKey, TValue>(key, value));
            }
        }

        public void RemoveAt(int index)
        {
            pairs.RemoveAt(index);
        }

        public int Count => pairs.Count;

        public bool HasKey(TKey key)
        {
            for (int i = 0; i < pairs.Count; i++)
            {
                if (key.Equals(pairs[i].Item1))
                    return true;
            }

            return false;
        }
    }
}