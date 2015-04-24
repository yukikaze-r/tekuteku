using System.Collections.Generic;

public class Building : FieldElement {

	public GridPathFinder PathFinder {
		get;
		set;
	}

	public void CalculatePath() {
		this.PathFinder = new GridPathFinder(this, this.FieldMap.Roads.Count);
	}

}
