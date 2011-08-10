namespace Collections.Tests.Helpers
{
    using System;
    using System.Collections.Generic;

    static class EnumerableEx
    {
        public static IEnumerable<TResult> ZipFull<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");
            if (resultSelector == null) throw new ArgumentNullException("resultSelector");
            return ZipFullIterator(first, second, resultSelector);
        }

        private static IEnumerable<TResult> ZipFullIterator<TFirst, TSecond, TResult>(IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        {
            using (var e1 = first.GetEnumerator())
            using (var e2 = second.GetEnumerator())
            {
                bool e1Moved;
                bool e2Moved;
                while ((e1Moved = e1.MoveNext()) | (e2Moved = e2.MoveNext()))
                {
                    yield return resultSelector(e1Moved ? e1.Current : default(TFirst), e2Moved ? e2.Current : default(TSecond));
                }
            }
        }
    }
}
