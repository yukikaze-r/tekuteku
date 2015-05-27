using UnityEngine;
using System.Collections;

[RequireComponent(typeof(FieldElementComponent))]
public class RoadComponent : FieldElementComponent {
	public Texture oneWayTexture;

	private OneTileRoad road;

	// Use this for initialization
	void Start () {
		road = (OneTileRoad)GetComponent<FieldElementComponent>().FieldElement;
		if (road != null) {
			road.OneWayTypeChangeListener += OnOneWayChange;
		}
	}
	
	private void OnOneWayChange() {
		if(road.OneWayDirection == Direction4.NONE) {
			gameObject.renderer.material.mainTexture = null;
		}
		else {
			gameObject.renderer.material.mainTexture = oneWayTexture;
			transform.rotation = road.OneWayDirection.Quaternion();
		}
	}

	void OnDestroy() {
		if (road != null) {
			road.OneWayTypeChangeListener -= OnOneWayChange;
		}
	}
}
