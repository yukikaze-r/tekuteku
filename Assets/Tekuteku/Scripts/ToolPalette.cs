using UnityEngine;
using System;
using System.Collections;

public class ToolPalette : MonoBehaviour {

	public event Action ChangeSlectionListener = delegate { };

	private Tool selected = Tool.INSPECTOR;
	private int level = 0;

	public Tool Selected {
		get {
			return selected;
		}
	}

	public int Level {
		get {
			return level;
		}
	}

	public void SetLevel(int level) {
		this.level = level;
		ChangeSlectionListener();
	}

	public void SetInspector(bool isOn) {
		ChangeTool(Tool.INSPECTOR, isOn);
	}

	public void SetHouse(bool isOn) {
		ChangeTool(Tool.HOUSE, isOn);
	}

	public void SetOffice(bool isOn) {
		ChangeTool(Tool.OFFICE, isOn);
	}

	public void SetRoad(bool isOn) {
		ChangeTool(Tool.ROAD, isOn);
	}

	public void SetSlope(bool isOn) {
		ChangeTool(Tool.SLOPE, isOn);
	}

	private void ChangeTool(Tool tool, bool isOn) {
		if (isOn) {
			this.selected = tool;
			ChangeSlectionListener();
		}
	}
}


public enum Tool {
	INSPECTOR, HOUSE, OFFICE, ROAD, SLOPE
};
