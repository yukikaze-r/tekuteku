using System;
using System.Collections.Generic;

public class Road : FieldElement {

	private int roadIndex;
	private OneWayType oneWayType;

	public event Action OneWayTypeChangeListener = delegate { };

	public Road(int roadIndex) {
		this.roadIndex = roadIndex;
		this.OneWayType = OneWayType.NONE;
	}

	public int Index {
		get {
			return roadIndex;
		}
	}

	public OneWayType OneWayType {
		get {
			return oneWayType;
		}
		set {
			oneWayType = value;
			OneWayTypeChangeListener();
		}
	}

}


public enum OneWayType {
	NONE, DOWN_TOP, TOP_DOWN, RIGHT_LEFT, LEFT_RIGHT
}

