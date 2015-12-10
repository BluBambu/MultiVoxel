using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;

public static class Concurrency {

	// Wraps func in function that catches and logs all exceptions.
	// Starts func in a new thread.
	public static void StartThread(Action func, string name, Logger logger) {
		Thread thread = new Thread(() => {
			try {
				logger.Log(string.Format("{0} thread started", name));
				func();
			} catch (Exception e) {
				string message = String.Format("Exception from thread \"{0}\":\n{1}",
					name,
					e.ToString());
				logger.Error(message);
				Debug.Log(message);
			}
		});
		thread.Start();
	}

	public static void Enqueue<T>(Queue<T> queue, T t) {
		lock (queue) {
			queue.Enqueue(t);
		}
	}

	public static bool Dequeue<T>(Queue<T> queue, out T t) {
		lock (queue) {
			if (queue.Count > 0) {
				t = queue.Dequeue();
				return true;
			} else {
				t = default(T);
				return false;
			}
		}
	}
}
