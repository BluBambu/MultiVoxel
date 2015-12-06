using System.Collections;

public static class Config {
	public static readonly bool HAS_SERVER = !Contains(System.Environment.GetCommandLineArgs(), "--client-only");
	public const string SERVER_ADDRESS = "127.0.0.1"; // set this to IP address of server for production build
	public const int SERVER_PORT = 9999; // set this to 80 for production build
	public const string SERVER_LOG_FILE = "./server.log";
	public const string CLIENT_LOG_FILE = "./client.log";

	private static bool Contains(string[] strings, string s) {
		foreach (string str in strings) {
			if (str.Equals (s))
				return true;
		}
		return false;
	}
}
