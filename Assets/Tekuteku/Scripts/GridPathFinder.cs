using System;
using System.Collections.Generic;

public class GridPathFinder {
	private int[] distances;

	public GridPathFinder(FieldElement goal, int maxRoads) {
		distances = new int[maxRoads];
		CalculateDistances(goal,1);
	}

	private void CalculateDistances(FieldElement goal, int distance) {
		foreach (var e in goal.Connections) {
			if (e is Road) {
				var road = (Road)e;
				if (distances[road.Index]==0 || distance < distances[road.Index]) {
					distances[road.Index] = distance;
					CalculateDistances(road, distance + 1);
				}
			}
		}
	}

	public Road GetNextRoad(FieldElement at) {
		int min = int.MaxValue;
		Road result = null;
		foreach (var e in at.Connections) {
			if (e is Road) {
				Road road = (Road)e;
				int d = distances[road.Index];
				if (d < min) {
					result = road;
					min = d;
				}
			}
		}
		return result;
	}


}
