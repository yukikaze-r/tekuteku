using System.Collections.Generic;

public class Building : FieldElement {

	public GridPathFinder PathFinder {
		get;
		set;
	}

	public void CalculatePath() {
		this.PathFinder = new GridPathFinder(this, this.FieldMap.Roads.Count);
	}


	public override void RegisterFieldMap(FieldMap fieldMap, VectorInt2 position) {
		base.RegisterFieldMap(fieldMap, position);

		foreach (var lv in fieldMap.GetAroundPositions(position)) {
			FieldElement next = fieldMap.GetFieldElementAt(lv);
			if (next != null && next is Road) {
				this.AddContact(next);
				next.AddContact(this);
			}
		}
	}

}
