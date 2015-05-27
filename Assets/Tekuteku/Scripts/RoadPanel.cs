using UnityEngine;
using System;
using System.Collections.Generic;

public class RoadPanel : MonoBehaviour {

	private IEnumerable<OneTileRoad> roads;

	public void AcceptModel(IEnumerable<OneTileRoad> roads) {
		this.roads = roads;
	}

	public void ChangeOneWayType() {
		foreach (var road in roads) {
			road.OneWayDirection = GetNextOneWayDirection(road.OneWayDirection);
		}
	}

	private Direction4 GetNextOneWayDirection(Direction4 dir) {
		switch (dir) {
			case Direction4.NONE:
				return Direction4.UP;
			case Direction4.UP:
				return Direction4.RIGHT;
			case Direction4.RIGHT:
				return Direction4.DOWN;
			case Direction4.DOWN:
				return Direction4.LEFT;
			case Direction4.LEFT:
				return Direction4.NONE;
		}
		throw new Exception();
	}
}
