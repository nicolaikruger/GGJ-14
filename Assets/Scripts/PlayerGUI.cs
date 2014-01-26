using UnityEngine;
using System.Collections;

public class PlayerGUI : MonoBehaviour {

	public PlayerScript _playerScript;
	public Camera _cam;
	public TextMesh _nameTag;
	public GameObject _nameTagObject;


	private static Vector3 thisPlayerCamPos;

	void Start() {
		if (networkView.isMine) {
			thisPlayerCamPos = _cam.transform.position;
			// DestroyImmediate (_nameTagObject);
		}
	}

	void OnGUI() {
		if (networkView.isMine) {
			thisPlayerCamPos = _cam.transform.position;
			GUI.Label (new Rect (10, 0, 200, 20), "Score:\t" + _playerScript.score);
			GUI.Label (new Rect (10, 20, 200, 20), "Color:\t" + _playerScript.team);
			GUI.Label (new Rect (10, 40, 200, 20), "Power:\t\t" + _playerScript.role);
			GUI.Label (new Rect (10, 60, 200, 20), "Max Power:\t" + Mathf.Ceil (_playerScript.playerCount / 2));
			GUI.Label (new Rect (10, 80, 200, 20), "Dash cooldown:\t\t" + (_playerScript.dashCooldown <= 0 ? "Ready" : "" + _playerScript.dashCooldown));
			GUI.Label (new Rect (10, 100, 200, 20), "Interact cooldown:\t" + (_playerScript.interactionCooldown <= 0 ? "Ready" : "" + _playerScript.interactionCooldown));

			// right hand side labels (scoreboard)
			var players = GameObject.FindGameObjectsWithTag("Player");

			int i = 0;
			foreach(GameObject obj in players) {
				PlayerScript player = obj.GetComponent<PlayerScript>();
				GUI.Label (new Rect (10, Screen.height - (++i*20), 200, 20), player.name + ":\t" + player.score);
			}

		} else {
			_nameTag.text = "" + _playerScript.role;
			_nameTag.transform.LookAt(thisPlayerCamPos);
		}
	}
}
