using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

/*
 * The "server" is a collection of threads running C# (non-Unity) code.
 * 
 * This scripts starts the server with parameters in Config.cs.
 * 
 * In general, the server is responsible for
 *   Accepting client connections
 *   Receiving and processing data from clients
 *   Sending data to clients
 * 
 * Currently, the server
 *   Merges VoxelCommand's from each client
 *   Broadcasts VoxelCommand's to each client
 *   Applies VoxelCommand's to its own VoxelData
 *   Sends VoxelData to each new client
 */
public class Server : MonoBehaviour {

	/*
	 * coarseLock should be held by code that does any of the following:
	 *   mutating clientSockets
	 *   writing to any of the sockets in clientSockets
	 *   touching voxelData
	 */
	private static object coarseLock = new object();
	private static List<Socket> clientSockets = new List<Socket>();
	private static VoxelData voxelData = new VoxelData();
	
	void Awake () {
		if (Config.HAS_SERVER) {
			Socket serverSocket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			serverSocket.Bind (new IPEndPoint (IPAddress.Parse(Config.SERVER_ADDRESS), Config.SERVER_PORT));
			serverSocket.Listen (0);
			Concurrency.StartThread(() => AcceptClients(serverSocket), "server accepting clients loop");
		}
	}

	private static void AcceptClients(Socket serverSocket) {
		while (true) {
			// accept client
			Socket clientSocket = serverSocket.Accept();

			lock (coarseLock) {
				// send model
				Protocol.Send(clientSocket, voxelData);

				// add client to sockets
				clientSockets.Add(clientSocket);
			}
			Concurrency.StartThread(() => HandleClient(clientSocket), "server handle client");
		}
	}

	private static void HandleClient(Socket clientSocket) {
		while (true) {
			// deserialize message to command
			VoxelCommand cmd = (VoxelCommand) Protocol.Receive(clientSocket);

			lock (coarseLock) {
				// apply command to model
				cmd.Apply(voxelData);

				// broadcast command to other clients
				foreach (Socket socket in clientSockets) {
					Protocol.Send(socket, cmd);
				}
			}
		}
	}
}
