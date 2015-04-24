using UnityEngine;
using System.Collections.Generic;

public abstract class FieldElement {

	public FieldMap FieldMap { get; set; }
	public VectorInt2 Position { get; set; }

	protected List<FieldElement> contacts = new List<FieldElement>();
	
	private HashSet<UnityChanController> vehicles = new HashSet<UnityChanController>();
	public HashSet<UnityChanController> Vehicles {
		get {
			return vehicles;
		}
	}

	public void AddContact(FieldElement r) {
		contacts.Add(r);
	}


	public virtual IEnumerable<FieldElement> ConnectionsFrom {
		get {
			foreach (var e in contacts) {
				if (e is Road) {
					Road r = (Road)e;
					if (r.OneWayDirection != Direction4.NONE) {
						if (r.Position.GetNext(r.OneWayDirection.Reverse()) == this.Position) {
							continue;
						}
					}
				}
				yield return e;
			}
		}
	}


	public virtual IEnumerable<FieldElement> ConnectionsTo {
		get {
			foreach (var e in contacts) {
				if (e is Road) {
					Road r = (Road)e;
					if (r.OneWayDirection != Direction4.NONE) {
						if (r.Position.GetNext(r.OneWayDirection) == this.Position) {
							continue;
						}
					}
				}
				yield return e;
			}
		}
	}
}
