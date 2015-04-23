using UnityEngine;
using System.Collections;

[RequireComponent(typeof(FieldElementComponent))]
public class RoadComponent : FieldElementComponent {
	public Texture oneWayTexture;

	private Road road;

	// Use this for initialization
	void Start () {
		road = (Road) GetComponent<FieldElementComponent>().FieldElement;
		road.OneWayTypeChangeListener += OnOneWayChange;
	}
	
	private void OnOneWayChange() {
		if(road.OneWayType == OneWayType.NONE) {
			gameObject.renderer.material.mainTexture = null;
		}
		else {
			gameObject.renderer.material.mainTexture = oneWayTexture;
			switch (road.OneWayType) {
				case OneWayType.DOWN_TOP:
					transform.rotation = Direction4.UP.Quaternion();
					break;
				case OneWayType.TOP_DOWN:
					transform.rotation = Direction4.DOWN.Quaternion();
					break;
				case OneWayType.LEFT_RIGHT:
					transform.rotation = Direction4.RIGHT.Quaternion();
					break;
				case OneWayType.RIGHT_LEFT:
					transform.rotation = Direction4.LEFT.Quaternion();
					break;
			}

		}

	}

	void OnDestroy() {
		road.OneWayTypeChangeListener -= OnOneWayChange;
	}
}
