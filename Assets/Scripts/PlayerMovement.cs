using UnityEngine;
using System.Collections;


public class PlayerMovement : MonoBehaviour {

    private Transform _trans;

    public float _turnSpeed = 2.0f;
    public float _walkSpeed = 4.0f;
    public float _backSpeed = 1.0f;
	public float _dashSpeed = 30.0f;

	private bool dashing = false;
	private Vector3 dashTo = new Vector3(0,0,0);

	void Update() {
		if (dashing) {
			float step = _dashSpeed * Time.deltaTime;
			_trans.position = Vector3.MoveTowards(_trans.position, dashTo, step);
		}
	}

    void Awake()
    {
		foreach (GameObject wall in GameObject.FindGameObjectsWithTag("BlueWalls")) {
			BoxCollider collider = (BoxCollider) wall.GetComponent<BoxCollider>();
			collider.enabled = false;
		}
        this._trans = this.transform;
    }

	void OnCollisionEnter(Collision collision) {
		dashing = false;
		_trans.Translate(new Vector3(0,0,0));
		Debug.Log ("COLLISION");
	}

	void OnTriggerEnter(Collider collider) {
		if (collider.tag == "Player") {			
			Debug.Log ("Trigger Event!");
		} 
	}

    public void TurnLeft()
    {
        var rot = _trans.rotation;
        _trans.rotation = rot * Quaternion.Euler(0, -_turnSpeed, 0);
    }

    public void TurnRight()
    {
        var rot = _trans.rotation;
        _trans.rotation = rot * Quaternion.Euler(0, _turnSpeed, 0);
    }

    public void MoveForward()
    {
		_trans.Translate(Vector3.forward * Time.deltaTime * _walkSpeed);
    }

	public void MoveForward(float multiplier) {
		_trans.Translate(Vector3.forward * Time.deltaTime * _walkSpeed * multiplier);
	}

    public void MoveBackward()
    {
        _trans.Translate(Vector3.back * Time.deltaTime * _backSpeed);
    }

	public void Dash() {
		RaycastHit hit;
		Vector3 fwd = _trans.TransformDirection(Vector3.forward);
		if (Physics.Raycast (_trans.position, fwd, out hit)) {
			Debug.Log ("There is something in front of the object! About " + hit.distance + " units away");
			dashTo = _trans.position + fwd * hit.distance;
			dashing = true;
		}
	}
}
