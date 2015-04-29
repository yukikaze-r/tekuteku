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

	public List<Road> Roads {
		get {
			return roads;
		}
	}

	public FieldElement GetFieldElementAt(VectorInt2 pos) {
		FieldElement result;
		if (posFieldElement.TryGetValue(pos, out result)) {
			return result;
		}
		else {
			return null;
		}
	}


	protected void Awake() {
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

	protected void Start() {
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

		ui.Open(ui.toolPalettePrefab).GetComponent<ToolPalette>().ChangeSlectionListener += OnChangeToolSelection;
	}

	public void Build(VectorInt2 pos, FieldElementType type) {

		switch (type) {
			case FieldElementType.HOUSE:
				AppendBuilding(pos, FieldElementType.HOUSE);
				CreateGo(pos, housePrefab);
				break;
			case FieldElementType.OFFICE:
				AppendBuilding(pos, FieldElementType.OFFICE).CalculatePath();
				CreateGo(pos, officePrefab);
				break;
			case FieldElementType.ROAD:
				AppendRoad(pos);
				CreateGo(pos, roadPrefab);
				MakeGridPathFinders();
				break;
		}
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
			if (selected.TrueForAll(e => e is Road)) {
				fieldInfomationPanel = ui.Open(ui.roadPanelPrefab);
				fieldInfomationPanel.GetComponent<RoadPanel>().AcceptModel(selected.Select(e => (Road)e));
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

	private void OnChangeToolSelection() {
		ClearSelectedFieldElements();
	}

	private void CreateGo(VectorInt2 pos, GameObject prefab) {
		GameObject child = (GameObject)Instantiate(prefab, this.GetCenter( pos, prefab.transform.position.y), Quaternion.identity);
		child.transform.parent = gameObject.transform;
		child.GetComponent<FieldElementComponent>().AcceptModel(posFieldElement[pos]);
	}


	public void MakeGridPathFinders() {
		foreach (Building office in offices) {
			office.CalculatePath();
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
		foreach (var lv in v.GetAroundPositions4()) {
			if (lv.IsInboundRect(0, 0, data.GetLength(0), data.GetLength(1)) && data[lv.x, lv.y] != 0) {
				r.AddContact(posFieldElement.ContainsKey(lv) ? posFieldElement[lv] : MakeFieldElement(lv));
			}
		}
		return r;
	}

	private Road AppendRoad(VectorInt2 v) {
		Road r = CreateRoad(v);
		foreach (var lv in v.GetAroundPositions4()) {
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
		foreach (var lv in v.GetAroundPositions4()) {
			if (lv.IsInboundRect(0, 0, data.GetLength(0), data.GetLength(1)) && data[lv.x, lv.y] == FieldElementType.ROAD) {
				r.AddContact(posFieldElement.ContainsKey(lv) ? (Road)posFieldElement[lv] : MakeRoad(lv));
			}
		}
		return r;
	}

	private Building AppendBuilding(VectorInt2 v, FieldElementType type) {
		Building r = CreateBuilding(v, type);
		foreach (var lv in v.GetAroundPositions4()) {
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

}


public enum FieldElementType {
	NONE = 0,
	ROAD = 1,
	HOUSE = 2,
	OFFICE = 3
}