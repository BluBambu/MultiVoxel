using UnityEngine;
using System.Collections.Generic;
using System.Net.Sockets;

/*
 * Client provides an API to send and receive messages from the server.
 * 
 * This script starts a client that tries to connect to a server with parameters specified in Config.cs.
 * 
 * Game clients should communicate with the server only through static methods defined here.
 */
public class Client : MonoBehaviour {
	private static Socket _socket;
	private static IDictionary<System.Type, Queue<object>> _queues = new Dictionary<System.Type, Queue<object>>();
	
	void Start () {
		_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		_socket.Connect (Config.SERVER_ADDRESS, Config.SERVER_PORT);
		Concurrency.StartThread (RunClient, "client main thread");
	}

	private static void RunClient() {
		while (true) {
			object obj = Protocol.Receive(_socket);
			System.Type type = obj.GetType();
			lock (_queues) {
				Queue<object> queue;
				if (!_queues.TryGetValue(type, out queue)) {
					queue = new Queue<object>();
					_queues.Add (type, queue);
				}
				queue.Enqueue(obj);
			}
		}
	}

	// Send an object to the server.
	// Requires that obj is serializable.
	// TODO(kvu787): Non-blocking (does no I/O operations).
	public static void Send(object obj) {
		Protocol.Send (_socket, obj);
	}

	/*
	 * Retrieve the next object of type T sent from the server, if available.
	 * Non-blocking (does no I/O operations).
	 * 
	 * The underlying model is a set of queues, each containing objects of a single C# type.
	 * For example, given the current queue state:
	 * 
	 *   ChatMessage  : ["hello", "goodbye"]
	 *   VoxelCommand : [ cmd1, cmd2 ]
	 * 
	 * The output for several calls to TryReceive follow:
	 * 
	 *   TryReceive<VoxelCommand> => cmd1,    true
	 *   TryReceive<ChatMessage>  => "hello", true
	 *   TryReceive<VoxelCommand> => cmd2,    true
	 *   TryReceive<VoxelCommand> => ?,       false
	 *   TryReceive<VoxelDatat>   => ?,       false
	 */
	public static bool TryReceive<T>(out T t) {
		lock (_queues) {
			Queue<object> queue;
			if (_queues.TryGetValue(typeof(T), out queue)) {
				if (queue.Count > 0) {
					t = (T) queue.Dequeue();
					return true;
				}
			}
			t = default(T);
			return false;
		}
	}
}
