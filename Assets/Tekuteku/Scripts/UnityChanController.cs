using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Animator))]
public class UnityChanController : MonoBehaviour {

	private FieldMap fieldMap;

	public FieldMap FieldMap {
		set {
			fieldMap = value;
		}
	}


	private Animator animator;
	private int speedId;
	private float speed;

	private bool isSetRotation;
	private Quaternion fromRotation;
	private Quaternion toRotation;
	private float distanceInFieldElement;

	private int currentLevel = 0;
	private FieldElement currentFieldElement;
	private FieldElement nextFieldElement;
	private Building goal = null;


	private VectorInt2 CurrentMapPosition {
		get {
			return fieldMap.GetMapPosition(transform.position);
		}
	}

	public FieldElement NextFieldElement {
		get {
			return nextFieldElement;
		}
	}


	protected void Start() {
		isSetRotation = false;
		animator = GetComponent<Animator>();
		speedId = Animator.StringToHash("Speed");


		var offices = fieldMap.Offices;
		if (offices.Count == 0) {
			Destroy(gameObject);
			return;
		}

		var goalOfficeIndex = UnityEngine.Random.Range(0, offices.Count);

		goal = offices[goalOfficeIndex];

		animator.SetFloat(speedId, 1f);

		Walk();
	}

	protected void Update() {
		animator.SetFloat(speedId, speed);
	}

	protected void FixedUpdate() {
		if (nextFieldElement != null && nextFieldElement.Vehicles.Count() >= 1) {
			FieldElement nextNext = this.GetNextFieldElement(nextFieldElement);
			if (nextNext == currentFieldElement) {
				Walk();
				return;
			}
			foreach (var vehicle in nextFieldElement.Vehicles) {
				if (vehicle.NextFieldElement == nextNext) {
					speed = 0f;
					return;
				}
			}
		}

		VectorInt2 oldMapPosition = this.CurrentMapPosition;
		var pos = gameObject.transform.position;


		if (speed < 1f) {
			speed += 0.1f;
		}

		if (!IsUTurn() || Quaternion.Angle(transform.rotation,this.toRotation) <= 10f) {
			pos += transform.rotation * new Vector3(0, 0, 1) * Time.deltaTime * speed;
		}
		transform.rotation = Quaternion.Slerp(this.fromRotation, this.toRotation, distanceInFieldElement / (Mathf.PI / 4));
		gameObject.transform.position = pos;
		distanceInFieldElement += Time.deltaTime * speed;

		if (!oldMapPosition.Equals(this.CurrentMapPosition)) {
			Walk();
		}

	}

	private void Walk() {
		if (nextFieldElement != null) {
			currentLevel = nextFieldElement.Position.z;
		}
		currentFieldElement = fieldMap.GetFieldElementAt(this.CurrentMapPosition, currentLevel);
		if (nextFieldElement != null && currentFieldElement != nextFieldElement) {
			return; // たぶんまだ次のFieldElementに来てない。(1インスタンスで2マス以上ある道路)
		}

		if (currentFieldElement != null) {
			currentFieldElement.Vehicles.Remove(this); // TODO マス+高度単位にしないと
		}
		if (currentFieldElement == goal) {
			Destroy(gameObject);
			return;
		}

		currentFieldElement.Vehicles.Add(this);
		nextFieldElement = GetNextFieldElement(currentFieldElement);
		MoveTo((nextFieldElement.Position - currentFieldElement.Position).xy.Direction4);
	}

	private FieldElement GetNextFieldElement(FieldElement fieldElement) {
		if (goal.ConnectionsFrom.Contains(fieldElement)) {
			return goal;
		}
		FieldElement result = goal.PathFinder.GetNextRoad(fieldElement);
		if (currentFieldElement.ConnectionsFrom.Contains(result) == false) {
			goal.CalculatePath();
			return goal.PathFinder.GetNextRoad(fieldElement);
		}
		else {
			return result;
		}
	}

	private void MoveTo(Direction4 d) {
		if (isSetRotation == false) {
			transform.rotation = d.Quaternion();
			isSetRotation = true;
		}
		this.fromRotation = transform.rotation;
		this.toRotation = d.Quaternion();
		distanceInFieldElement = 0;
	}

    private bool IsUTurn() {
		return Quaternion.Angle(this.toRotation,this.fromRotation) >= 100;
    }

}