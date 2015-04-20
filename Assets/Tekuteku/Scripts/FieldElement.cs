using UnityEngine;
using System.Collections.Generic;

public abstract class FieldElement {

	public FieldMap FieldMap { get; set; }
	public VectorInt2 Position { get; set; }

	private List<FieldElement> contacts = new List<FieldElement>(); 
	private HashSet<UnityChanController> vehicles = new HashSet<UnityChanController>();
	public HashSet<UnityChanController> Vehicles {
		get {
			return vehicles;
		}
	}

	public void AddContact(FieldElement r) {
		contacts.Add(r);
	}


	public IEnumerable<FieldElement> Connections {
		get {
			return contacts;
		}
	}
}
