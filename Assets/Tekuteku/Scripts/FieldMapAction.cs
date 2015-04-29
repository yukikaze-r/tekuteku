using UnityEngine;
using System;
using System.Collections.Generic;

[@RequireComponent(typeof(FieldMap))]
public class FieldMapAction : MonoBehaviour {

	public UI ui;

	private FieldMap fieldMap;

	protected void Start() {
		this.fieldMap = GetComponent<FieldMap>();
	}

	protected void Update() {
		if (Input.GetMouseButtonUp(0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit)) {
				ClickLeft(fieldMap.GetMapPosition(hit.point));
			}

		}
	}

	private void ClickLeft(VectorInt2 pos) {
		var widget = ui.GetWidget(ui.toolPalettePrefab);
		if (widget != null) {
			Tool tool = widget.GetComponent<ToolPalette>().Selected;
			if (tool == Tool.INSPECTOR) {
				DoInspector(pos);
			} else {
				DoBuilding(tool, pos);
			}
		}
	}

	private void DoBuilding(Tool tool, VectorInt2 pos) {
		if (fieldMap.GetFieldElementAt(pos) == null) {
			return;
		}
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
		}
	}

	private void DoInspector(VectorInt2 pos) {

		FieldElement element = fieldMap.GetFieldElementAt(pos);
		if (element != null) {
			fieldMap.SelectFieldElement(element);


		} else {
			fieldMap.ClearSelectedFieldElements();

		}
	}
}
