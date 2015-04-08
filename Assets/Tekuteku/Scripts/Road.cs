using System.Collections.Generic;

public class Road : FieldElement {

	private int roadIndex;
	private FieldElement[] next = new FieldElement[4];

	public Road(int roadIndex) {
		this.roadIndex = roadIndex;
	}

	public void PutNext(Direction4 d, FieldElement r) {
		next[(int)d] = r;
	}

	public int Index {
		get {
			return roadIndex;
		}
	}

	override public IEnumerable<FieldElement> Connections {
		get {
			return next;
		}
	}




}
