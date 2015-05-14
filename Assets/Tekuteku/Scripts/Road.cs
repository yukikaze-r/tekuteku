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


	public override void RegisterFieldMap(FieldMap fieldMap, VectorInt3 position) {
		base.RegisterFieldMap(fieldMap, position);

		roadIndex = fieldMap.Roads.Count();
		fieldMap.Roads.Add(this);

		InitializeContacts();

		fieldMap.MakeGridPathFinders();
	}

	protected virtual void InitializeContacts() {
		AddContactsArround(this.Position.xy, this.Position.z);
	}

	protected void AddContactsArround(VectorInt2 pos, int level) {
		foreach (var lv in this.FieldMap.GetAroundPositions(pos)) {
			FieldElement next = this.FieldMap.GetFieldElementAt(lv, level);
			if (next != null) {
				this.AddContact(next);
				next.AddContact(this);
			}
		}

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
}
