using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public abstract class FieldElement {

	public FieldMap FieldMap { get; private set; }
	public VectorInt3 Position { get; private set; }
	public virtual int Height {
		get {
			return 1;
		}
	}

	protected List<FieldElement> contacts = new List<FieldElement>();
	
	private HashSet<UnityChanController> vehicles = new HashSet<UnityChanController>();
	public HashSet<UnityChanController> Vehicles {
		get {
			return vehicles;
		}
	}

	public virtual void RegisterFieldMap(FieldMap fieldMap, VectorInt3 position) {
		this.FieldMap = fieldMap;
		this.Position = position;
		foreach (var pos in Positions) {
			fieldMap.PutFieldElement(pos, this);
		}
	}

	public bool IsPuttable(FieldMap fieldMap, VectorInt3 position) {
		foreach (var pos in GetPositions(position)) {
			if (fieldMap.ContainsFieldElement(pos.xy, position.z, this.Height)) {
				return false;
			}
		}
		return true;
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
	
	public virtual IEnumerable<VectorInt3> GetPositions(VectorInt3 org) {
		yield return org;
	}

	public IEnumerable<VectorInt3> Positions {
		get {
			return GetPositions(this.Position);
		}
	}

}
