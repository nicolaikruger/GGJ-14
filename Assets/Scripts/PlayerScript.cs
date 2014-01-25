using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

	// consts
	private const int NOTEAM = 0;
	private const int BLUE = 1;
	private const int RED = 2;

	public Material _noTeamMaterial;
	public Material _blueTeamMaterial;
	public Material _redTeamMaterial;
	private Material[] teamMaterials = new Material[3];
	public Renderer _renderer;

	public float _dashCooldownTime = 5f;
	public float _interactionCooldownTime = 5f;

	private int playerCount;

	private int team = NOTEAM;
	private int role = 0;

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

	void OnTriggerEnter(Collider collider) {
		if (Network.isServer) {
			if (collider.tag == "Pickup") {
				networkView.RPC("SetTeam", RPCMode.All, BLUE);
			}
		}


		if (collider.tag == "Player") {
			if(!networkView.isMine && Network.isServer)
				networkView.RPC ("Test", RPCMode.All, "heyhey");
		} 
	}


	[RPC]
	public void Test(string testString, NetworkMessageInfo nmi) {
		Debug.Log("recieved: \"" + testString + "\" from " + nmi.sender);
	}

	[RPC]
	public void SetTeam(int newTeam) {
		team = newTeam;
		_renderer.material = teamMaterials[newTeam];
		Debug.Log("Changed material");
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
	public void SetPosition(Vector3 pos) {
		SetPosition(pos);
	}
}
