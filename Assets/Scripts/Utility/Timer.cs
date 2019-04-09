using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Utilities {
	public class Timer {

		// Time left
		private float time;
		// If timer is counting down
		// IDEA: remove bool just check if time is larger than 0 instead
		private bool running;
		// Time to reset to
		private float resetTime;

		/// <summary>
		/// Creates a timer, doesn't start it
		/// </summary>
		public Timer() {
			running = false;
		}

		/// <summary>
		/// Creates a timer, starts it with a set countdown
		/// </summary>
		/// <param name="time">Countdown time</param>
		public Timer(float time) {
			Restart(time);
		}

		/// <summary>
		/// Starts/restarts the timer with the specified countdown time.
		/// </summary>
		/// <param name="time">Countdown time</param>
		public void Restart(float time) {
			this.time = time;
			resetTime = time;
			running = true;
		}

		public void Restart() {
			this.time = resetTime;
			running = true;
		}

		/// <summary>
		/// Keeps amount of ms overflown when updated past 0, retur
		/// </summary>
		/// <returns>
		/// return number of times overflown, 0 on normal reset, 
		/// 1 or more if last tick was more than entire length of countdown
		/// </returns>
		public int RestartWithDelta() {
			return RestartWithDelta(resetTime);
		}

		public int RestartWithDelta(float time) {
			int overflowTimes = 0;
			while (this.time < 0) {
				this.time += time;
				overflowTimes++;
			}
			running = true;
			return overflowTimes;
		}

		public void Extend(float time) {
			this.time += time;
			running = true;
		}

		public void Stop() {
			running = false;
		}

		/// <summary>
		/// Counts the timer down the specified amount.
		/// The timer stops once it's out of time and needs to be restarted with a new time.
		/// Returns true if (and only if) it reaches 0 on this update.
		/// </summary>
		/// <param name="dt">Amount to count dowm.</param>
		/// <returns>
		/// Returns true if it counts past 0 this update.
		/// Returns false if it hasn't reached 0 yet, or if it has already reached 0 and is no longer running.
		/// </returns>
		public bool Update(float dt) {
			if (!running) {
				return false;
			}

			time -= dt;
			if (time < 0) {
				running = false;
				return true;
			}

			return false;
		}

		public bool Update() {
			return Update(UnityEngine.Time.deltaTime);
		}

		/// <summary>
		/// Check wether the timer is running.
		/// </summary>
		/// <returns>Returns false if the timer either has not been started or has run out of time.</returns>
		public bool IsRunning() {
			return running;
		}

		public float TimeLeft() {
			if (running) {
				return time;
			} else {
				return resetTime;
			}
		}

	}
}
