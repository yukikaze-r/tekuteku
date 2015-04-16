using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class FieldMap : MonoBehaviour {

	public GameObject roadPrefab;
	public GameObject housePrefab;
	public GameObject officePrefab;
	public int sizeX;
	public int sizeZ;

	private FieldElementType[,] data;

	private List<Road> roads = new List<Road>();
	private Dictionary<VectorInt2, FieldElement> posFieldElement = new Dictionary<VectorInt2, FieldElement>();
	private List<Building> offices = new List<Building>();

	public List<Building> Offices {
		get {
			return offices;
		}
	}
	
	public FieldElement GetFieldElementAt(VectorInt2 pos) {
		return posFieldElement[pos];
	}

	enum FieldElementType {
		NONE = 0,
		ROAD = 1,
		HOUSE = 2,
		OFFICE = 3
	}

	void Awake() {
		data = new FieldElementType[sizeX, sizeZ];
		for (int i = 0; i < data.GetLength(0); i++) {
			for (int j = 0; j < data.GetLength(1); j++) {
				if (i == 0 || i == data.GetLength(0) - 1 || j == 0 || j == data.GetLength(1) - 1) {
					data[i, j] = FieldElementType.ROAD;
				}
				else {
					data[i, j] = FieldElementType.NONE;
				}
			}
		}
		data[1, 2] = FieldElementType.HOUSE;
		data[3, 4] = FieldElementType.OFFICE;

		MakeFieldElements();
		MakeGridPathFinders();
	}

	void Start() {
		for (int i = 0; i < data.GetLength(0); i++) {
			for (int j = 0; j < data.GetLength(1); j++) {
				switch (data[i, j]) {
					case FieldElementType.ROAD:
						CreateBuilding(i,j,roadPrefab);
						break;
					case FieldElementType.HOUSE:
						CreateBuilding(i,j,housePrefab);
						break;
					case FieldElementType.OFFICE:
						CreateBuilding(i, j, officePrefab);
						break;
				}
			}
		}
	}

	private void CreateBuilding(int x, int y, GameObject prefab) {
		GameObject child = (GameObject)Instantiate(prefab, new Vector3(x, prefab.transform.position.y, y), Quaternion.identity);
		child.transform.parent = gameObject.transform;
		child.AddComponent<FieldElementComponent>().AcceptModel(posFieldElement[new VectorInt2(x,y)]);
	}


	private void MakeGridPathFinders() {
		foreach (Building office in offices) {
			office.PathFinder = new GridPathFinder(office, roads.Count);
		}
	}

	private void MakeFieldElements() {
		for (int i = 0; i < data.GetLength(0); i++) {
			for (int j = 0; j < data.GetLength(1); j++) {
				VectorInt2 v = new VectorInt2(i, j);
				if (data[i, j] != 0 && !posFieldElement.ContainsKey(v)) {
					MakeFieldElement(v);
				}
			}
		}
	}

	private FieldElement MakeFieldElement(VectorInt2 v) {
		if (data[v.x, v.y] == FieldElementType.NONE) {
			throw new Exception();
		}
		if (data[v.x, v.y] == FieldElementType.ROAD) {
			return MakeRoad(v);
		}
		else {
			return MakeBuilding(v, data[v.x, v.y]);
		}
	}

	private Road MakeRoad(VectorInt2 v) {
		Road r = new Road(roads.Count);
		r.Position = v;
		roads.Add(r);
		posFieldElement[v] = r;
		for (int i = 0; i < 4; i++) {
			Direction4 d = (Direction4)i;
			VectorInt2 lv = v;
			lv.Move(d);
			if (lv.IsInboundRect(0, 0, data.GetLength(0), data.GetLength(1)) && data[lv.x, lv.y] != 0) {
				r.PutNext(d, posFieldElement.ContainsKey(lv) ? posFieldElement[lv] : MakeFieldElement(lv));
			}
		}
		return r;
	}


	private Building MakeBuilding(VectorInt2 v, FieldElementType type) {
		Building r;
		switch (type) {
			case FieldElementType.OFFICE:
				r = new Building();
				offices.Add(r);
				break;
			case FieldElementType.HOUSE:
				r = new House();
				break;
			default:
				throw new Exception();
		}
		r.Position = v;

		posFieldElement[v] = r;
		for (int i = 0; i < 4; i++) {
			Direction4 d = (Direction4)i;
			VectorInt2 lv = v;
			lv.Move(d);
			if (lv.IsInboundRect(0, 0, data.GetLength(0), data.GetLength(1)) && data[lv.x, lv.y] == FieldElementType.ROAD) {
				r.AddContact(posFieldElement.ContainsKey(lv) ? (Road)posFieldElement[lv] : MakeRoad(lv));
			}
		}
		return r;
	}
}
