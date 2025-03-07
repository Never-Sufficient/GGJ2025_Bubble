using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils
{
    public static class RandomUtils
    {
        public static int RandomSelect(List<float> probability)
        {
            if (Mathf.Abs(probability.Sum() - 1.0f) > 0.0001f)
                return -1;
            var sum = 0.0f;
            var randomValue = Random.Range(0.0f, 1.0f);
            for (var i = 0; i < probability.Count; i++)
            {
                sum += probability[i];
                if (sum > randomValue)
                    return i;
            }

            return probability.Count - 1;
        }
    }
}