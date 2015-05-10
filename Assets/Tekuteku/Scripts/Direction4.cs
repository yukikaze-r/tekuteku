using UnityEngine;
using System.Collections;

public enum Direction4 {
	NONE, UP, DOWN, RIGHT, LEFT
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

	public static Direction4 Reverse(this Direction4 d) {
		switch (d) {
			case Direction4.UP:
				return Direction4.DOWN;
			case Direction4.DOWN:
				return Direction4.UP;
			case Direction4.RIGHT:
				return Direction4.LEFT;
			case Direction4.LEFT:
				return Direction4.RIGHT;
		}
		throw new System.Exception();
	}

	public static Direction4 Rotate(this Direction4 d) {
		switch (d) {
			case Direction4.UP:
				return Direction4.RIGHT;
			case Direction4.RIGHT:
				return Direction4.DOWN;
			case Direction4.DOWN:
				return Direction4.LEFT;
			case Direction4.LEFT:
				return Direction4.UP;
		}
		throw new System.Exception();
	}
}