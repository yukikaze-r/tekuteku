using System;
using System.Collections.Generic;

public class House : Building, IDisposable {
	private TimerService.Interval interval;

	public House() {
		interval = new TimerService.Interval(OnTimer, 1f);
	}

	private void OnTimer() {
		this.FieldMap.PutVehicle(this.Position);
	}

	public void Dispose() {
		interval.Dispose();
	}
}
