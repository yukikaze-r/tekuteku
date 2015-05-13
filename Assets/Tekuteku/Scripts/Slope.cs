using System;
using System.Collections.Generic;

public class Slope : Road {

	private Direction4 direction = Direction4.DOWN;

	public Slope() {
	}

	public Direction4 Direction {
		get {
			return direction;
		}
		set {
			direction = value;
		}
	}

	public override IEnumerable<VectorInt2> GetPositions(VectorInt2 org) {
		yield return org;
		yield return org.GetNext(direction);
	}

	public override bool IsConnectFrom(FieldElement next) {
		// TODO: 2階部分の接続確認 / 一方通行の処理
		return this.Position.GetNext(direction.Reverse()) == next.Position;
	}

	public override bool IsConnectTo(FieldElement next) {
		return this.Position.GetNext(direction.Reverse()) == next.Position;
	}
}
