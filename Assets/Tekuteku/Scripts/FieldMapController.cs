using UnityEngine;
using System;
using System.Collections.Generic;

[@RequireComponent(typeof(FieldMap))]
public class FieldMapController : MonoBehaviour {

	public UI ui;

	private FieldMap fieldMap;
	private ToolPalette toolPalette;

	private GameObject cursor = null;
	private Direction4 cursorDirection = Direction4.NONE;

	protected void Start() {
		this.fieldMap = GetComponent<FieldMap>();
		this.toolPalette = ui.Open(ui.toolPalettePrefab).GetComponent<ToolPalette>();

		toolPalette.ChangeSlectionListener += OnChangeToolSelection;
	}

	protected void Update() {
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
				ClickLeft(fieldMap.GetMapPosition(hit.point));
			}
		}
	}

	private void ClickLeft(VectorInt2 pos) {
		Tool tool = toolPalette.Selected;
		var pos3 = pos.z(toolPalette.Level);
		if (tool == Tool.INSPECTOR) {
			DoInspector(pos3);
		} else {
			fieldMap.Build(pos3, tool, cursorDirection);
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
			Destroy(cursor);
			cursor = null;
			cursorDirection = Direction4.NONE;
		}

		if (toolPalette.Selected != Tool.INSPECTOR) {
			VectorInt2 pos;
			GameObject prefab = fieldMap.GetPrefab(FieldMap.GetFieldElementTypeFromBuildingTool(toolPalette.Selected), toolPalette.Level);
			if (GetCursorMapPosition(out pos)) {
				cursor = fieldMap.CreateGo(pos, prefab);
			} else {
				cursor = fieldMap.CreateGo(pos, prefab);
				cursor.SetActive(false);
			}
			cursor.GetComponent<FieldElementComponent>().MakeCursor();
			cursorDirection = Direction4.UP;
		}
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
}
