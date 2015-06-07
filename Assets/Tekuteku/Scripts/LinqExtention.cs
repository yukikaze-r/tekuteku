using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class LinqExtension {
	public static void ForEach<T>(this IEnumerable<T> source, System.Action<T> action) {
		foreach (T item in source) action(item);
	}

	public static void ForEach<T>(this IEnumerable<T> source, System.Action<T, int> action) {
		int i = 0;
		foreach (T item in source) action(item, i++);
	}

	public static IEnumerable<T> WhereMin<T, V>(this IEnumerable<T> source, System.Func<T, V> eval) where V : IComparable<V> {
		return WhereMinMax(source, eval, true);
	}

	public static IEnumerable<T> WhereMax<T, V>(this IEnumerable<T> source, System.Func<T, V> eval) where V : IComparable<V> {
		return WhereMinMax(source, eval, false);
	}

	private static IEnumerable<T> WhereMinMax<T, V>(this IEnumerable<T> source, System.Func<T, V> eval, bool isMin) where V : IComparable<V> {
		List<T> resultList = null;
		T result = default(T);
		V maxVal = default(V);
		int phase = 0;
		foreach (T t in source) {
			if (phase == 0) {
				result = t;
				phase++;
			} else {
				V v = eval(t);
				if (phase == 1) {
					maxVal = eval(result);
					phase++;
				}
				int n = v.CompareTo(maxVal);
				if (n == 0) {
					if (resultList == null) {
						resultList = new List<T>();
						resultList.Add(result);
					}
					resultList.Add(t);
				} else if (isMin ? n < 0 : n > 0) {
					resultList = null;
					maxVal = v;
					result = t;
				}
			}
		}
		if (resultList != null) {
			return resultList;
		} else {
			return new T[] { result };
		}
	}

	public static T SafeFirstOrDefault<T>(this IEnumerable<T> source) {
		IEnumerator<T> enumerator = source.GetEnumerator();
		if (enumerator.MoveNext()) {
			return enumerator.Current;
		} else {
			return default(T);
		}
	}

	// 適当に作りました
	public static T SafeFirstOrDefault<T>(this IEnumerable<T> source, Func<T, bool> func) {

		foreach (T t in source) {
			if (func(t)) {
				return t;
			}
		}

		return default(T);
	}

	public static int SafeIntSum<T>(this IEnumerable<T> source, Func<T, int> func) {
		int result = 0;
		foreach (T t in source) {
			result += func(t);
		}

		return result;
	}


	public static long SafeLongSum<T>(this IEnumerable<T> source, Func<T, long> func) {
		long result = 0;
		foreach (T t in source) {
			result += func(t);
		}

		return result;
	}

	public static IEnumerable<T> SafeSelect<T, U>(this IEnumerable<U> source, Func<U, T> func) {
		foreach (U u in source) {
			yield return func(u);
		}
	}
}
