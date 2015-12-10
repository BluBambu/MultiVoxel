using System.Collections;

public static class Config {
	public const string SERVER_LOG_FILE = "./server.log";
	public const string CLIENT_LOG_FILE = "./client.log";
	public static readonly bool ENABLE_LOGGING = !Contains(System.Environment.GetCommandLineArgs(), "--no-log");
	public const bool ENABLE_UDP_LOGGING = true; // this will probably flood the logs with UDP events

	private static bool Contains(string[] strings, string s) {
		foreach (string str in strings) {
			if (str.Equals (s))
				return true;
		}
		return false;
	}
}
