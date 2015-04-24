using System;
using System.Collections.Generic;

public class Road : FieldElement {

	private int roadIndex;
	private Direction4 oneWayDirection;

	public event Action OneWayTypeChangeListener = delegate { };

	public Road(int roadIndex) {
		this.roadIndex = roadIndex;
		this.oneWayDirection = 0;
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
		
	public override IEnumerable<FieldElement> ConnectionsFrom {
		get {
			foreach (var e in base.ConnectionsFrom) {
				if (this.OneWayDirection != Direction4.NONE) {
					if (e.Position == this.Position.GetNext(this.OneWayDirection)) {
						continue;
					}
				}
				yield return e;
			}
		}
	}


	public override IEnumerable<FieldElement> ConnectionsTo {
		get {
			foreach (var e in base.ConnectionsTo) {
				if (this.OneWayDirection != Direction4.NONE) {
					if (e.Position == this.Position.GetNext(this.OneWayDirection.Reverse())) {
						continue;
					}
				}
				yield return e;
			}
		}
	}

}
