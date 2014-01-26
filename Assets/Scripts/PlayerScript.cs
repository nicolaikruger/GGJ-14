using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

	// consts
	private const int NOTEAM = 0;
	private const int BLUE = 1;
	private const int RED = 2;

	// statics
	private static bool gameStarted = false;

	// pickup prefab
	public GameObject _pickupPrefab;

	// graphics
	public Material _noTeamMaterial;
	public Material _blueTeamMaterial;
	public Material _redTeamMaterial;
	private Material[] teamMaterials = new Material[3];
	public Renderer _renderer;

	// game values
	public float _dashCooldownTime = 5f;
	public float _interactionCooldownTime = 5f;
	public int _killValue;
	public int _teamlessKillValue;
	// TODO survive value?

	[HideInInspector] public int playerCount;

	[HideInInspector] public int team = NOTEAM;
	[HideInInspector] public int role = 0;

	[HideInInspector] public float dashCooldown;
	[HideInInspector] public float interactionCooldown;

	private bool alive;
	[HideInInspector] public int score;

	[HideInInspector] public string name;

	// Use this for initialization
	void Start () {
		teamMaterials[NOTEAM] = _noTeamMaterial;
		teamMaterials[BLUE] = _blueTeamMaterial;
		teamMaterials[RED] = _redTeamMaterial;
	}

	void init() {
		alive = true;
		SetTeam(NOTEAM);
		SetRole (0);
		SetScore (0);
		dashCooldown = 0f;
		interactionCooldown = 0f;
		GetComponent<BoxCollider>().enabled = true;
	}

	void OnGUI() {
		if (Network.isServer && !gameStarted) {
			// Button for starting game, for server
			if (GUI.Button (new Rect (100, 100, 200, 100), "Start game")) {
				// get all players
				var players = GameObject.FindGameObjectsWithTag("Player");

				// spawn pickups
				for (int i = 0; i < players.Length; i++) {
					GameObject go = (GameObject) Network.Instantiate(
						_pickupPrefab,
						new Vector3(Random.Range(-25, 25), 1, (Random.Range(-25, 25))),
						Quaternion.identity,
						0);
					PickupScript pickup = go.GetComponent<PickupScript>();
					pickup.color = 1 + (i % 2);
					pickup.role = 1 + Mathf.CeilToInt(i/2);
				}

				// reset and spawn each player
				foreach(GameObject obj in players) {
					PlayerScript player = obj.GetComponent<PlayerScript>();
					player.networkView.RPC("SetPlayerCount", RPCMode.All, players.Length);
					player.networkView.RPC("StartGame", RPCMode.All);
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		// Manage cooldowns
		if (dashCooldown > 0) {
			dashCooldown -= Time.deltaTime;
		}
		if (team == RED || team == BLUE) {
			if (interactionCooldown > 0) {
				Color color = _renderer.material.color;
				color.a = 0.75f; // TODO put this in inspector
				_renderer.material.color = color;
				//_renderer.material.shader = Shader.Find ("Transparent/Diffuse");
				interactionCooldown -= Time.deltaTime;
			} else {
				SetTeam(team);
				/*Color color = _renderer.material.color;
				color.a = 1.0f;
				_renderer.material.color = color;*/
				//_renderer.material.shader = Shader.Find ("Transparent/Diffuse");
			}
		}
	}

	// When player collides with shit
	void OnTriggerEnter(Collider collider) {
		// Only server handles triggers
		if (!Network.isServer) return;
		
		// can't interact if on Cooldown
		if (interactionCooldown > 0) return;
		
		// If you don't have a team, you can interact with pickups, and nothing else
		if (team == NOTEAM) {
			if (collider.tag == "Pickup") {
				PickupScript pickup = collider.gameObject.GetComponent<PickupScript>();

				networkView.RPC("SetTeam", RPCMode.All, pickup.color);
				networkView.RPC("SetRole", RPCMode.All, pickup.role);

				// remove pickup
				Network.Destroy(collider.gameObject.networkView.viewID);
			}
			else return;
		}
		
		// If you have a team, and you don't hit a player, no collision happens
		if (collider.tag != "Player") return;
		
		// target of collision
		PlayerScript target = collider.gameObject.GetComponent<PlayerScript>();
		
		// Don't collide with target if target is on cooldown
		if (target.interactionCooldown > 0) return;

		// Don't collide if target is dead
		if (!target.alive) return;

		// Kill for easy points if target has no teams (or stun)
		if (target.team == NOTEAM) {
			// Kill target
			target.networkView.RPC("Kill", RPCMode.All);
			// Increase score
			networkView.RPC ("SetScore", RPCMode.All, score + _teamlessKillValue);
		}
		// Swap roles if same team
		else if (team == target.team) {
			int thisrole = role;
			int targetrole = target.role;

			networkView.RPC("SetRole", RPCMode.All, targetrole);
			target.networkView.RPC("SetRole", RPCMode.All, thisrole);

			networkView.RPC("StartInteractionCooldown", RPCMode.All);
			target.networkView.RPC("StartInteractionCooldown", RPCMode.All);
		}
		// check for kill if other team, and/or swap if same role
		else {
			if (role > target.role) {
				// Kill target
				target.networkView.RPC("Kill", RPCMode.All);
				// Increase score
				networkView.RPC ("SetScore", RPCMode.All, score + _killValue);
			}
			else if (role == target.role) {
				int thisteam = team;
				int targetteam = target.team;

				networkView.RPC("SetTeam", RPCMode.All, targetteam);
				target.networkView.RPC("SetTeam", RPCMode.All, thisteam);
				
				networkView.RPC("StartInteractionCooldown", RPCMode.All);
				target.networkView.RPC("StartInteractionCooldown", RPCMode.All);
			}
			else if (role < target.role) {
				// Kill target
				networkView.RPC("Kill", RPCMode.All);
				// Increase score
				target.networkView.RPC ("SetScore", RPCMode.All, target.score + _killValue);
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
		GetComponent<BoxCollider>().enabled = false;
	}

	[RPC]
	public void StartGame() {
		init ();
		gameStarted = true;
	}

	[RPC]
	public void SetPosition(Vector3 pos) {
		this.transform.position = pos;
	}

	[RPC]
	public void SetName(string newName) {
		name = newName;
	}
}