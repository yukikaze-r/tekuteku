using UnityEngine;
using System.Collections;

public class ToolPalette : MonoBehaviour {


	private Tool tool = Tool.HOUSE;

	public Tool Tool {
		get {
			return tool;
		}
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
		this.tool = tool;
	}
}


public enum Tool {
	HOUSE, OFFICE, ROAD
};