using System;
using System.Collections.Generic;
using System.Linq;

public class House : Building, IDisposable {
	private TimerService.Interval interval;

	public House() {
		interval = new TimerService.Interval(OnTimer, 10f);
	}


	public override void RegisterFieldMap(FieldMap fieldMap, VectorInt3 position) {
		base.RegisterFieldMap(fieldMap, position);
	}

	private void OnTimer() {
		if (ConnectionsFrom.Count() >= 1) {
			if (this.Vehicles.Count() == 0) {
				this.FieldMap.PutVehicle(this.Position.xy);
			}
		}
	}

	public void Dispose() {
		interval.Dispose();
	}
}
