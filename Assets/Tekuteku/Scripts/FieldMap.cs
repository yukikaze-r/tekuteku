using UnityEngine;
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
	public UI ui;


	private FieldElementType[,] data;

	private List<Road> roads = new List<Road>();
	private Dictionary<VectorInt2, FieldElement> posFieldElement = new Dictionary<VectorInt2, FieldElement>();
	private List<Building> offices = new List<Building>();
	private List<FieldElement> selected = new List<FieldElement>();
	private GameObject fieldInfomationPanel;

	public event Action<FieldElement, bool> SelectChangeListener = delegate { };

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
		for (int i = 0; i < 5; i++) {
			for (int j = 0; j < 10; j++) {
				if (i == 0 || i == 5 - 1 || j == 0 || j == 10 - 1) {
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
				VectorInt2 pos = new VectorInt2(i, j);
				switch (data[i, j]) {
					case FieldElementType.ROAD:
						CreateGo(pos, roadPrefab);
						break;
					case FieldElementType.HOUSE:
						CreateGo(pos, housePrefab);
						break;
					case FieldElementType.OFFICE:
						CreateGo(pos, officePrefab);
						break;
				}
			}
		}
	}
	
	void Update () {
		if (Input.GetMouseButtonUp(0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit)) {
				ClickLeft(GetMapPosition(hit.point));
			}

		}
	}

	private void ClickLeft(VectorInt2 pos) {
		var widget = ui.GetWidget(ui.toolPalettePrefab);
		if (widget != null) {
			Tool tool = widget.GetComponent<ToolPalette>().Selected;
			if (tool == Tool.INSPECTOR) {
				DoInspector(pos);
			}
			else {
				DoBuilding(tool, pos);
			}
		}
	}

	private void DoBuilding(Tool tool, VectorInt2 pos) {
		if (posFieldElement.ContainsKey(pos)) {
			return;
		}
		switch (tool) {
			case Tool.HOUSE:
				AppendBuilding(pos, FieldElementType.HOUSE);
				CreateGo(pos, housePrefab);
				break;
			case Tool.OFFICE:
				AppendBuilding(pos, FieldElementType.OFFICE);
				MakeGridPathFinders();
				CreateGo(pos, officePrefab);
				break;
			case Tool.ROAD:
				AppendRoad(pos);
				CreateGo(pos, roadPrefab);
				MakeGridPathFinders();
				break;
		}
	}

	private void DoInspector(VectorInt2 pos) {
		if (fieldInfomationPanel != null) {
			Destroy(fieldInfomationPanel);
		}

		FieldElement element;
		if (posFieldElement.TryGetValue(pos, out element)) {
			if (selected.Contains(element)) {
				selected.Remove(element);
				SelectChangeListener(element, false);
			}
			else {
				selected.Add(element);
				SelectChangeListener(element, true);
			}

			if(selected.Count >= 1) {
				if (selected.TrueForAll(e => e is Road)) {
					fieldInfomationPanel = ui.Open(ui.roadPanelPrefab);
					fieldInfomationPanel.GetComponent<RoadPanel>().AcceptModel(selected.Select(e=>(Road)e));
				}
			}

		}
		else {
			foreach (var e in selected) {
				SelectChangeListener(e, false);
			}
			selected.Clear();
		}
	}

	private void CreateGo(VectorInt2 pos, GameObject prefab) {
		GameObject child = (GameObject)Instantiate(prefab, this.GetCenter( pos, prefab.transform.position.y), Quaternion.identity);
		child.transform.parent = gameObject.transform;
		child.GetComponent<FieldElementComponent>().AcceptModel(posFieldElement[pos]);
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

	private Road CreateRoad(VectorInt2 v) {
		Road r = new Road(roads.Count) { Position = v, FieldMap = this };
		roads.Add(r);
		posFieldElement[v] = r;
		return r;
	}

	private Road MakeRoad(VectorInt2 v) {
		Road r = CreateRoad(v);
		foreach(var lv in GetAroundPositions(v)) {
			if (lv.IsInboundRect(0, 0, data.GetLength(0), data.GetLength(1)) && data[lv.x, lv.y] != 0) {
				r.AddContact(posFieldElement.ContainsKey(lv) ? posFieldElement[lv] : MakeFieldElement(lv));
			}
		}
		return r;
	}

	private Road AppendRoad(VectorInt2 v) {
		Road r = CreateRoad(v);
		foreach (var lv in GetAroundPositions(v)) {
			if (lv.IsInboundRect(0, 0, data.GetLength(0), data.GetLength(1))) {
				FieldElement next;
				if (posFieldElement.TryGetValue(lv, out next)) {
					r.AddContact(next);
					next.AddContact(r);
				}
			}
		}
		return r;
	}

	private Building CreateBuilding(VectorInt2 v, FieldElementType type) {
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
		r.FieldMap = this;

		posFieldElement[v] = r;
		return r;
	}


	private Building MakeBuilding(VectorInt2 v, FieldElementType type) {
		Building r = CreateBuilding(v, type);
		foreach (var lv in GetAroundPositions(v)) {
			if (lv.IsInboundRect(0, 0, data.GetLength(0), data.GetLength(1)) && data[lv.x, lv.y] == FieldElementType.ROAD) {
				r.AddContact(posFieldElement.ContainsKey(lv) ? (Road)posFieldElement[lv] : MakeRoad(lv));
			}
		}
		return r;
	}

	private Building AppendBuilding(VectorInt2 v, FieldElementType type) {
		Building r = CreateBuilding(v, type);
		foreach (var lv in GetAroundPositions(v)) {
			if (lv.IsInboundRect(0, 0, data.GetLength(0), data.GetLength(1))) {
				FieldElement next;
				if (posFieldElement.TryGetValue(lv, out next) && next is Road) {
					r.AddContact(next);
					next.AddContact(r);
				}
			}
		}
		return r;
	}

	public VectorInt2 GetMapPosition(Vector3 v) {
		return new VectorInt2((int)(v.x), (int)(v.z));
	}


	public Vector3 GetCenter(VectorInt2 v, float y) {
		return new Vector3(v.x + 0.5f, y, v.y+0.5f);
	}

	public void PutVehicle(VectorInt2 pos) {
		GameObject child = (GameObject)Instantiate(vehiclePrefab, GetCenter(pos, vehiclePrefab.transform.position.y), Quaternion.identity);
		child.transform.parent = gameObject.transform;
		child.GetComponent<UnityChanController>().FieldMap = this;
	}

	private IEnumerable<VectorInt2> GetAroundPositions(VectorInt2 org) {
		for (int i = 0; i < 4; i++) {
			Direction4 d = (Direction4)i;
			VectorInt2 lv = org;
			lv.Move(d);
			yield return lv;
		}
	}
}
