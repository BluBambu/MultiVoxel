using System;
using System.IO;

public class Logger {

	private StreamWriter _writer;
	private object _lock = new object();

	public Logger(string filepath) {
		_writer = new StreamWriter(File.Create (filepath));
	}

	public void Log(string message) {
		Print ("LOG  ", message);
	}

	public void Warn(string message) {
		Print ("WARN ", message);
	}

	public void Error(string message) {
		Print ("ERROR", message);
	}

	private void Print(string prefix, string message) {
		if (Config.ENABLE_LOGGING) {
			lock (_lock) {
				String s = String.Format ("{0} {1}: {2}", GetTimestamp(), prefix, message);
				_writer.WriteLine (s);
				_writer.Flush ();
			}
		}
	}

	private static string GetTimestamp() {
		return DateTime.Now.ToString ("hh:mm:ss.fff");
	}
}
