using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public abstract class FieldElement {

	public FieldMap FieldMap { get; private set; }
	public VectorInt2 Position { get; private set; }

	protected List<FieldElement> contacts = new List<FieldElement>();
	
	private HashSet<UnityChanController> vehicles = new HashSet<UnityChanController>();
	public HashSet<UnityChanController> Vehicles {
		get {
			return vehicles;
		}
	}

	public void RegisterFieldMap(FieldMap fieldMap, VectorInt2 position) {
		this.FieldMap = fieldMap;
		this.Position = position;
		SetPosFieldElement();
	}

	public void AddContact(FieldElement r) {
		contacts.Add(r);
	}


	public virtual IEnumerable<FieldElement> ConnectionsFrom {
		get {
			return contacts.OfType<Road>().Where(r => r.IsConnectTo(this)).OfType<FieldElement>();
		}
	}


	public virtual IEnumerable<FieldElement> ConnectionsTo {
		get {
			return contacts.OfType<Road>().Where(r => r.IsConnectFrom(this)).OfType<FieldElement>();
		}
	}

	protected virtual void SetPosFieldElement() {
		this.FieldMap.PutFieldElement(this.Position, this);
	}

}
