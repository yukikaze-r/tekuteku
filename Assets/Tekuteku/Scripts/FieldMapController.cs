using UnityEngine;
using System;
using System.Collections.Generic;

[@RequireComponent(typeof(FieldMap))]
public class FieldMapController : MonoBehaviour {

	public UI ui;

	private FieldMap fieldMap;
	private ToolPalette toolPalette;

	protected void Start() {
		this.fieldMap = GetComponent<FieldMap>();
		this.toolPalette = ui.GetWidget(ui.toolPalettePrefab).GetComponent<ToolPalette>();
	}

	protected void Update() {
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
		var pos3 = new VectorInt3(pos.x, pos.y, toolPalette.Level);
		if (tool == Tool.INSPECTOR) {
			DoInspector(pos3);
		} else {
			DoBuilding(tool, pos3);
		}
	}

	private void DoBuilding(Tool tool, VectorInt3 pos) {
		switch (tool) {
			case Tool.HOUSE:
				fieldMap.Build(pos, FieldElementType.HOUSE);
				break;
			case Tool.OFFICE:
				fieldMap.Build(pos, FieldElementType.OFFICE);
				break;
			case Tool.ROAD:
				fieldMap.Build(pos, FieldElementType.ROAD);
				break;
			case Tool.SLOPE:
				fieldMap.Build(pos, FieldElementType.SLOPE);
				break;
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
}
