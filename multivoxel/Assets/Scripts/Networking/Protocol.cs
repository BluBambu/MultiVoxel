using System.Collections.Generic;
using System.Net.Sockets;
using System;

/*
 * Protocol defines static methods to send and receive C# objects over a socket.
 * 
 * The networking stack is roughly summarized below. Higher levels depend on lower levels:
 * 
 *                     Client game loop scripts
 *                           (Unity)
 *                              |
 *                              |
 *         Server.cs ---+--- Client.cs
 *                      |
 *                      |
 *             +--------+---------+
 *             |                  |
 *             |                  |
 *     +--- Protocol.cs ----+    Config.cs
 *     |                    |
 *     |                    |
 * Encoding.cs     System.Net.Sockets (TCP)
 * 
 */
public static class Protocol {

	// Send object through the socket.
	// Blocks until obj is sent.
	// Assumes ownership of obj.
	// Returns false if Socket was closed.
	public static bool Send(Socket socket, object obj) {
		object copy = Encoding.Copy (obj);
		byte[] data = Encoding.Serialize (copy);
		byte[] lenBytes = System.BitConverter.GetBytes ((ulong)data.LongLength);
		try {
			socket.Send (lenBytes);
			socket.Send (data);
		} catch (ObjectDisposedException) {
			return false;
		}
		return true;
	}

	// Receive an object of type T from a socket.
	// Assumes that the next object is of type T.
	// Recover the type of the object using GetType.
	// Returns false if Socket was closed.
	public static bool Receive<T>(Socket socket, out T t) {
		try {
			// read len
			byte[] lenBytes = new byte[8];
			if (!FullReceive(socket, lenBytes)) {
				t = default(T);
				return false;
			}
			ulong len = System.BitConverter.ToUInt64(lenBytes, 0);
			
			// read data 
			byte[] data = new byte[len];
			if (!FullReceive(socket, data)) {
				t = default(T);
				return false;
			}

			// set outparam
			t = (T) Encoding.Deserialize(data);
			return true;
		} catch (ObjectDisposedException) {
			t = default(T);
			return false;
		}
	}

	private static bool FullReceive(Socket socket, byte[] buf) {
		int len = buf.Length;
		return socket.Receive(buf) == len;
	}
}
