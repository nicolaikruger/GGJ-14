using UnityEngine;
using System.Collections;

public class PlayerGUI : MonoBehaviour {

	public PlayerScript _playerScript;

	void OnGUI() {
		if (networkView.isMine) {
			GUI.Label (new Rect (10, 0, 200, 20), "Score:\t" + _playerScript.score);
			GUI.Label (new Rect (10, 20, 200, 20), "Team:\t" + _playerScript.team);
			GUI.Label (new Rect (10, 40, 200, 20), "Role:\t\t" + _playerScript.role);
			GUI.Label (new Rect (10, 60, 200, 20), "Max Role:\t" + Mathf.Ceil(_playerScript.playerCount/2));
			GUI.Label (new Rect (10, 80, 200, 20), "Dash cooldown:\t\t" + (_playerScript.dashCooldown <= 0 ? "Ready" : "" + _playerScript.dashCooldown));
			GUI.Label (new Rect (10, 100, 200, 20), "Interact cooldown:\t" + (_playerScript.interactionCooldown <= 0 ? "Ready" : "" + _playerScript.interactionCooldown));
		}
	}
}
