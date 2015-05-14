using System;
using System.Collections.Generic;

public class Slope : Road {

	private Direction4 direction = Direction4.DOWN;

	public Slope() {
	}

	public override int Height {
		get {
			return 2;
		}
	}

	public Direction4 Direction {
		get {
			return direction;
		}
		set {
			direction = value;
		}
	}

	public override IEnumerable<VectorInt3> GetPositions(VectorInt3 org) {
		yield return org;
		var next = org.xy.GetNext(direction);
		yield return new VectorInt3(next.x, next.y, org.z + 1);
	}

	protected override void InitializeContacts() {
		AddContactsArround(this.Position.xy, this.Position.z);
		AddContactsArround(this.Position.xy.GetNext(direction), this.Position.z + 1);
	}

	public override bool IsConnectFrom(FieldElement contacted) {
		// TODO: 一方通行の処理
		return true;
	}

	public override bool IsConnectTo(FieldElement contacted) {
		return true;
	}
}
