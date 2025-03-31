using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bizza.Library
{
    public static class Enumerator
    {
        public static IEnumerator Then(this IEnumerator enumerator, Action onComplete)
        {
            //while (enumerator.MoveNext())
            //{
            //    yield return enumerator.Current;
            //}
            yield return enumerator;
            onComplete();
        }

        public static IEnumerator Add(this IEnumerator self, IEnumerator other)
        {
            while (self.MoveNext())
            {
                yield return self.Current;
            }
            while (other.MoveNext())
            {
                yield return other.Current;
            }
        }
    }
}
