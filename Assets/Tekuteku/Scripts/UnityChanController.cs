using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Animator))]
public class UnityChanController : MonoBehaviour {

	private const float SPEED = 6.0f;

	private Animator animator;
	private int speedId;
	private int doWalkId;

	private Quaternion fromRotation;
	private Quaternion toRotation;
	private float timeInFieldElement;

	private FieldMap fieldMap;
	private FieldElement currentFieldElement;
	private Road nextRoad;
	private Building goal = null;

	// Use this for initialization
	void Start() {
		animator = GetComponent<Animator>();
		speedId = Animator.StringToHash("Speed");
		doWalkId = Animator.StringToHash("Do Walk");
		fieldMap = GameObject.Find("Field").GetComponent<FieldMap>();
		
		var offices = fieldMap.Offices;
		var goalOfficeIndex = Random.Range(0, offices.Count);

		goal = offices[goalOfficeIndex];

		animator.SetFloat(speedId, 1f);

		Walk();
	}

	private VectorInt2 CurrentMapPosition {
		get {
			return fieldMap.GetMapPosition(transform.position);
		}
	}
	/*
	// Update is called once per frame
	void Update() {
		float speed = 1; // Input.GetAxis("Vertical") * SPEED;

		//ジョイパッドが手元にない人への救済処置
		float period = SPEED / 9f;
		if (Input.GetKey(KeyCode.Alpha9)) {
			speed = period * 9;
		}
		else if (Input.GetKey(KeyCode.Alpha8)) {
			speed = period * 8;
		}
		else if (Input.GetKey(KeyCode.Alpha7)) {
			speed = period * 7;
		}
		else if (Input.GetKey(KeyCode.Alpha6)) {
			speed = period * 6;
		}
		else if (Input.GetKey(KeyCode.Alpha5)) {
			speed = period * 5;
		}
		else if (Input.GetKey(KeyCode.Alpha4)) {
			speed = period * 4;
		}
		else if (Input.GetKey(KeyCode.Alpha3)) {
			speed = period * 3;
		}
		else if (Input.GetKey(KeyCode.Alpha2)) {
			speed = period * 2;
		}
		else if (Input.GetKey(KeyCode.Alpha1)) {
			speed = period * 1;
		}


		animator.SetFloat(speedId, speed);
//		animator.SetBool(doWalkId, isMoving);
		
		var pos = gameObject.transform.position;

		if (pos.z > 100 ){
			if (pos.x < 100) {
				toRotate = Quaternion.Euler(0, 90, 0);
			}
			else {
				toRotate = Quaternion.Euler(0, 180, 0);
			}
		}
		else if (pos.z < 0) {
			if (pos.x > 0) {
				toRotate = Quaternion.Euler(0, 270, 0);
			}
			else {
				toRotate = Quaternion.Euler(0, 0, 0);
			}
		}


//		transform.rotation = Quaternion.Slerp(transform.rotation, toRotate, Time.deltaTime * 0.5f);



		if (isMoving) {
			float span = Mathf.Min(movingSpan, Time.deltaTime);

			pos += transform.rotation * new Vector3(0, 0, 1) * span * speed;
			gameObject.transform.position = pos;

			movingSpan -= Time.deltaTime;
			if (movingSpan <= 0) {
				isMoving = false;
			}
		}

		if (Input.GetKey(KeyCode.A)) {
			MoveTo(Direction4.LEFT);
		}
		else if (Input.GetKey(KeyCode.S)) {
			MoveTo(Direction4.RIGHT);
		}
		else if (Input.GetKey(KeyCode.W)) {
			MoveTo(Direction4.UP);
		}
		else if (Input.GetKey(KeyCode.Z)) {
			MoveTo(Direction4.DOWN);
		}

		if (isMoving == false) {
			FieldElement current = fieldMap.GetFieldElementAt(CurrentMapPosition);
			if (current == goal) {
				Destroy(gameObject);
			}
			else if (goal.Connections.Contains(current)) {
				MoveTo((goal.Position - current.Position).Direction4);
			}
			else {
				Road next = goal.PathFinder.GetNextRoad(current);
				MoveTo((next.Position - current.Position).Direction4);
			}
//			MoveTo(path.Dequeue());
		}
	}*/

	public void FixedUpdate() {
		if (nextRoad != null && nextRoad.Vehicles.Count() >= 1) {
			return;
		}

		VectorInt2 oldMapPosition = this.CurrentMapPosition;
		var pos = gameObject.transform.position;
		pos += transform.rotation * new Vector3(0, 0, 1) * Time.deltaTime;
		transform.rotation = Quaternion.Slerp(this.fromRotation, this.toRotation, timeInFieldElement /( Mathf.PI / 4));
		gameObject.transform.position = pos;
		timeInFieldElement += Time.deltaTime;

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
		if (goal.Connections.Contains(currentFieldElement)) {
			nextRoad = null;
			MoveTo((goal.Position - currentFieldElement.Position).Direction4);
		}
		else {
			nextRoad = goal.PathFinder.GetNextRoad(currentFieldElement);
			MoveTo((nextRoad.Position - currentFieldElement.Position).Direction4);
		}
	}

	private void MoveTo(Direction4 d) {
		if (timeInFieldElement == 0) {
			transform.rotation = d.Quaternion();
		}
		this.fromRotation = transform.rotation;
		this.toRotation = d.Quaternion();
		timeInFieldElement = 0;
	}

}