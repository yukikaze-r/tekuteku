using UnityEngine;
using System.Collections;

[@RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour {

	private bool isLastButtonDown = false;
	private Vector3 lastPoint;
	private Camera camera;

	protected void Start () {
		camera = GetComponent<Camera>();
		transform.LookAt(transform.parent);
	}
	
	protected void Update () {
		if (Input.GetMouseButton(0)) {
			if(isLastButtonDown) {
				Vector3 d = Input.mousePosition - lastPoint;
				float dx = camera.fieldOfView * 3f * d.x / Screen.width;
				float dy = camera.fieldOfView * 2f * d.y / Screen.height;
				Vector3 xv = transform.localRotation * Vector3.left;
				Vector3 yv = transform.localRotation * Vector3.up;
				transform.parent.Rotate(xv, dy);
				transform.parent.Rotate(yv, dx);
				transform.LookAt(transform.parent);
			}
			lastPoint = Input.mousePosition;
			isLastButtonDown = true;
		} else {
			isLastButtonDown = false;
		}
	}
}
