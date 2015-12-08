using System.Collections.Generic;
using System.Net.Sockets;

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
	public static void Send(Socket socket, object obj) {
		byte[] data = Encoding.Serialize (obj);
		byte[] lenBytes = System.BitConverter.GetBytes ((ulong)data.LongLength);
		socket.Send (lenBytes);
		socket.Send (data);
	}

	// Receive an object from a socket.
	// Recover the type of the object using GetType.
	public static object Receive(Socket socket) {
		// read len
		byte[] lenBytes = new byte[8];
		socket.Receive (lenBytes);
		ulong len = System.BitConverter.ToUInt64(lenBytes, 0);
		
		// read data 
		byte[] data = new byte[len];
		socket.Receive (data);
		
		return Encoding.Deserialize(data);
	}
}
