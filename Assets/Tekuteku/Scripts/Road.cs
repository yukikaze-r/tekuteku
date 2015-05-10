using System;
using System.Collections.Generic;
using System.Linq;

public class Road : FieldElement {

	private int roadIndex;
	private Direction4 oneWayDirection;

	public event Action OneWayTypeChangeListener = delegate { };

	public Road() {
		this.oneWayDirection = 0;
	}


	public override void RegisterFieldMap(FieldMap fieldMap, VectorInt2 position) {
		base.RegisterFieldMap(fieldMap, position);

		roadIndex = fieldMap.Roads.Count();
		fieldMap.Roads.Add(this);

		foreach (var lv in fieldMap.GetAroundPositions(this.Position)) {
			FieldElement next = fieldMap.GetFieldElementAt(lv);
			if (next != null) {
				this.AddContact(next);
				next.AddContact(this);
			}
		}

		fieldMap.MakeGridPathFinders();
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

	public virtual bool IsConnectFrom(FieldElement from) {
		return this.IsOneWay == false || from.Position != this.Position.GetNext(this.OneWayDirection);
	}

	public virtual bool IsConnectTo(FieldElement to) {
		return this.IsOneWay == false || to.Position != this.Position.GetNext(this.OneWayDirection.Reverse());
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
}
