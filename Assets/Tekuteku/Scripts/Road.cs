using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Road : FieldElement {

	private int roadIndex;
	private Direction4 oneWayDirection;

	public event Action OneWayTypeChangeListener = delegate { };

	public Road() {
		this.oneWayDirection = 0;
	}


	public override void RegisterFieldMap(FieldMap fieldMap, VectorInt3 position) {
		base.RegisterFieldMap(fieldMap, position);

		roadIndex = fieldMap.Roads.Count();
		fieldMap.Roads.Add(this);

		fieldMap.MakeGridPathFinders();
	}


	public override IEnumerable<VectorInt3> ContactedPositions {
		get {
			return this.FieldMap.GetAroundPositions(this.Position);
		}
	}

	public bool IsContactableTo(FieldElement fieldElement) {
		return this.ContactedPositions.Intersect(fieldElement.Positions).Count() >= 1;
	}

	public int Index {
		get {
			return roadIndex;
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

	public virtual bool IsConnectFrom(FieldElement contacted) {
		return this.IsOneWay == false || contacted.Position.xy != this.Position.xy.GetNext(this.OneWayDirection);
	}

	public virtual bool IsConnectTo(FieldElement contacted) {
		return this.IsOneWay == false || contacted.Position.xy != this.Position.xy.GetNext(this.OneWayDirection.Reverse());
	}

	public override IEnumerable<FieldElement> ConnectionsFrom {
		get {
			return base.ConnectionsFrom.Where(IsConnectFrom);
		}
	}

	public override IEnumerable<FieldElement> ConnectionsTo {
		get {
			return base.ConnectionsFrom.Where(IsConnectTo);
		}
	}

	public override float GetVehicleAltitude(Vector2 pos) {
		return this.Position.z == 0 ? 0.05f : 0.95f;
	}

}
