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
	private Quaternion toRotate;

	private bool isMoving;
	private Direction4 direction;
	private float movingSpan;

	private FieldMap fieldMap;
	private Building goal = null;

	// Use this for initialization
	void Start() {
		animator = GetComponent<Animator>();
		speedId = Animator.StringToHash("Speed");
		doWalkId = Animator.StringToHash("Do Walk");
		toRotate = Quaternion.Euler(0, 0, 0);
		isMoving = false;
		fieldMap = GameObject.Find("Field").GetComponent<FieldMap>();
		
		var offices = fieldMap.Offices;
		var goalOfficeIndex = Random.Range(0, offices.Count);

		goal = offices[goalOfficeIndex];
	}

	private VectorInt2 CurrentMapPosition {
		get {
			return new VectorInt2((int)(transform.position.x+0.5f), (int)(transform.position.z+0.5f));
		}
	}

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


//		animator.SetFloat(speedId, speed);
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
	}

	public void MoveTo(Direction4 d) {
		if (isMoving == false) {
			Debug.Log(d);
			transform.rotation = d.Quaternion();
			direction = d;
			movingSpan = 1;

			isMoving = true;
		}
	}

}