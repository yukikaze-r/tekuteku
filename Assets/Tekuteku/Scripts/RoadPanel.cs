using UnityEngine;
using System.Collections.Generic;

public class RoadPanel : MonoBehaviour {

	private IEnumerable<Road> roads;

	public void AcceptModel(IEnumerable<Road> roads) {
		this.roads = roads;
	}

	public void ChangeOneWayType() {
		foreach (var road in roads) {
			switch (road.OneWayDirection) {
				case Direction4.NONE:
					road.OneWayDirection = Direction4.UP;
					break;
				case Direction4.UP:
					road.OneWayDirection = Direction4.RIGHT;
					break;
				case Direction4.RIGHT:
					road.OneWayDirection = Direction4.DOWN;
					break;
				case Direction4.DOWN:
					road.OneWayDirection = Direction4.LEFT;
					break;
				case Direction4.LEFT:
					road.OneWayDirection = Direction4.NONE;
					break;
			}
		}
	}


}
