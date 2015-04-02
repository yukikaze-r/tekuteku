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

	private int[,] data;

	private List<Road> roads = new List<Road>();
	private Dictionary<VectorInt2, FieldElement> posFieldElement = new Dictionary<VectorInt2, FieldElement>();

	void Start() {

		data = new int[sizeX, sizeZ];
		for (int i = 0; i < data.GetLength(0); i++) {
			for (int j = 0; j < data.GetLength(1); j++) {
				if (i == 0 || i == data.GetLength(0) - 1 || j == 0 || j == data.GetLength(1) - 1) {
					data[i, j] = 1;
				}
				else {
					data[i, j] = 0;
				}
			}
		}
		data[1, 2] = 2;
		data[28, 4] = 3;

		MakeFieldElements();

		for (int i = 0; i < data.GetLength(0); i++) {
			for (int j = 0; j < data.GetLength(1); j++) {
				switch (data[i, j]) {
					case 1:
						CreateBuilding(i,j,roadPrefab);
						break;
					case 2:
						CreateBuilding(i,j,housePrefab);
						break;
					case 3:
						CreateBuilding(i, j, officePrefab);
						break;
				}
			}
		}
	}

	private void CreateBuilding(int x, int y, GameObject prefab) {
		GameObject child = (GameObject)Instantiate(prefab, new Vector3(x, prefab.transform.position.y, y), Quaternion.identity);
		child.transform.parent = gameObject.transform;
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
		if (data[v.x, v.y] == 0) {
			throw new Exception();
		}
		if (data[v.x, v.y] == 1) {
			return MakeRoad(v);
		}
		else {
			return MakeBuilding(v);
		}
	}

	private Road MakeRoad(VectorInt2 v) {
		Road r = new Road(roads.Count);
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


	private Building MakeBuilding(VectorInt2 v) {
		Building r = new Building();
		posFieldElement[v] = r;
		for (int i = 0; i < 4; i++) {
			Direction4 d = (Direction4)i;
			VectorInt2 lv = v;
			lv.Move(d);
			if (lv.IsInboundRect(0, 0, data.GetLength(0), data.GetLength(1)) && data[lv.x, lv.y] == 1) {
				r.AddContact(posFieldElement.ContainsKey(lv) ? (Road)posFieldElement[lv] : MakeRoad(lv));
			}
		}
		return r;
	}
}
