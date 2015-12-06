using UnityEngine;
using System;
using System.Threading;

public static class Concurrency {

	// Wraps func in function that catches and logs all exceptions.
	// Starts func in a new thread.
	public static void StartThread(Action func, string name, Logger logger) {
		Thread thread = new Thread(() => {
			try {
				func();
			} catch (Exception e) {
				string message = String.Format("Exception from thread \"{0}\":\n{1}",
					name,
					e.ToString());
				logger.Log(message);
				Debug.Log(message);
			}
		});
		thread.Start();
	}
}
