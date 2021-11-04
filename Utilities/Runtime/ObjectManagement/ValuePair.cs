namespace Utilities.Objects
{
    [System.Serializable]
    public class ValuePair<T1, T2>
    {
        public T1 Item1;
        public T2 Item2;

        public ValuePair(T1 Item1, T2 Item2)
        {
            this.Item1 = Item1;
            this.Item2 = Item2;
        }
    }
}