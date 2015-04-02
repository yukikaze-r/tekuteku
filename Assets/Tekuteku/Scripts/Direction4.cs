using UnityEngine;
using System.Collections;

public enum Direction4 {
	UP, DOWN, RIGHT, LEFT
}

public static class Direction4Extention {




	public static Quaternion Quaternion(this Direction4 d) {
		switch (d) {
			case Direction4.UP:
				return UnityEngine.Quaternion.Euler(0, 0, 0);
			case Direction4.DOWN:
				return UnityEngine.Quaternion.Euler(0, 180, 0);
			case Direction4.RIGHT:
				return UnityEngine.Quaternion.Euler(0, 90, 0);
			case Direction4.LEFT:
				return UnityEngine.Quaternion.Euler(0, 270, 0);
		}
		throw new System.Exception();
	}
}