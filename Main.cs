#region

using Renoir;
using static Util;

#endregion


namespace Renoir {

}


public class MainApp {


	private static void Main(string[] args) {


		var jo = JBuilder.FromProperties(typeof(Level));
		
		Logger.debug($"test: {jo.ToJson()}");


		//Initializer.Run();


		//var json = JBuilder.FromProperties(typeof(Level));
		//Logger.debug($"test {json}");
	}

}
