using System;
using System.IO;

public class Logger {

	private StreamWriter _writer;

	public Logger(string filepath) {
		_writer = new StreamWriter(File.Create (filepath));
	}

	public void Log(string message) {
		String s = String.Format ("{0}: {1}", DateTime.UtcNow.ToString ("MM/dd/yy HH:mm:ss.fff"), message);
		_writer.WriteLine (s);
		_writer.Flush ();
	}
}
