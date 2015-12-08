using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

/*
 * The "server" is a collection of threads running non-Unity code.
 * 
 * In general, the server is responsible for
 *   Accepting client connections
 *   Receiving and processing data from clients
 *   Sending data to clients
 * 
 * Currently, the server
 *   Expects a UDP port fom each new client as the first incoming TCP message
 *   Sends VoxelData to each new client as the first outgoing TCP message
 *   Applies each incoming TCP VoxelCommand to its own VoxelData
 *   Broadcasts each incoming TCP and UDP message to outgoing TCP and UDP connections, respectively
 */
public static class Server {

	private static Logger _logger;

	/*
	 * coarseLock must be held by code that does any of the following:
	 *   mutate _clientSockets
	 *   writing to any of the sockets in _clientSockets
	 *   touch _voxelData
	 */
	private static object _coarseLock = new object();

	private static List<Socket> _clientSockets = new List<Socket>();
	private static List<Address> _clientUdpAddresses = new List<Address> ();
	private static VoxelData _voxelData = new VoxelData();

	// Throws an exception on error.
	public static void Start(int tcpPort, int udpPort, string logfilePath) {
		_logger = new Logger (logfilePath);

		// open TCP listener
		Socket serverSocket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		serverSocket.Bind (new IPEndPoint (IPAddress.Any, tcpPort));
		serverSocket.Listen (0);
		_logger.Log (string.Format ("TCP listening on {0}", Utils.IPAddressToString(serverSocket.LocalEndPoint)));

		// open UDP listener
		UdpClient serverUdpClient = new UdpClient (udpPort);
		_logger.Log (string.Format ("UDP listening on {0}", Utils.IPAddressToString(serverUdpClient.Client.LocalEndPoint)));

		// start accepting clients
		Concurrency.StartThread (() => AcceptTcpClients (serverSocket), "server accept TCP clients loop", _logger);

		// start broadcasting incoming UDP packets
		Concurrency.StartThread (() => HandleUdp (serverUdpClient), "server broadcast UDP packets", _logger);
	}
	
	private static void AcceptTcpClients(Socket serverSocket) {
		while (true) {
			// accept client
			Socket clientSocket = serverSocket.Accept();
			_logger.Log(string.Format ("accepted client from {0}", Utils.IPAddressToString(clientSocket.RemoteEndPoint)));
		
			Concurrency.StartThread(() => HandleTcpClient(clientSocket), "server handle client", _logger);
		}
	}
	
	private static void HandleTcpClient(Socket clientSocket) {
		// receive UDP port
		int clientUdpPort = (int) Protocol.Receive(clientSocket);
		string clientHost = ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString();
		_logger.Log (string.Format ("received UDP client at {0}:{1}", clientHost, clientUdpPort));

		lock (_clientUdpAddresses) {
			// add new UDP address to list
			_clientUdpAddresses.Add(new Address(clientHost, clientUdpPort));
		}

		lock (_coarseLock) {
			// send model
			_logger.Log("sending model to client...");
			Protocol.Send(clientSocket, VoxelSerializer.SerializeVoxelData(_voxelData));
			_logger.Log("...sent model to client");
			
			// add client to sockets
			_clientSockets.Add(clientSocket);
		}

		while (true) {
			// deserialize message to command
			object obj = (VoxelCommand) Protocol.Receive(clientSocket);
			_logger.Log ("TCP received object from client");

			lock (_coarseLock) {
				// apply command to model
				if (obj.GetType() == typeof(VoxelCommand)) {
					((VoxelCommand) obj).Apply(_voxelData);
				}
				
				// broadcast command to other clients
				_logger.Log ("TCP broadcasting object to clients...");
				foreach (Socket socket in _clientSockets) {
					Protocol.Send(socket, obj);
				}
				_logger.Log ("TCP ...broadcasted object to clients");
			}
		}
	}

	private static void HandleUdp(UdpClient serverUdpClient) {
		while (true) {
			IPEndPoint ipEndPoint = null;
			byte[] data = serverUdpClient.Receive(ref ipEndPoint);
			if (Config.ENABLE_UDP_LOGGING)
				_logger.Log ("UDP received object from client");

			// acquire UDP client resources
			lock (_clientUdpAddresses) {
				if (Config.ENABLE_UDP_LOGGING)
					_logger.Log ("UDP broadcasting to clients...");
				foreach (Address clientUdpAddress in _clientUdpAddresses) {
					serverUdpClient.Send(data, data.Length, clientUdpAddress.host, clientUdpAddress.port);
				}
				if (Config.ENABLE_UDP_LOGGING)
					_logger.Log ("...UDP broadcasted to clients");
			}
		}
	}
}