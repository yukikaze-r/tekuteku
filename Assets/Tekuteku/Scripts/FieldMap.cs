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
	public GameObject slopePrefab;
	public UI ui;


	private List<Road> roads = new List<Road>();
	private Dictionary<VectorInt2, FieldElement> posFieldElement = new Dictionary<VectorInt2, FieldElement>();
	private List<Building> offices = new List<Building>();

	
	private List<FieldElement> selected = new List<FieldElement>();
	private GameObject fieldInfomationPanel;
	private GameObject cursor = null;
	private ToolPalette toolPalette;

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

	public FieldElement GetFieldElementAt(VectorInt2 pos) {
		FieldElement result;
		if (posFieldElement.TryGetValue(pos, out result)) {
			return result;
		}
		else {
			return null;
		}
	}

	public void PutFieldElement(VectorInt2 pos, FieldElement element) {
		posFieldElement[pos] = element;
	}



	protected void Start() {
		this.toolPalette = ui.Open(ui.toolPalettePrefab).GetComponent<ToolPalette>();
		toolPalette.ChangeSlectionListener += OnChangeToolSelection;
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
			case FieldElementType.SLOPE:
				AppendSlope(pos);
				CreateGo(pos, slopePrefab);
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
		if (toolPalette.Selected == Tool.SLOPE) {
			Debug.Log("OnChangeToolSelection toolPalette.Selected == Tool.SLOPE");
			VectorInt2 pos;
			if (GetCursorMapPosition(out pos)) {
				cursor = CreateGo(pos, slopePrefab);
			} else {
				cursor = CreateGo(pos, slopePrefab);
				cursor.SetActive(false);
			}
			cursor.GetComponent<FieldElementComponent>().MakeCursor();
		} else {
			Debug.Log("OnChangeToolSelection "+cursor);
			if (cursor != null) {
				Debug.Log("Destroy(cursor); " + cursor);
				Destroy(cursor);
				cursor = null;
			}
		}
	}

	protected void Update() {
		if (cursor != null) {
			VectorInt2 pos;
			if (GetCursorMapPosition(out pos)) {
				var p = cursor.transform.position;
				cursor.transform.position = this.GetCenter(pos, p.y);
				cursor.SetActive(true);
			} else {
				cursor.SetActive(false);
			}
		}
	}

	private bool GetCursorMapPosition(out VectorInt2 pos) {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit)) {
			pos = GetMapPosition(hit.point);
			return true;
		} else {
			pos = new VectorInt2();
			return false;
		}
	}

	private GameObject CreateGo(VectorInt2 pos, GameObject prefab) {
		GameObject child = (GameObject)Instantiate(prefab, this.GetCenter(pos, prefab.transform.position.y), prefab.transform.rotation);
		child.transform.parent = gameObject.transform;
		if (posFieldElement.ContainsKey(pos)) {
			child.GetComponent<FieldElementComponent>().AcceptModel(posFieldElement[pos]);
		}
		return child;
	}


	public void MakeGridPathFinders() {
		foreach (Building office in offices) {
			office.CalculatePath();
		}
	}


	private Road CreateRoad(VectorInt2 v) {
		Road r = new Road(roads.Count);
		r.RegisterFieldMap(this, v);
		roads.Add(r);
		return r;
	}

	private Road AppendRoad(VectorInt2 v) {
		Road r = CreateRoad(v);
		foreach (var lv in GetAroundPositions(v)) {
			FieldElement next;
			if (posFieldElement.TryGetValue(lv, out next)) {
				r.AddContact(next);
				next.AddContact(r);
			}
		}
		return r;
	}

	private Slope CreateSlope(VectorInt2 v) {
		Slope r = new Slope(roads.Count);
		r.RegisterFieldMap(this, v);
		roads.Add(r);
		return r;
	}

	private Slope AppendSlope(VectorInt2 v) {
		Slope r = CreateSlope(v);
		foreach (var lv in GetAroundPositions(v)) {
			// TODO AddContact
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
		r.RegisterFieldMap(this, v);
		return r;
	}


	private Building AppendBuilding(VectorInt2 v, FieldElementType type) {
		Building r = CreateBuilding(v, type);
		foreach (var lv in GetAroundPositions(v)) {
			FieldElement next;
			if (posFieldElement.TryGetValue(lv, out next) && next is Road) {
				r.AddContact(next);
				next.AddContact(r);
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
		foreach (var lv in org.GetAroundPositions4()) {
			if (lv.IsInboundRect(0, 0, this.Width, this.Height)) {
				yield return lv;
			}
		}
	}
}


public enum FieldElementType {
	NONE = 0,
	ROAD = 1,
	HOUSE = 2,
	OFFICE = 3,
	SLOPE = 4,
}