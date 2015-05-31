using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[@RequireComponent(typeof(FieldMap))]
public class FieldMapController : MonoBehaviour {

	public UI ui;

	private FieldMap fieldMap;
	private ToolPalette toolPalette;

	private VectorInt2 startRangeCursorPosition;
	private List<GameObject> rangeCursorGos = new List<GameObject>();
	private bool isRangeCursorMode = false;
	private GameObject cursor = null;
	private Direction4 cursorDirection = Direction4.NONE;

	protected void Start() {
		this.fieldMap = GetComponent<FieldMap>();
		this.toolPalette = ui.Open(ui.toolPalettePrefab).GetComponent<ToolPalette>();

		toolPalette.ChangeSlectionListener += OnChangeToolSelection;
	}

	protected void Update() {
		if (isRangeCursorMode) {
			foreach (var go in rangeCursorGos) {
				Destroy(go);
			}
			rangeCursorGos.Clear();

			Vector2 pos;
			if(GetCursorMapPosition(out pos)) {
				foreach (var p in GetRangeCursorPositions(startRangeCursorPosition, pos)) {
					rangeCursorGos.Add(CreateCursorGo(p));
				}
			}
		}

		if (cursor != null) {
			VectorInt2 pos;
			if (GetCursorMapPosition(out pos)) {
				var p = cursor.transform.position;
				cursor.transform.position = fieldMap.GetCenter(pos, p.y);
				cursor.SetActive(true);
			} else {
				cursor.SetActive(false);
			}

			if (Input.GetButtonUp("Rotate Cursor Direction")) {
				cursor.transform.Rotate(Vector3.up, 90f);
				cursorDirection = cursorDirection.Rotate();
			}
		}

		if (InputSystem.main.GetMouseButtonUp(0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit)) {
				ClickLeft(fieldMap.GetMapPosition(hit.point), fieldMap.GetFloatMapPosition(hit.point));
			}
		}
	}

	private void ClickLeft(VectorInt2 pos, Vector2 floatPos) {
		Tool tool = toolPalette.Selected;
		var pos3 = pos.z(toolPalette.Level);
		if (tool == Tool.INSPECTOR) {
			DoInspector(pos3);
		} else if(tool == Tool.ROAD) {
			if (isRangeCursorMode == false) {
				isRangeCursorMode = true;
				startRangeCursorPosition = pos;
				DestroyCursor();
			} else {
				foreach (var rangePos in GetRangeCursorPositions(startRangeCursorPosition, floatPos)) {
					fieldMap.Build(rangePos.z(toolPalette.Level), tool, cursorDirection);
				}
				foreach (var go in rangeCursorGos) {
					Destroy(go);
				}
				rangeCursorGos.Clear();
				CreateCursor();
				isRangeCursorMode = false;
			}
		} else {
			fieldMap.Build(pos3, tool, cursorDirection);
		}
	}


	private IEnumerable<VectorInt2> GetRangeCursorPositions(VectorInt2 startPos, Vector2 end) {
		float xInEnd = end.x - (int)end.x;
		float yInEnd = end.y - (int)end.y;

		int sx = startPos.x;
		int sy = startPos.y;
		int ex = (int)end.x;
		int ey = (int)end.y;

		Func<int, Func<int, VectorInt2>> funcX = c => i => new VectorInt2(i, c);
		Func<int, Func<int, VectorInt2>> funcY = c => i => new VectorInt2(c, i);

		if (yInEnd < xInEnd) {
			return GetPositions(sx, ex, 0, funcX(sy)).Concat(GetPositions(sy, ey, 1, funcY(ex)));
		} else {
			return GetPositions(sy, ey, 0, funcY(sx)).Concat(GetPositions(sx, ex, 1, funcX(ey)));
		}
	}

	private static IEnumerable<VectorInt2> GetPositions(int s, int e, int skip, Func<int,VectorInt2> f) {
		if (s < e) {
			for (int i = s + skip; i <= e; i++) {
				yield return f(i);
			}
		} else {
			for (int i = s - skip; i >= e; i--) {
				yield return f(i);
			}
		}
	}


	private void DoInspector(VectorInt3 pos) {

		FieldElement element = fieldMap.GetFieldElementAt(pos.xy, pos.z);
		if (element != null) {
			fieldMap.SelectFieldElement(element);
		} else {
			fieldMap.ClearSelectedFieldElements();

		}
	}


	private void OnChangeToolSelection() {
		fieldMap.ClearSelectedFieldElements();

		if (cursor != null) {
			DestroyCursor();
		}

		if (toolPalette.Selected != Tool.INSPECTOR) {
			CreateCursor();
		}
	}


	private void CreateCursor() {
		VectorInt2 pos;
		if (GetCursorMapPosition(out pos)) {
			cursor = CreateCursorGo(pos);
		} else {
			cursor = CreateCursorGo(pos);
			cursor.SetActive(false);
		}
		cursorDirection = Direction4.UP;
	}

	private GameObject CreateCursorGo(VectorInt2 pos) {
		GameObject prefab = fieldMap.GetPrefab(FieldMap.GetFieldElementTypeFromBuildingTool(toolPalette.Selected), toolPalette.Level);
		GameObject result = fieldMap.CreateGo(pos, prefab);
		result.GetComponent<FieldElementComponent>().MakeCursor();
		return result;
	}

	private void DestroyCursor() {
		Destroy(cursor);
		cursor = null;
		cursorDirection = Direction4.NONE;
	}
	
	private bool GetCursorMapPosition(out VectorInt2 pos) {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit)) {
			pos = fieldMap.GetMapPosition(hit.point);
			return true;
		} else {
			pos = new VectorInt2();
			return false;
		}
	}


	private bool GetCursorMapPosition(out Vector2 pos) {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit)) {
			pos = fieldMap.GetFloatMapPosition(hit.point);
			return true;
		} else {
			pos = new Vector2();
			return false;
		}
	}
}
