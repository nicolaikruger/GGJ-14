using UnityEngine;
using System.Collections;

public class Gravity : MonoBehaviour {
	
	private Rigidbody _rigBody;
	private GameObject _gameObj;
	
	void Awake() {
		_gameObj = this.gameObject;
        _rigBody = _gameObj.AddComponent<Rigidbody>();
        _rigBody.mass = 80.0f;
		_rigBody.useGravity = true;
		_rigBody.freezeRotation = true;
	}

    public void Disable()
    {
        _rigBody.useGravity = false;
    }

    public void Enable()
    {
        _rigBody.useGravity = true;
    }
}
