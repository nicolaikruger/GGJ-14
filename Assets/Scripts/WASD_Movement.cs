using UnityEngine;
using System.Collections;

public class WASD_Movement : MonoBehaviour {

    public PlayerMovement _player;
	public Camera _cam;

	// Use this for initialization
	void Start () {
		// Disable if not player for this client
		if (!networkView.isMine) {
			enabled = false;
			_cam.enabled = false;
		}

		_player = (PlayerMovement) this.GetComponent("PlayerMovement");
	}

    void Update()
    {

		if (Input.GetKey (KeyCode.Space)) {
			_player.Dash ();
		}
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            _player.TurnLeft();
        }

		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            _player.TurnRight();
        }

		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            _player.MoveForward();
        }

		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            _player.MoveBackward();
        }
    }
}
