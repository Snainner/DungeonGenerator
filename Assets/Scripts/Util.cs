using UnityEngine;
using Random = System.Random;
using System.Collections.Generic;

namespace Utils
{

   public static class ListUtils
   {
        static Random rng = new Random();
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[n] = value;
            }
        }
   }

   
}
