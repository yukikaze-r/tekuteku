using System;
using System.Collections.Generic;
using System.Linq;

public class House : Building, IDisposable {
	private TimerService.Interval interval;

	public House() {
		interval = new TimerService.Interval(OnTimer, 10f);
	}

	private void OnTimer() {
		if (Connections.Count() >= 1) {
			if (this.Vehicles.Count() == 0) {
				this.FieldMap.PutVehicle(this.Position);
			}
		}
	}

	public void Dispose() {
		interval.Dispose();
	}
}
