﻿using UnityEngine;
using System.Collections;

[@RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour {

	private const float SUPRESS_MOUSE_UP_SCROLL_DISTANCE = 5f;

	private bool isLastButtonDown = false;
	private float moveDistance = 0;
	private Vector3 lastPoint;
	private Camera camera;

	protected void Start () {
		camera = GetComponent<Camera>();
		transform.LookAt(transform.parent);
	}
	
	protected void Update () {
		if (InputSystem.main.GetScrollStartButton()) {
			if(isLastButtonDown) {
				if (Input.GetKey("left shift")) {
					Vector3 d = Input.mousePosition - lastPoint;
					moveDistance += d.magnitude;
					float dx = camera.fieldOfView * 3f * d.x / Screen.width;
					float dy = camera.fieldOfView * 2f * d.y / Screen.height;
					Vector3 xv = transform.localRotation * Vector3.left;
					Vector3 yv = transform.localRotation * Vector3.up;
					transform.parent.Rotate(xv, dy);
					transform.parent.Rotate(yv, dx);
					transform.LookAt(transform.parent);
				} else {
					RaycastHit hit1, hit2;
			        Ray ray1 = camera.ScreenPointToRay(Input.mousePosition);
					if (Physics.Raycast(ray1, out hit1)) {
						Ray ray2 = camera.ScreenPointToRay(lastPoint);
						if (Physics.Raycast(ray2, out hit2)) {
							moveDistance += (Input.mousePosition - lastPoint).magnitude;
							transform.parent.Translate(hit2.point - hit1.point, Space.World);
							
						}
					}
				}
			}

			if (moveDistance > SUPRESS_MOUSE_UP_SCROLL_DISTANCE) {
				InputSystem.main.SupressMouseButtonUp();
			}

			lastPoint = Input.mousePosition;
			isLastButtonDown = true;
		} else {
			isLastButtonDown = false;
			moveDistance = 0f;
		}

		transform.Translate(new Vector3(0, 0, Input.GetAxis("Mouse ScrollWheel")*16f));
	}
}
