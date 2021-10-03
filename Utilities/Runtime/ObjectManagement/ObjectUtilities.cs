namespace Utilities.Objects
{
    public static class ObjectUtilities
    {
        /// <summary>
        /// Makes sure that the given array is of a certain length.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"> The array to lock. </param>
        /// <param name="length"> The desired length of the array. </param>
        public static void LockArrayLength<T>(ref T[] array, int length)
        {
            if (array.Length < length)
            {
                T[] temp = new T[array.Length];
                array.CopyTo(temp, 0);

                array = new T[length];
                temp.CopyTo(array, 0);

                for (int i = temp.Length; i < array.Length; i++)
                {
                    array[i] = default(T);
                }
            }
            else if (array.Length > length)
            {
                T[] temp = new T[length];
                for (int i = 0; i < length; i++)
                {
                    temp[i] = array[i];
                }

                array = new T[length];
                temp.CopyTo(array, 0);
            }
        }

        /// <summary>
        /// Makes sure that the given SerializableArray array and the 
        /// SerializeableArrays within it are of a certain length.
        /// </summary>
        /// <typeparam name="TArray"></typeparam>
        /// <param name="array"> The SerializableArray array to lock. </param>
        /// <param name="Length"> The desired length of the SerializableArray array. </param>
        /// <param name="length"> The desired length of the SerializableArrays within. </param>
        public static void Lock2DArrayLength<TArray>(ref SerializableArray<TArray>[] array, int Length, int length)
        {
            LockArrayLength(ref array, Length);

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == null)
                    array[i] = new SerializableArray<TArray>(0);

                LockArrayLength(ref array[i].a, length);
            }
        }
    }
}
