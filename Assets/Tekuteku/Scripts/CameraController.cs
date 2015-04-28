using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	private bool isLastButtonDown = false;
	private Vector3 lastPoint;

	protected void Start () {
	
	}
	
	protected void Update () {
		if (Input.GetMouseButton(0)) {
			if(isLastButtonDown) {
				var d = Input.mousePosition - lastPoint;
				Debug.Log("d:"+d.x / Screen.width);
				var rotationDegree = Camera.main.fieldOfView * d.x / Screen.width;
				// http://0310unity.hateblo.jp/entry/unity_camera_circlemove_1
			}
			lastPoint = Input.mousePosition;
			isLastButtonDown = true;
		} else {
			isLastButtonDown = false;
		}
	}
}
