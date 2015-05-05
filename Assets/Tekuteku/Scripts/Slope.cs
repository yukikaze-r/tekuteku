using System;
using System.Collections.Generic;

public class Slope : Road {

	private Direction4 direction = Direction4.UP;

	public Slope(int roadIndex) : base(roadIndex) {
	}

	public Direction4 Direction {
		get {
			return direction;
		}
		set {
			direction = value;
		}
	}

	protected override void SetPosFieldElement() {
		this.FieldMap.PutFieldElement(this.Position, this);
		this.FieldMap.PutFieldElement(this.Position.GetNext(direction), this);
	}

	public virtual bool IsConnectFrom(FieldElement next) {
		// TODO: 2階部分の接続確認 / 一方通行の処理
		return this.Position.GetNext(direction.Reverse()) == next.Position;
	}

	public virtual bool IsConnectTo(FieldElement next) {
		return this.Position.GetNext(direction.Reverse()) == next.Position;
	}
}
