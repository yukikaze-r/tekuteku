using UnityEngine;
using System.Collections;

public class ToolPalette : MonoBehaviour {


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

	private void ChangeTool(Tool tool) {
		this.selected = tool;
	}
}


public enum Tool {
	INSPECTOR, HOUSE, OFFICE, ROAD
};