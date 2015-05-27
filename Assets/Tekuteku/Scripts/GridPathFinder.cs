using System;
using System.Collections.Generic;

public class GridPathFinder {
	private int[] distances;

	public GridPathFinder(FieldElement goal, int maxRoads) {
		distances = new int[maxRoads];
		CalculateDistances(goal,1);
	}

	private void CalculateDistances(FieldElement goal, int distance) {
		RoadIndexDistanceList list = new RoadIndexDistanceList();
		foreach (var e in goal.ConnectionsFrom) {
			if (e is Road) {
				Road r = (Road)e;
				list.Add(r, distance);
				distances[r.Index] = distance;
			}
		}
		CalculateDistances(list);
	}

	private void CalculateDistances(RoadIndexDistanceList list) {
		Road road;
		int distance;
		while (list.PopLowestDistance(out road, out distance)) {
			if (distances[road.Index] >= distance) {
				int newDistance = distance + road.Cost;
				foreach (var e in road.ConnectionsFrom) {
					if (e is Road) {
						Road r = (Road)e;
						int current = distances[r.Index];
						if (current == 0 || current > newDistance) {
							list.Add(r, newDistance);
							distances[r.Index] = newDistance;
						}
					}
				}
			}
		}
	}

	public Road GetNextRoad(FieldElement at) {
		int min = int.MaxValue;
		Road result = null;
		foreach (var e in at.ConnectionsTo) {
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


	protected class RoadIndexDistanceList {

		private int minDistance = 0;
		private int count = 0;
		private Stack<Road>[] distanceRoads = new Stack<Road>[128];

		public void Add(Road road, int distance) {
			if (distance > distanceRoads.Length) {
				Array.Resize(ref distanceRoads, distanceRoads.Length * 2);
			}
			Stack<Road> roads = distanceRoads[distance];
			if (roads == null) {
				distanceRoads[distance] = roads = new Stack<Road>();
			}
			roads.Push(road);
			count++;
			minDistance = Math.Min(minDistance, distance);
		}

		public bool PopLowestDistance(out Road road, out int distance) {
			if (count == 0) {
				road = null;
				distance = 0;
				return false;
			}

			while (true) {
				Stack<Road> roads = distanceRoads[minDistance];
				if (roads != null && roads.Count >= 1) {
					road = roads.Pop();
					distance = minDistance;
					count--;
					return true;
				}
				minDistance++;
			}
		}
	}
}
