using UnityEngine;
using System;
using System.Threading;

public static class Concurrency {
	// TODO(kvu787): cleanup
	public static void StartThread(Action func, string name) {
		Thread thread = new Thread(() => {
			try {
				func();
			} catch (UnityException e) {
				Debug.Log("unity exception from " + "\"" + name + "\"");
				Debug.LogException(e);
			} catch (Exception e) {
				Debug.Log("non-unity exception from " + "\"" + name + "\"");
				Debug.Log (e.ToString());
			}
		});
		thread.Start ();
	}
}
