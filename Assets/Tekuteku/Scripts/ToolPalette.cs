using UnityEngine;
using System;
using System.Collections;

public class ToolPalette : MonoBehaviour {

	public event Action ChangeSlectionListener = delegate { };

	private Tool selected = Tool.INSPECTOR;

	public Tool Selected {
		get {
			return selected;
		}
	}

	public void SetInspector() {
		ChangeTool(Tool.INSPECTOR);
	}

	public void SetHouse() {
		ChangeTool(Tool.HOUSE);
	}

	public void SetOffice() {
		ChangeTool(Tool.OFFICE);
	}

	public void SetRoad() {
		ChangeTool(Tool.ROAD);
	}

	public void SetSlope() {
		ChangeTool(Tool.SLOPE);
	}

	private void ChangeTool(Tool tool) {
		this.selected = tool;
		ChangeSlectionListener();
	}
}


public enum Tool {
	INSPECTOR, HOUSE, OFFICE, ROAD, SLOPE
};