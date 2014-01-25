using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

	public float speed = 5f;

	// Update is called once per frame
	void Update () {
		// only let the owner move this object
		if (networkView.isMine) {
			if (Input.GetKey(KeyCode.W)) {
				transform.Translate(Vector3.forward * Time.deltaTime * speed);
			}
			if (Input.GetKey(KeyCode.S)) {
				transform.Translate(Vector3.back * Time.deltaTime * speed);
			}
			if (Input.GetKey(KeyCode.A)) {
				transform.Translate(Vector3.left * Time.deltaTime * speed);
			}
			if (Input.GetKey(KeyCode.D)) {
				transform.Translate(Vector3.right * Time.deltaTime * speed);
			}
		}
		else enabled = false;
	}
}
