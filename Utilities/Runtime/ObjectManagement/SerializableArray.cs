namespace Utilities.Objects
{
    /// <summary>
    /// A special class used to serialize 2D arrays in the inspector.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class SerializableArray<T>
    {
        public T[] a = new T[0];

        public SerializableArray(int length)
        {
            a = new T[length];
        }

        public int Length { get => a.Length; }
        public T this[int i] { get => a[i]; set => a[i] = value; }
    }
}
