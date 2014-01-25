using UnityEngine;
using System.Collections;


public class PlayerMovement : MonoBehaviour {

    private Transform _trans;
    //private Gravity _gravity;

    public float _turnSpeed = 2.0f;
    public float _walkSpeed = 0.1f;
    public float _backSpeed = 1.0f;

    void Awake()
    {
        this._trans = this.transform;
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

    public void MoveBackward()
    {
        _trans.Translate(Vector3.back * Time.deltaTime * _backSpeed);
    }
}
