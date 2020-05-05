using System;
using Renoir;
using Renoir.Renoir;


namespace Renoir {

	/// <summary>
	/// 
	/// </summary>
	abstract class Initializer {

		/// <summary>
		/// This flag is set after running the static initialisation.
		/// </summary>
		protected bool StaticFinished { get; set; } = false;

		
		
		
		
		
	}
	

}


public class MainApp {

	public object ToBeExecuted(object param) {
		Console.WriteLine($"Starting: {param}");

		var k = 1.0;

		for (var i = 0; i < 231234234; i++) k = k * i + Math.Pow(k, i);

		return k;
	}


	private static void Main(string[] args) {
		var ma = new MainApp();
		var pe = new BackgroundExecutor(obj => Console.WriteLine("dsds"));

		pe.Run();
		pe.WaitFor();

		
		Console.WriteLine($"Time is: {pe.Time} ");
	}

}
