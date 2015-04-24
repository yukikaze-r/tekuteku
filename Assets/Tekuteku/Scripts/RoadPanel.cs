using UnityEngine;
using System.Collections.Generic;

public class RoadPanel : MonoBehaviour {

	private IEnumerable<Road> roads;

	public void AcceptModel(IEnumerable<Road> roads) {
		this.roads = roads;
	}

	public void ChangeOneWayType() {
		foreach (var road in roads) {
			OneWayType t = road.OneWayType;
			switch (t) {
				case OneWayType.NONE:
					road.OneWayType = OneWayType.DOWN_TOP;
					break;
				case OneWayType.DOWN_TOP:
					road.OneWayType = OneWayType.LEFT_RIGHT;
					break;
				case OneWayType.LEFT_RIGHT:
					road.OneWayType = OneWayType.TOP_DOWN;
					break;
				case OneWayType.TOP_DOWN:
					road.OneWayType = OneWayType.RIGHT_LEFT;
					break;
				case OneWayType.RIGHT_LEFT:
					road.OneWayType = OneWayType.NONE;
					break;
			}
		}
	}


}
