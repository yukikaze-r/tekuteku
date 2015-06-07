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
	
	private HashSet<MoveUnit> moveUnits = new HashSet<MoveUnit>();
	public HashSet<MoveUnit> MoveUnits {
		get {
			return moveUnits;
		}
	}

	public virtual void RegisterFieldMap(FieldMap fieldMap, VectorInt3 position) {
		this.FieldMap = fieldMap;
		this.Position = position;
		foreach (var pos in this.Positions) {
			fieldMap.PutFieldElement(pos, this);
		}
		foreach (var contactedRoad in GetContactedRoads()) {
			this.AddContact(contactedRoad);
			contactedRoad.AddContact(this);
		}
	}

	private IEnumerable<Road> GetContactedRoads() {
		return this.ContactedPositions
			.Select(pos => this.FieldMap.GetFieldElementAt(pos.xy, pos.z))
			.Where(e => e != null).Distinct().OfType<Road>()
			.Where(r => r.IsContactableTo(this));
	}

	public bool IsPuttable(FieldMap fieldMap, VectorInt3 position) {
		foreach (var pos in GetPositions(position)) {
			if (fieldMap.ContainsFieldElement(pos.xy, position.z, this.Height)) {
				return false;
			}
		}
		return true;
	}

	public abstract IEnumerable<VectorInt3> ContactedPositions {
		get;
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

	public virtual  float GetVehicleAltitude(Vector2 pos) {
		return 0.05f;
	}

	public bool ContainsPosition(VectorInt2 p) {
		foreach (var pos in this.Positions) {
			if (p.x == pos.x && p.y == pos.y) {
				return true;
			}
		}
		return false;
	}

	public bool ContainsPosition(Vector3 position) {
		return ContainsPosition(this.FieldMap.GetMapPosition(position));
	}

	public virtual bool CanEnter(MoveUnit moveUnit, out MoveUnit blockedBy) {
		blockedBy = null;
		if (this.MoveUnits.Count() == 0) {
			return true;
		}
		FieldElement nextNext = moveUnit.GetNextFieldElement(moveUnit.NextFieldElement);
		foreach (var vehicle in this.MoveUnits) {
			if (vehicle.NextFieldElement == nextNext) {
				blockedBy = vehicle;
				return false;
			}
		}
		return true;
	}

	public virtual bool CanMove(MoveUnit moveUnit, out MoveUnit blockedBy) {
		return moveUnit.NextFieldElement.CanEnter(moveUnit, out blockedBy);
	}

	private IEnumerable<VectorInt3> GetMyPositionFromConnectedPosition(VectorInt3 connectedPosition) {
		foreach(var p in this.Positions) {
			if(this.FieldMap.GetAroundPositions(p).Contains(connectedPosition)) {
				yield return p;
			}
		}
	}

	public IEnumerable<Vector3> GetConnectionPoints(FieldElement from) {
		foreach (var connectedPosition in this.ContactedPositions) {
			var fieldElement = this.FieldMap.GetFieldElementAt(connectedPosition.xy, connectedPosition.z);
			if (fieldElement != null) {
				foreach(var p in GetMyPositionFromConnectedPosition(connectedPosition) ){
					yield return (this.FieldMap.GetCenter(p) + this.FieldMap.GetCenter(connectedPosition)) / 2;
				}
			}
		}
	}

}
