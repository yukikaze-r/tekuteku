using UnityEngine;
using System.Collections;

public class RoadPanel : MonoBehaviour {

	private Road road;

	public void AcceptModel(Road road) {
		this.road = road;
	}

	public void ChangeOneWayType() {
		OneWayType t = road.OneWayType;
		switch (t) {
			case OneWayType.NONE:
				road.OneWayType = OneWayType.DOWN_TOP;
				break;
			case OneWayType.DOWN_TOP:
				road.OneWayType = OneWayType.TOP_DOWN;
				break;
			case OneWayType.TOP_DOWN:
				road.OneWayType = OneWayType.LEFT_RIGHT;
				break;
			case OneWayType.LEFT_RIGHT:
				road.OneWayType = OneWayType.RIGHT_LEFT;
				break;
			case OneWayType.RIGHT_LEFT:
				road.OneWayType = OneWayType.DOWN_TOP;
				break;
		}


	}


}
