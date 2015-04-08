using System.Collections;

public struct VectorInt2 {

	public int x;
	public int y;

	public VectorInt2(int x, int y) {
		this.x = x;
		this.y = y;
	}


	public bool IsInboundRect(int sx, int sy, int dx, int dy) {
		return sx <= x && sy <= y && x < sx + dx && y < sy + dy;
	}

	public static VectorInt2 operator +(VectorInt2 v1, VectorInt2 v2) {
		return new VectorInt2(v1.x + v2.x, v1.y + v2.y);
	}

	public static VectorInt2 operator -(VectorInt2 v1, VectorInt2 v2) {
		return new VectorInt2(v1.x - v2.x, v1.y - v2.y);
	}


	public void Move(Direction4 d, int distance = 1) {
		switch (d) {
			case Direction4.UP:
				y += distance;
				break;
			case Direction4.DOWN:
				y -= distance;
				break;
			case Direction4.RIGHT:
				x += distance;
				break;
			case Direction4.LEFT:
				x -= distance;
				break;
		}

	}

	public Direction4 Direction4 {
		get {
			if (x == 0 && y > 0) {
				return Direction4.UP;
			}
			if (x == 0 && y < 0) {
				return Direction4.DOWN;
			}
			if (x > 0 && y == 0) {
				return Direction4.RIGHT;
			}
			if (x < 0 && y == 0) {
				return Direction4.LEFT;
			}
			throw new System.Exception("not direction4");
		}
	}
}
