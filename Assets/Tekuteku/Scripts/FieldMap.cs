﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class FieldMap : MonoBehaviour {


	public int sizeX;
	public int sizeZ;
	public GameObject vehiclePrefab;
	public GameObject roadPrefab;
	public GameObject housePrefab;
	public GameObject officePrefab;
	public GameObject slopePrefab;
	public GameObject overpassPrefab;
	public UI ui;


	private List<Road> roads = new List<Road>();
	private Dictionary<VectorInt2, FieldElement[]> posFieldElements = new Dictionary<VectorInt2, FieldElement[]>();
	private List<Building> offices = new List<Building>();

	private List<FieldElement> selected = new List<FieldElement>();
	private GameObject fieldInfomationPanel;

	public event Action<FieldElement, bool> SelectChangeListener = delegate { };

	public int Width {
		get {
			return sizeX;
		}
	}

	public int Height {
		get {
			return sizeZ;
		}
	}


	public List<Building> Offices {
		get {
			return offices;
		}
	}

	public List<Road> Roads {
		get {
			return roads;
		}
	}

	public FieldElement GetFieldElementAt(VectorInt2 pos, int level) {
		FieldElement [] array;
		if (posFieldElements.TryGetValue(pos, out array)) {
			if (level < array.Count()) {
				return array[level];
			}
		}
		return null;
	}

	public bool ContainsFieldElement(VectorInt2 pos, int level = 0, int height = int.MaxValue) {
		FieldElement[] array;
		if (posFieldElements.TryGetValue(pos, out array)) {
			for (int l = level; l < level + height; l++) {
				if (l >= array.Count()) {
					break;
				}
				if (array[l] != null) {
					return true;
				}
			}
		}
		return false;
	}

	public void PutFieldElement(VectorInt3 pos, FieldElement element) {
		FieldElement [] result;
		if (posFieldElements.TryGetValue(pos.xy, out result)) {
			if (result.Count() <= pos.z) {
				Array.Resize(ref result, pos.z + 1);
				posFieldElements[pos.xy] = result;
			}
		} else {
			result = new FieldElement[pos.z + 1];
			posFieldElements[pos.xy] = result;
		}
		result[pos.z] = element;
	}



	protected void Start() {
	}

	public void Build(VectorInt3 pos, Tool tool, Direction4 direction) {
		FieldElementType type = GetFieldElementTypeFromBuildingTool(tool);
		FieldElement fieldElement = CreateFieldElement(type, direction);
		if (fieldElement.IsPuttable(this, pos) == false) {
			return;
		}

		fieldElement.RegisterFieldMap(this, pos);
		var go = CreateGo(pos.xy, GetPrefab(type, pos.z), direction);
		go.GetComponent<FieldElementComponent>().AcceptModel(fieldElement);
	}

	private FieldElement CreateFieldElement(FieldElementType type, Direction4 direction) {
		switch (type) {
			case FieldElementType.HOUSE:
				return new House();
			case FieldElementType.OFFICE:
				return new Office();
			case FieldElementType.ROAD:
				return new OneTileRoad();
			case FieldElementType.SLOPE:
				return new Slope() { Direction = direction };
		}
		throw new Exception();
	}


	public GameObject GetPrefab(FieldElementType type, int level) {
		switch (type) {
			case FieldElementType.HOUSE:
				return housePrefab;
			case FieldElementType.OFFICE:
				return officePrefab;
			case FieldElementType.ROAD:
				return level == 0 ? roadPrefab : overpassPrefab;
			case FieldElementType.SLOPE:
				return slopePrefab;
		}
		throw new Exception();
	}

	public static FieldElementType GetFieldElementTypeFromBuildingTool(Tool tool) {
		switch (tool) {
			case Tool.HOUSE:
				return FieldElementType.HOUSE;
			case Tool.OFFICE:
				return FieldElementType.OFFICE;
			case Tool.ROAD:
				return FieldElementType.ROAD;
			case Tool.SLOPE:
				return FieldElementType.SLOPE;
		}
		throw new Exception(tool.ToString());

	}

	public void SelectFieldElement(FieldElement element) {
		if (fieldInfomationPanel != null) {
			Destroy(fieldInfomationPanel);
		}

		if (selected.Contains(element)) {
			selected.Remove(element);
			SelectChangeListener(element, false);
		} else {
			selected.Add(element);
			SelectChangeListener(element, true);
		}

		if (selected.Count >= 1) {
			if (selected.TrueForAll(e => e is OneTileRoad)) {
				fieldInfomationPanel = ui.Open(ui.roadPanelPrefab);
				fieldInfomationPanel.GetComponent<RoadPanel>().AcceptModel(selected.Select(e => (OneTileRoad)e));
			}
		}
	}

	public void ClearSelectedFieldElements() {
		if (fieldInfomationPanel != null) {
			Destroy(fieldInfomationPanel);
		}
		foreach (var e in selected) {
			SelectChangeListener(e, false);
		}
		selected.Clear();
	}


	public GameObject CreateGo(VectorInt2 pos, GameObject prefab, Direction4 direction = Direction4.NONE) {
		GameObject child = (GameObject)Instantiate(prefab, this.GetCenter(pos, prefab.transform.position.y), prefab.transform.rotation);
		child.transform.parent = gameObject.transform;
		if (direction != Direction4.NONE) {
			child.transform.rotation = direction.Quaternion();
		}
		return child;
	}


	public void MakeGridPathFinders() {
		foreach (Building office in offices) {
			office.CalculatePath();
		}
	}
	
	public VectorInt2 GetMapPosition(Vector3 v) {
		return new VectorInt2((int)(v.x), (int)(v.z));
	}

	public Vector2 GetFloatMapPosition(Vector3 v) {
		return new Vector2(v.x, v.z);
	}

	public Vector3 GetCenter(VectorInt2 v, float y) {
		return new Vector3(v.x + 0.5f, y, v.y+0.5f);
	}

	public Vector3 GetCenter(VectorInt3 v) {
		return new Vector3(v.x + 0.5f, v.z, v.y + 0.5f);
	}

	public void CreateVehicleGo(VectorInt2 pos) {
		GameObject child = (GameObject)Instantiate(vehiclePrefab, GetCenter(pos, vehiclePrefab.transform.position.y), Quaternion.identity);
		child.transform.parent = gameObject.transform;
		child.GetComponent<MoveUnit>().FieldMap = this;
	}

	public IEnumerable<VectorInt2> GetAroundPositions(VectorInt2 org) {
		foreach (var lv in org.GetAroundPositions4()) {
			if (lv.IsInboundRect(0, 0, this.Width, this.Height)) {
				yield return lv;
			}
		}
	}
	
	public IEnumerable<VectorInt3> GetAroundPositions(VectorInt3 org) {
		foreach (var lv in org.xy.GetAroundPositions4()) {
			if (lv.IsInboundRect(0, 0, this.Width, this.Height)) {
				yield return new VectorInt3(lv.x, lv.y, org.z);
			}
		}
	}
}


public enum FieldElementType {
	NONE,
	ROAD,
	SLOPE,
	HOUSE,
	OFFICE,
}