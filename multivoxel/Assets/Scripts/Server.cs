using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

/*
 * The "server" is a collection of threads running C# (non-Unity) code.
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
public static class Server {
	
	/*
	 * coarseLock should be held by code that does any of the following:
	 *   mutating clientSockets
	 *   writing to any of the sockets in clientSockets
	 *   touching voxelData
	 */
	private static object _coarseLock = new object();
	private static List<Socket> _clientSockets = new List<Socket>();
	private static VoxelData _voxelData = new VoxelData();
	private static Logger _logger;

	// Throws an exception on error.
	public static void Start(int tcpPort, string logfilePath) {
		_logger = new Logger (logfilePath);
		Socket serverSocket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		serverSocket.Bind (new IPEndPoint (IPAddress.Any, tcpPort));
		serverSocket.Listen (0);
		_logger.Log (string.Format (
			"listening on {0}:{1}",
			((IPEndPoint) serverSocket.LocalEndPoint).Address,
			((IPEndPoint) serverSocket.LocalEndPoint).Port));
		Concurrency.StartThread (() => AcceptClients (serverSocket), "server accept client loop", _logger);
	}

	private static void AcceptClients(Socket serverSocket) {
		_logger.Log ("accept clients thread started");
		while (true) {
			// accept client
			Socket clientSocket = serverSocket.Accept();
			_logger.Log(string.Format (
				"accepted client from {0}:{1}",
				((IPEndPoint) clientSocket.RemoteEndPoint).Address,
				((IPEndPoint) clientSocket.RemoteEndPoint).Port));
		
			Concurrency.StartThread(() => HandleClient(clientSocket), "server handle client", _logger);
		}
	}
	
	private static void HandleClient(Socket clientSocket) {
		lock (_coarseLock) {
			// send model
			_logger.Log("sending model to client...");
			Protocol.Send(clientSocket, _voxelData);
			_logger.Log("...sent model to client");
			
			// add client to sockets
			_clientSockets.Add(clientSocket);
		}

		while (true) {
			// deserialize message to command
			VoxelCommand cmd = (VoxelCommand) Protocol.Receive(clientSocket);
			_logger.Log ("received command from client");
			
			lock (_coarseLock) {
				// apply command to model
				cmd.Apply(_voxelData);
				
				// broadcast command to other clients
				_logger.Log ("broadcasting command to clients...");
				foreach (Socket socket in _clientSockets) {
					Protocol.Send(socket, cmd);
				}
				_logger.Log ("...broadcasted command to clients");
			}
		}
	}
}