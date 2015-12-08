﻿using System.Collections.Generic;
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
	private static Queue<object> _sendQueue = new Queue<object>();
	private static IDictionary<System.Type, Queue<object>> _receiveQueues = new Dictionary<System.Type, Queue<object>>();
	private static Logger _logger;

	// Throws an exception on error.
	public static void Start(string serverAddress, int serverPort, string logfilePath) {
		_logger = new Logger (logfilePath);
		Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		_logger.Log (string.Format("connecting to {0}:{1}...", serverAddress, serverPort));
		socket.Connect (serverAddress, serverPort);
		_logger.Log (string.Format (
			"...connected to {0}:{1}",
			((IPEndPoint) socket.RemoteEndPoint).Address,
			((IPEndPoint) socket.RemoteEndPoint).Port));
		Concurrency.StartThread (() => Sender(socket), "client sender", _logger);
		Concurrency.StartThread (() => Receiver(socket), "client receiver", _logger);
	}

	// Send an object to the server.
	// Requires that obj is serializable.
	public static void Send(object obj) {
		lock (_sendQueue) {
			_sendQueue.Enqueue(obj);
		}
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
		lock (_receiveQueues) {
			Queue<object> queue;
			if (_receiveQueues.TryGetValue(typeof(T), out queue)) {
				if (queue.Count > 0) {
					t = (T) queue.Dequeue();
					return true;
				}
			}
			t = default(T);
			return false;
		}
	}

	private static void Sender(Socket socket) {
		_logger.Log ("sender thread started");
		while (true) {
			object obj = null;
			if (Concurrency.Dequeue(_sendQueue, out obj)) {
				System.Type type = obj.GetType();
				_logger.Log (string.Format("sending object of type {0}...", type));
				Protocol.Send(socket, obj);
				_logger.Log (string.Format("...sent object of type {0}", type));
			}
		}
	}
	
	private static void Receiver(Socket socket) {
		_logger.Log ("receiver thread started");
		while (true) {
			object obj = Protocol.Receive(socket);
			System.Type type = obj.GetType();
			_logger.Log(string.Format("received object of type {0}", type));
			lock (_receiveQueues) {
				Queue<object> queue;
				if (!_receiveQueues.TryGetValue(type, out queue)) {
					queue = new Queue<object>();
					_receiveQueues.Add (type, queue);
				}
				queue.Enqueue(obj);
			}
		}
	}
}
