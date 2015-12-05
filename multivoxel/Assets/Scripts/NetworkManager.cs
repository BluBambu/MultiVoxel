using UnityEngine;

public class NetworkManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (Config.HAS_SERVER) {
			Concurrency.StartThread(() => Server.Start(Config.SERVER_PORT, Config.SERVER_LOG_FILE), "server thread");
		}
		Concurrency.StartThread(() => Client.Start(Config.SERVER_ADDRESS, Config.SERVER_PORT, Config.CLIENT_LOG_FILE), "client thread");
	}
}
