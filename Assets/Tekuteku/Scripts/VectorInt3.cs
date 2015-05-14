using System.Collections.Generic;

public struct VectorInt3 {

	public int x;
	public int y;
	public int z;

	public VectorInt3(int x, int y, int z) {
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public VectorInt2 xy {
		get {
			return new VectorInt2(x, y);
		}
	}

	public static VectorInt3 operator +(VectorInt3 v1, VectorInt3 v2) {
		return new VectorInt3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
	}

	public static VectorInt3 operator -(VectorInt3 v1, VectorInt3 v2) {
		return new VectorInt3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
	}


	public override int GetHashCode() {
		return base.GetHashCode();
	}

	public static bool operator ==(VectorInt3 left, VectorInt3 right) {
		return left.Equals(right);
	}

	public static bool operator !=(VectorInt3 left, VectorInt3 right) {
		return !left.Equals(right);
	}
}
