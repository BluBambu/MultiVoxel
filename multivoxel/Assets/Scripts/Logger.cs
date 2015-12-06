using System;
using System.IO;

public class Logger {

	private StreamWriter _writer;
	private object _lock = new object();

	public Logger(string filepath) {
		_writer = new StreamWriter(File.Create (filepath));
	}

	public void Log(string message) {
		lock (_lock) {
			String s = String.Format ("{0}: {1}", DateTime.Now.ToString ("MM/dd/yy hh:mm:ss.fff"), message);
			_writer.WriteLine (s);
			_writer.Flush ();
		}
	}
}
