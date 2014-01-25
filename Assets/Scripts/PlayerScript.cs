using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

	// consts
	private const int NOTEAM = 0;
	private const int BLUE = 1;
	private const int RED = 2;

	// graphics
	public Material _noTeamMaterial;
	public Material _blueTeamMaterial;
	public Material _redTeamMaterial;
	private Material[] teamMaterials = new Material[3];
	public Renderer _renderer;

	// game values
	public float _dashCooldownTime = 5f;
	public float _interactionCooldownTime = 5f;

	private int playerCount;

	int team = NOTEAM;
	int role = 0;

	private float dashCooldown;
	private float interactionCooldown;

	private bool alive;
	private int score;





	// Use this for initialization
	void Start () {
		teamMaterials[NOTEAM] = _noTeamMaterial;
		teamMaterials[BLUE] = _blueTeamMaterial;
		teamMaterials[RED] = _redTeamMaterial;
	}
	
	// Update is called once per frame
	void Update () {
		// Manage cooldowns
		if (dashCooldown > 0) {
			dashCooldown -= Time.deltaTime;
		}
		if (interactionCooldown > 0) {
			interactionCooldown -= Time.deltaTime;
		}
	
	}

	// When player collides with shit
	void OnTriggerEnter(Collider collider) {
		if (Network.isServer) {
			if (collider.tag == "Pickup") {
				// TODO Need pickups first
			}

			else if (collider.tag == "Player") {
				PlayerScript other = collider.gameObject.GetComponent<PlayerScript>();

			}
		}
	}


	[RPC]
	public void Test(string testString, NetworkMessageInfo nmi) {
		Debug.Log("recieved: \"" + testString + "\" from " + nmi.sender);
	}

	[RPC]
	public void SetTeam(int newTeam) {
		// change team and color
		team = newTeam;
		_renderer.material = teamMaterials[newTeam];

		// fix wall-collision if your team was changed
		if (networkView.isMine) {
			foreach (GameObject wall in GameObject.FindGameObjectsWithTag("BlueWalls")) {
				BoxCollider collider = (BoxCollider) wall.GetComponent<BoxCollider>();
				collider.enabled = team == BLUE ? false : true;
			}

			foreach (GameObject wall in GameObject.FindGameObjectsWithTag("RedWalls")) {
				BoxCollider collider = (BoxCollider) wall.GetComponent<BoxCollider>();
				collider.enabled = team == RED ? false : true;
			}
		}
	}

	[RPC]
	public void SetRole(int newRole) {
		role = newRole;
		// TODO somehow show changed role
	}

	[RPC]
	public void StartDashCooldown() {
		dashCooldown = _dashCooldownTime;
	}

	[RPC]
	public void StartInteractionCooldown() {
		interactionCooldown = _interactionCooldownTime;
	}

	[RPC]
	public void SetScore(int newScore) {
		score = newScore;
	}

	[RPC]
	public void SetPlayerCount(int newCount) {
		playerCount = newCount;
	}

	// TODO add for alive/dead as well
	[RPC]
	public void Kill() {
		alive = false;
		// TODO whatevs, maybe remove, maybe dead body, w/e
		SetPosition(new Vector3(0, -50, 0));
	}

	[RPC]
	public void Spawn() {

	}

	[RPC]
	public void SetPosition(Vector3 pos) {
		SetPosition(pos);
	}
}
