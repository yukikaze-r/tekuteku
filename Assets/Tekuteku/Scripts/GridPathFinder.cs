using System.Collections.Generic;

public class GridPathFinder {
	private int[] distances;

	public GridPathFinder(FieldElement goal, int maxRoads) {
		distances = new int[maxRoads];
		CalculateDistances(goal,1);
	}

	private void CalculateDistances(FieldElement goal, int distance) {
		foreach (var e in goal.Contacts) {
			if (e is Road) {
				var road = (Road)e;
				if (distances[road.Index]==0 || distance < distances[road.Index]) {
					distances[road.Index] = distance;
					CalculateDistances(road, distance + 1);
				}
			}
		}

	}


}
