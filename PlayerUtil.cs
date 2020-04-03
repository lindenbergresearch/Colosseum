namespace Colosseum {

	/// <summary>
	///     Flag with timeout setting
	/// </summary>
	internal class TimeOutFlag {

		private bool flag;
		public float Time { get; set; }
		public bool Active { get; set; }


		private void setTimeoutFlag(float timeout, bool flag = false) {
			Time = timeout;
			Active = true;
		}


		private void update(float delta) {
			Time -= delta;

			if (Time <= 0) {
				Active = false;
				flag = !flag;
			}
		}

	}


	public class PlayerUtil {

	}

}