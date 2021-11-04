using UnityEngine;

namespace Utilities.Objects
{
    [System.Serializable]
    public class ChancedValues<T>
    {
        public ChanceValuePair<T>[] chancedValues;
        public int Length { get => chancedValues.Length; }
        public T this[int i] => chancedValues[i].value;

        private float[] normalizedChances;

        public T GetRandom()
        {
            NormalizeChances();
            float r = Random.value;

            float currentRange = 0;
            for (int i = 0; i < normalizedChances.Length; i++)
            {
                currentRange += normalizedChances[i];

                if (r < currentRange) return chancedValues[i].value;
            }
            return default(T);
        }
        public T GetRandom(out int index)
        {
            NormalizeChances();
            float r = Random.value;

            float currentRange = 0;
            for (int i = 0; i < normalizedChances.Length; i++)
            {
                currentRange += normalizedChances[i];

                if (r < currentRange)
                {
                    index = i;
                    return chancedValues[i].value;
                }
            }
            index = -1;
            return default(T);
        }

        private void NormalizeChances()
        {
            normalizedChances = new float[chancedValues.Length];

            float totalSum = 0;
            for (int i = 0; i < chancedValues.Length; i++)
            {
                totalSum += chancedValues[i].chance;
            }

            for (int i = 0; i < chancedValues.Length; i++)
            {
                normalizedChances[i] = chancedValues[i].chance / totalSum;
            }
        }
    }

    [System.Serializable]
    public class ChanceValuePair<T>
    {
        public float chance;
        public T value;
    }
}