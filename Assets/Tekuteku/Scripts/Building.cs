using System;
using System.Collections.Generic;
using System.Linq;

public class Building : FieldElement {

	public GridPathFinder PathFinder {
		get;
		set;
	}

	public void CalculatePath() {
		this.PathFinder = new GridPathFinder(this, this.FieldMap.Roads.Count);
	}


	public override void RegisterFieldMap(FieldMap fieldMap, VectorInt3 position) {
		if (position.z != 0) {
			throw new Exception("position.z:" + position.z);
		}

		base.RegisterFieldMap(fieldMap, position);

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

	public override IEnumerable<VectorInt3> ContactedPositions {
		get {
			return this.Positions.SelectMany(pos => this.FieldMap.GetAroundPositions(pos)).Distinct();
		}
	}

}
