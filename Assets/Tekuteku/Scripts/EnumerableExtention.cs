using System;
using System.Collections.Generic;

public static class EnumerableExtention {
	public static T SelectRandom<T>(this IEnumerable<T> self) {
		List<T> list = new List<T>(self);
		return list[new Random().Next(list.Count)];
	}

}
