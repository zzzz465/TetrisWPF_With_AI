using System;
using System.Collections.Generic;
using System.Linq;

namespace XUnitTest_Tetris
{
    public static class CollectionAssert
    {
        public static void CollectionSameWithoutOrder<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            if(expected.Count() != actual.Count())
                throw new Xunit.Sdk.CollectionException(actual, expected.Count(), actual.Count(), -1, null);

            foreach(var element in actual)
            {
                if(!expected.Contains(element))
                    throw new Xunit.Sdk.CollectionException(actual, expected.Count(), actual.Count(), -1, null);
            }
        }
    }
}