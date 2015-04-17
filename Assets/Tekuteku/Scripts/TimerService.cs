using UnityEngine;
using System;
using System.Collections.Generic;

public class TimerService : MonoBehaviour {

	private static TimerService instance;
	private List<Interval> intervals = new List<Interval>();
	private HashSet<Interval> removes = new HashSet<Interval>();

	void Awake() {
		instance = this;
	}

	void FixedUpdate() {
		float current = Time.time;

		foreach (var interval in intervals) {
			interval.Check(current);
		}

		if (removes.Count >= 1) {
			intervals.RemoveAll(e => removes.Contains(e));
			removes.Clear();
		}
	}

	public class Interval : IDisposable {
		private float lastTime;
		private float intervalSpan;
		private Action action;

		public Interval(Action action, float intervalSpan) {
			this.action = action;
			this.lastTime = Time.time;
			this.intervalSpan = intervalSpan;
			TimerService.instance.intervals.Add(this);
		}


		public void Check(float current) {
			float performTime = lastTime + intervalSpan;
			if (performTime <= current) {
				action();
				lastTime = performTime;
			}
		}

		public void Dispose() {
			TimerService.instance.removes.Add(this);
		}
	}


}
