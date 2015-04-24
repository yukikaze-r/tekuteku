using System.Collections.Generic;

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

	public VectorInt2 GetNext(Direction4 d, int distance = 1) {
		VectorInt2 l = this;
		l.Move(d, distance);
		return l;
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

	public IEnumerable<VectorInt2> GetAroundPositions4() {
		for (int i = 1; i <= 4; i++) {
			VectorInt2 lv = this;
			lv.Move((Direction4)i);
			yield return lv;
		}
	}

    public override int GetHashCode () {
        return base.GetHashCode ();
    }

	public static bool operator ==(VectorInt2 left, VectorInt2 right) {
        return left.Equals (right);
    }

	public static bool operator !=(VectorInt2 left, VectorInt2 right) {
        return !left.Equals (right);
    }
}
