using UnityEngine;
using System.Collections;
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

	private const float SPEED = 6.0f;

	private Animator animator;
	private int speedId;
	private int doWalkId;
	public float speed;

	public Quaternion fromRotation;
	public Quaternion toRotation;
	public float distanceInFieldElement;

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
		animator = GetComponent<Animator>();
		speedId = Animator.StringToHash("Speed");
		doWalkId = Animator.StringToHash("Do Walk");
		
		var offices = fieldMap.Offices;
		var goalOfficeIndex = Random.Range(0, offices.Count);

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
//		speed = 1f;

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
		if (currentFieldElement != null) {
			currentFieldElement.Vehicles.Remove(this);
		}
		currentFieldElement = fieldMap.GetFieldElementAt(this.CurrentMapPosition);
		if (currentFieldElement == goal) {
			Destroy(gameObject);
			return;
		}

		currentFieldElement.Vehicles.Add(this);
		nextFieldElement = GetNextFieldElement(currentFieldElement);
		MoveTo((nextFieldElement.Position - currentFieldElement.Position).Direction4);
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
		if (distanceInFieldElement == 0) {
//			transform.rotation = d.Quaternion();
		}
		this.fromRotation = transform.rotation;
		this.toRotation = d.Quaternion();
		distanceInFieldElement = 0;
	}

    private bool IsUTurn() {
		return Quaternion.Angle(this.toRotation,this.fromRotation) >= 100; // ピッタリ180度ではない時がある
    }

}