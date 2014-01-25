using UnityEngine;
using System.Collections;

public class WASD_Movement : MonoBehaviour {

    public PlayerMovement _player;

	// Use this for initialization
	void Start () {
        _player = (PlayerMovement) this.GetComponent("PlayerMovement");
	}

    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            _player.TurnLeft();
            setPlayerModeToCombet();
        }

        if (Input.GetKey(KeyCode.D))
        {
            _player.TurnRight();
            setPlayerModeToCombet();
        }

        if (Input.GetKey(KeyCode.W))
        {
            _player.MoveForward();
            setPlayerModeToCombet();
        }

        if (Input.GetKey(KeyCode.S))
        {
            _player.MoveBackward();
            setPlayerModeToCombet();
        }
    }

    private void setPlayerModeToCombet()
    {
       // PlayerCharacter.PlayerMode = PlayerMode.Combat;
    }
}
