namespace Colosseum {

	
	/// <summary>
	/// Flag with timeout setting
	/// </summary>
	class TimeOutFlag {
		public float Time { get; set; }
		public bool Active { get; set; }

		private bool flag;


		void setTimeoutFlag(float timeout, bool flag = false) {
			Time = timeout;
			Active = true;
		}


		void update(float delta) {
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
