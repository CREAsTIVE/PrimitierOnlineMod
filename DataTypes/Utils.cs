using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuchiGames.POM.Shared
{
    public static class IEnumerableUtils
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
                action(item);
        }

        public static int IndexOf<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            int index = 0;
            foreach (var item in enumerable)
            {
                if (predicate(item))
                    return index;
                index++;
            }
            return -1;
        }

        public static int IndexOf<T>(this IEnumerable<T> enumerable, T obj) =>
            enumerable.IndexOf(other => other?.Equals(obj) ?? false);
    }
    public static class ObjectUtils
    {
        public static T Apply<T>(this T value, Action<T> action)
        {
            action(value);
            return value;
        }
        public static R Let<T, R>(this T value, Func<T, R> action) => action(value);
    }
}
