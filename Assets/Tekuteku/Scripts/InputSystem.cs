using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputSystem : MonoBehaviour {
	public static InputSystem main;

	public int supressMouseButtonUpCount = 0;

	protected void Awake() {
		main = this;
	}

	protected void Update() {
		if (supressMouseButtonUpCount > 0) {
			supressMouseButtonUpCount--;
		}
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
