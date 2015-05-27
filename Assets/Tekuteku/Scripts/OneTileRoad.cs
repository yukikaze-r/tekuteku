using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class OneTileRoad : Road {
	private Direction4 oneWayDirection = Direction4.NONE;

	public event Action OneWayTypeChangeListener = delegate { };

	public override IEnumerable<VectorInt3> ContactedPositions {
		get {
			return this.FieldMap.GetAroundPositions(this.Position);
		}
	}

	public Direction4 OneWayDirection {
		get {
			return oneWayDirection;
		}
		set {
			oneWayDirection = value;


			this.FieldMap.MakeGridPathFinders();

			OneWayTypeChangeListener();
		}
	}

	public bool IsOneWay {
		get {
			return this.OneWayDirection != Direction4.NONE;
		}
	}

	public override bool IsConnectFrom(FieldElement contacted) {
		return this.IsOneWay == false || contacted.Position.xy != this.Position.xy.GetNext(this.OneWayDirection);
	}

	public override bool IsConnectTo(FieldElement contacted) {
		return this.IsOneWay == false || contacted.Position.xy != this.Position.xy.GetNext(this.OneWayDirection.Reverse());
	}

	public override float GetVehicleAltitude(Vector2 pos) {
		return this.Position.z == 0 ? 0.05f : 0.95f;
	}
}
