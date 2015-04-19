using UnityEngine;
using System.Collections.Generic;

public abstract class FieldElement {

	public FieldMap FieldMap { get; set; }
	public VectorInt2 Position { get; set; }

	private HashSet<UnityChanController> vehicles = new HashSet<UnityChanController>();
	public HashSet<UnityChanController> Vehicles {
		get {
			return vehicles;
		}
	}

	abstract public IEnumerable<FieldElement> Connections {
		get;
	}
}
