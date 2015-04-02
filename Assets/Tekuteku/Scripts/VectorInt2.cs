using UnityEngine;
using System.Collections;

public struct VectorInt2 {

	public int x;
	public int y;

	public VectorInt2(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public void Move(Direction4 d, int distance = 1) {
		switch (d) {
			case Direction4.UP:
				y -= distance;
				break;
			case Direction4.DOWN:
				y += distance;
				break;
			case Direction4.RIGHT:
				x += distance;
				break;
			case Direction4.LEFT:
				x -= distance;
				break;
		}

	}

	public bool IsInboundRect(int sx, int sy, int dx, int dy) {
		return sx <= x && sy <= y && x < sx + dx && y < sy + dy;
	}
}
