using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

/*
 * Client provides static methods to
 *   Connect to a server
 *   Send objects to the server
 *   Receive objects from the server
 *
 * Game clients should communicate with the server only through methods defined here.
 */
public static class Client {
	private static Queue<object> _tcpSendQueue = new Queue<object>();
	private static Queue<object> _udpSendQueue = new Queue<object>();
	private static IDictionary<System.Type, Queue<object>> _tcpReceiveQueues = new Dictionary<System.Type, Queue<object>>();
	private static IDictionary<System.Type, object> _udpReceiveMap = new Dictionary<System.Type, object> ();
	private static Logger _logger;
	private static string _address;

	// Throws an exception on error.
	public static void Start(string serverHost, int serverTcpPort, int serverUdpPort, string logfilePath) {
		_logger = new Logger (logfilePath);

		// open TCP socket
		Socket tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

		_logger.Log (string.Format("TCP connecting to {0}:{1}...", serverHost, serverTcpPort));
		tcpSocket.Connect (serverHost, serverTcpPort);
		_logger.Log (string.Format ("...TCP connected to {0}", Utils.IPAddressToString(tcpSocket.RemoteEndPoint)));
		_address = Utils.IPAddressToString (tcpSocket.LocalEndPoint);

		_logger.Log ("starting UDP client...");
		UdpClient udpClient = new UdpClient (serverHost, serverUdpPort);
		int clientUdpPort = ((IPEndPoint) udpClient.Client.LocalEndPoint).Port;
		_logger.Log (string.Format ("...started UDP client on port {0}", clientUdpPort));

		_logger.Log ("sending UDP port to server...");
		Protocol.Send (tcpSocket, clientUdpPort);
		_logger.Log ("...sent UDP port to server");

		Concurrency.StartThread (() => UdpSender(udpClient), "client UDP sender", _logger);
		Concurrency.StartThread (() => UdpReceiver(udpClient), "client UDP receiver", _logger);
		Concurrency.StartThread (() => TcpSender(tcpSocket), "client TCP sender", _logger);
		Concurrency.StartThread (() => TcpReceiver(tcpSocket), "client TCP receiver", _logger);
	}

	public static string GetAddress() {
		return _address;
	}

	// Send an object to the server over TCP.
	// Non-blocking (does no I/O operations).
	// Requires that obj is serializable.
	public static void SendTcp(object obj) {
		object copy = Encoding.Copy (obj);
		Concurrency.Enqueue (_tcpSendQueue, copy);
	}
	
	//Retrieve the *next* object of type T sent from the server over TCP, if available.
	// Non-blocking (does no I/O operations).
	public static bool TryReceiveTcp<T>(out T t) {
		lock (_tcpReceiveQueues) {
			Queue<object> queue;
			if (_tcpReceiveQueues.TryGetValue(typeof(T), out queue)) {
				if (queue.Count > 0) {
					t = (T) queue.Dequeue();
					return true;
				}
			}
			t = default(T);
			return false;
		}
	}

	// Send an object to the server over UDP.
	// Non-blocking (does no I/O operations).
	// Requires that obj is serializable.
	public static void SendUdp(object obj) {
		object copy = Encoding.Copy (obj);
		Concurrency.Enqueue (_udpSendQueue, copy);
	}

	// Retrieve the *latest* object of type T sent from the server over UDP, if available.
	// Non-blocking (does no I/O operations).
	public static bool TryReceiveUdp<T> (out T t) {
		lock (_udpReceiveMap) {
			object obj;
			if (_udpReceiveMap.TryGetValue(typeof(T), out obj)) {
				t = (T) obj;
				return true;
			}
			t = default(T);
			return false;
		}
	}

	private static void UdpSender(UdpClient udpClient) {
		while (true) {
			object obj = null;
			if (Concurrency.Dequeue(_udpSendQueue, out obj)) {
				System.Type type = obj.GetType();
				byte[] data = Encoding.Serialize(obj);
				if (Config.ENABLE_UDP_LOGGING)
					_logger.Log (string.Format("UDP sending object of type {0}...", type));
				udpClient.Send(data, data.Length);
				if (Config.ENABLE_UDP_LOGGING)
					_logger.Log (string.Format("...UDP sent object of type {0}", type));
			}
		}
	}
	
	private static void UdpReceiver(UdpClient udpClient) {
		while (true) {
			IPEndPoint ipEndPoint = null;
			byte[] data = udpClient.Receive (ref ipEndPoint);
			object obj = Encoding.Deserialize(data);
			System.Type type = obj.GetType();
			if (Config.ENABLE_UDP_LOGGING) {
				_logger.Log(string.Format ("UDP received object of type {0}", type));
			}
			lock (_udpReceiveMap) {
				_udpReceiveMap.Add(type, obj);
			}
		}
	}

	private static void TcpSender(Socket socket) {
		while (true) {
			object obj = null;
			if (Concurrency.Dequeue(_tcpSendQueue, out obj)) {
				System.Type type = obj.GetType();
				_logger.Log (string.Format("TCP sending object of type {0}...", type));
				Protocol.Send(socket, obj);
				_logger.Log (string.Format("...TCP sent object of type {0}", type));
			}
		}
	}
	
	private static void TcpReceiver(Socket socket) {
		while (true) {
			object obj;
			Protocol.Receive(socket, out obj);
			System.Type type = obj.GetType();
			_logger.Log(string.Format("TCP received object of type {0}", type));
			lock (_tcpReceiveQueues) {
				Queue<object> queue;
				if (!_tcpReceiveQueues.TryGetValue(type, out queue)) {
					queue = new Queue<object>();
					_tcpReceiveQueues.Add (type, queue);
				}
				queue.Enqueue(obj);
			}
		}
	}
}
