using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Animator))]
public class MoveUnit : MonoBehaviour {

	private const float DEADLOCK_DETECTION_TIME = 2f;

	private FieldMap fieldMap;

	public FieldMap FieldMap {
		set {
			fieldMap = value;
		}
	}


	private Animator animator;
	private int speedId;
	private float speed;

	private bool isInitializeTransformRotation;
	private Quaternion fromRotation;
	private Quaternion toRotation;
	private float distanceInFieldElement;

	public int currentFieldElementHash;
	public int nextFieldElementHash;

	private FieldElement currentFieldElement;
	private FieldElement nextFieldElement;
	private Building goal = null;

	private float stopTimeForDeadlockDetection;
	private MoveUnit blockedBy;


	public VectorInt2 CurrentMapPosition {
		get {
			return fieldMap.GetMapPosition(transform.position);
		}
	}

	public FieldElement CurrentFieldElement {
		get {
			return currentFieldElement;
		}
	}

	public FieldElement NextFieldElement {
		get {
			return nextFieldElement;
		}
	}

	public MoveUnit BlockedBy {
		get {
			return blockedBy;
		}
	}

	protected void Start() {
		isInitializeTransformRotation = false;
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

		WalkNextField();
	}

	protected void Update() {
		animator.SetFloat(speedId, speed);
		currentFieldElementHash = currentFieldElement != null ? currentFieldElement.GetHashCode() : 0;
		nextFieldElementHash = nextFieldElement != null ? nextFieldElement.GetHashCode() : 0;
		var pos = transform.position;
		pos.y = currentFieldElement.GetVehicleAltitude(new Vector2(pos.x, pos.z));
		transform.position = pos;
	}

	protected void FixedUpdate() {
		if (nextFieldElement == null) {
			StartMoveNext();
			return;
		}
		
		/*
		FieldElement nextNext = this.GetNextFieldElement(nextFieldElement);
		if (nextNext == currentFieldElement) {
			StartMoveNext(); // 移動中に経路が変わりUターン
			return;
		}*/

		if (currentFieldElement.CanMove(this, out blockedBy) == false) {
			speed = 0f;
			stopTimeForDeadlockDetection += Time.deltaTime;
			if (stopTimeForDeadlockDetection >= DEADLOCK_DETECTION_TIME) {
				DetectDeadLock();
				stopTimeForDeadlockDetection = 0f;
			}
			return;
		}

		blockedBy = null;
		stopTimeForDeadlockDetection = 0f;

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

		if (!currentFieldElement.ContainsPosition(transform.position)) {
			WalkNextField();
		}
	}

	public void DetectDeadLock() {
		var marked = new HashSet<MoveUnit>();
		MoveUnit carsor = this;
		do {
			if (marked.Contains(carsor)) {
				Debug.Log("dead lock detected");
				carsor.Destroy();
				return;
			}
			marked.Add(carsor);
			carsor = carsor.BlockedBy;
		}
		while (carsor != null);
	}

	public void Destroy() {
		currentFieldElement.MoveUnits.Remove(this);
		Destroy(gameObject);
	}

	private void WalkNextField() {
		if (currentFieldElement == null) {
			currentFieldElement = fieldMap.GetFieldElementAt(this.CurrentMapPosition, 0);
		} else {
			currentFieldElement.MoveUnits.Remove(this);
			currentFieldElement = nextFieldElement;
		}

		if (currentFieldElement == goal) {
			Destroy(gameObject);
			return;
		}

		currentFieldElement.MoveUnits.Add(this);
		StartMoveNext();
	}

	private void StartMoveNext() {
		nextFieldElement = GetNextFieldElement(currentFieldElement);
		if (nextFieldElement != null) {
			StartMove((nextFieldElement.Position - currentFieldElement.Position).xy.Direction4);
		}
	}

	public FieldElement GetNextFieldElement(FieldElement fieldElement) {
		if (goal.ConnectionsFrom.Contains(fieldElement)) {
			return goal;
		}
		return goal.PathFinder.GetNextRoad(fieldElement);
	}

	private void StartMove(Direction4 d) {
		if (isInitializeTransformRotation == false) {
			transform.rotation = d.Quaternion();
			isInitializeTransformRotation = true;
		}
		this.fromRotation = transform.rotation;
		this.toRotation = d.Quaternion();
		distanceInFieldElement = 0;
	}

    private bool IsUTurn() {
		return Quaternion.Angle(this.toRotation,this.fromRotation) >= 100;
    }

}