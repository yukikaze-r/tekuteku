using System;
using System.Collections.Generic;

public class House : Building, IDisposable {
	private TimerService.Interval interval;

	public House() {
		interval = new TimerService.Interval(OnTimer, 1f);
	}

	private void OnTimer() {
		UnityEngine.Debug.Log("OnTimer House");
	}

	public void Dispose() {
		interval.Dispose();
	}
}
