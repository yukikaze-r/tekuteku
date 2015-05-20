using System;
using System.Collections.Generic;
using UnityEngine;

public class Slope : Road {

	private Direction4 direction = Direction4.DOWN;

	public Slope() {
	}

	public override void RegisterFieldMap(FieldMap fieldMap, VectorInt3 position) {
		if (position.z != 0) {
			throw new Exception("position.z: "+position.z);
		}

		base.RegisterFieldMap(fieldMap, position);
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

	public override IEnumerable<VectorInt3> ContactedPositions {
		get {
			yield return this.Position.xy.GetNext(direction.Reverse()).z(this.Position.z);
			yield return this.Position.xy.GetNext(direction, 2).z(this.Position.z + 1);
		}
	}

	public override bool IsConnectFrom(FieldElement contacted) {
		return true;
	}

	public override bool IsConnectTo(FieldElement contacted) {
		return true;
	}

	public override float GetVehicleAltitude(Vector2 pos) {
		RaycastHit hit;
		if (Physics.Raycast(new Vector3(pos.x, 1000f, pos.y), new Vector3(0f, -1f, 0f), out hit, Mathf.Infinity)) {
			return hit.point.y;
		}
		else {
			return base.GetVehicleAltitude(pos);
		}
	}
}
