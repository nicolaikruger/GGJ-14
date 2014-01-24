using UnityEngine;
using System.Collections;


public class PlayerMovement : MonoBehaviour {

    private Transform _trans;
    private Gravity _gravity;

    public float _turnSpeed = 2.0f;
    public float _walkSpeed = 4.0f;
    public float _backSpeed = 2.0f;
    public float _runMultiplier = 2.0f;
    public float _jumpInitialSpeed;

    private bool _isRunning;
    private bool _enableRunning;

    private bool _isJumping = false;
    private float _curJumpSpeed;

    public bool _isGrounded = false;

    private Animation _animation;
    private Vector3 _lastPos; // used to track player movement

    void Awake()
    {
        this._trans = this.transform;
       // this._gravity = (Gravity) this.GetComponent("Gravity");

        this._isRunning = false;
        this._enableRunning = true;

        this._animation = this.animation;
        _animation.playAutomatically = true;
        
        _lastPos = _trans.position;
    }

    void OnTriggerEnter(Collider coll)
    {
        _isGrounded = true;
    }

    void OnTriggerExit(Collider coll)
    {
        _isGrounded = false;
    }

    void Update()
    {
        // Check if the player have moved since last update
        // TODO: also add check for ratation - only if animation for rotation
        if (_lastPos.Equals(_trans.position))
        {
            _animation.CrossFade("idle");
        }
        else
        {
            _lastPos = _trans.position;
        }


        if (_enableRunning)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                if (_isRunning)
                {
                    _walkSpeed /= _runMultiplier;
                    _backSpeed /= _runMultiplier;
                }
                else
                {
                    _walkSpeed *= _runMultiplier;
                    _backSpeed *= _runMultiplier;
                }

                _isRunning = !_isRunning;
            }
        }
        if (_isJumping)
        {
            if (_curJumpSpeed > 0.2f)
            {
                Debug.Log(_curJumpSpeed);
                _trans.Translate(Vector3.up * _curJumpSpeed);
                _curJumpSpeed += Physics.gravity.y * Time.deltaTime;
            }
            else
            {
                _isJumping = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
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
        if(_isRunning)
            _animation.CrossFade("run");
        else
            _animation.CrossFade("walk");

        _trans.Translate(Vector3.forward * Time.deltaTime * _walkSpeed);
    }

    public void MoveBackward()
    {
        _trans.Translate(Vector3.back * Time.deltaTime * _backSpeed);
    }

    public void Jump()
    {
        _animation.CrossFade("jump");
        if (_isGrounded && _isJumping == false) // You shall be on the ground and no double jump!
        {
            _curJumpSpeed = _jumpInitialSpeed;
            _isJumping = true;
        }
    }
}
