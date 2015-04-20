using System.Collections.Generic;

public class Road : FieldElement {

	private int roadIndex;


	public Road(int roadIndex) {
		this.roadIndex = roadIndex;
	}


	public int Index {
		get {
			return roadIndex;
		}
	}

}
