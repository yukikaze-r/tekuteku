using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputSystem : MonoBehaviour {
	public static InputSystem main;

	private int supressMouseButtonUpCount = 0;

	public bool IsSupressScroll {
		get;
		set;
	}

	protected void Awake() {
		main = this;
	}

	protected void Update() {
		if (supressMouseButtonUpCount > 0) {
			supressMouseButtonUpCount--;
		}
	}

	public bool GetScrollStartButton() {
		if (this.IsSupressScroll) {
			return false;
		}
		return Input.GetMouseButton(0);
	}

	public bool GetMouseButtonDown(int button) {
		if (EventSystem.current.IsPointerOverGameObject()) {
			return false;
		}
		return Input.GetMouseButtonDown(button);
	}

	public bool GetMouseButtonUp(int button) {
		if (supressMouseButtonUpCount > 0 || EventSystem.current.IsPointerOverGameObject()) {
			return false;
		}
		return Input.GetMouseButtonUp(button);
	}

	public void SupressMouseButtonUp() {
		supressMouseButtonUpCount = 2;
	}


}
