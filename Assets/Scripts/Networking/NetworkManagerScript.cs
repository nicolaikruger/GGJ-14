using UnityEngine;
using System.Collections;

public class NetworkManagerScript : MonoBehaviour {

	// Inspector vars
	public int maxConnections = 4;
	public int port = 25002;
	public string gameNameForMasterServer = "GGJ14_Game_CPH";
	public string gameName = "GGJ14 Game server!";
	public string comment = "";

	public Transform spawnPoint;
	public GameObject playerPrefab;

	// Server list
	private HostData[] hosts;
	private bool refreshing = false;

	// Player name
	private string playerName = "PlayerName";
	// directIP
	private string ip = "127.0.0.1";

	// button size whatevs
	private float btnX;
	private float btnY;
	private float btnW;
	private float btnH;

	// Called on instantiate
	void Start() {
		btnX = Screen.width * 0.05f;
		btnY = Screen.height * 0.05f;
		btnW = Screen.width * 0.2f;
		btnH = Screen.width * 0.05f;
	}

	//
	// Networking shit up here
	//

	void StartServer() {
		Network.InitializeServer(maxConnections, port, !Network.HavePublicAddress());

	}

	// ask master server for list of games
	void RefreshHostList() {
		refreshing = true;
		MasterServer.RequestHostList(gameNameForMasterServer);
	}

	// Spawn the player at the spawn point object
	void SpawnPlayer() {
		GameObject go = (GameObject) Network.Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity, 0);
		// set player name
		go.GetComponent<PlayerScript>().networkView.RPC("SetName", RPCMode.AllBuffered, playerName);
	}


	//
	// Event shit here
	//

	// Called when server is initialized
	void OnServerInitialized() {
		Debug.Log("Server initialized! Broadcasting to master server...");
		MasterServer.RegisterHost(gameNameForMasterServer, gameName, comment);
		SpawnPlayer();
	}

	void OnConnectedToServer() {
		Debug.Log("Connected to server!");
		SpawnPlayer();
	}

	// Called when master server shit happens
	void OnMasterServerEvent(MasterServerEvent mse) {
		// When registration happens
		if (mse == MasterServerEvent.RegistrationSucceeded) {
			Debug.Log("Server registered on Master server!");
		}

		// When server list is recieved, store it
		if (mse == MasterServerEvent.HostListReceived) {
			hosts = MasterServer.PollHostList();
			refreshing = false;
		}
	}


	//
	// Other server events
	//
	void OnPlayerDisconnected(NetworkPlayer player) {
		Debug.Log("Clean up after player " +  player);
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}




	//
	// GUI shit down here
	//

	// Called with each GUI event (multiple times per frame sometimes)
	void OnGUI() {
		// Don't show connection UI if you are connected or hosting server already
		if (!Network.isServer && !Network.isClient) {
			playerName = GUI.TextField(new Rect(btnX, btnY, btnW, btnH *0.5f), playerName);

			if (GUI.Button(new Rect(btnX, btnY * 1.2f + btnH, btnW, btnH), "Start server")) {
				Debug.Log ("Starting Server");
				StartServer();
			}
			if (GUI.Button(new Rect(btnX, btnY * (1.2f*2f) + btnH, btnW, btnH), "Find servers")) {
				Debug.Log ("Refreshing server list...");
				RefreshHostList();
			}
			if (GUI.Button(new Rect(btnX, btnY * (1.2f*3f) + btnH, btnW, btnH), "Connect to IP")) {
				Network.Connect(ip, port);
			}

			ip = GUI.TextField(new Rect(btnX, btnY * (1.2f*4f) + btnH, btnW, btnH *0.5f), ip);

			if (hosts != null) {
				for (int i = 0; i < hosts.Length; i++) {
					// put out a button for each server, clicking it will connect to server
					if (
					GUI.Button(
						new Rect(btnX * 1.5f + btnW, btnY * 1.2f + (btnH * i), btnW*3f, btnH), 
						hosts[i].ip[0] + " - " + hosts[i].connectedPlayers + " players")
					) {
						Network.Connect(hosts[i]);
					}
				}
			}
		}
	}
}
