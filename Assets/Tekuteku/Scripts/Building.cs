using System;
using System.Collections.Generic;

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

		foreach (var lv in fieldMap.GetAroundPositions(position.xy)) {
			FieldElement next = fieldMap.GetFieldElementAt(lv,0);
			if (next != null && next is Road) {
				this.AddContact(next);
				next.AddContact(this);
			}
		}
	}

}
