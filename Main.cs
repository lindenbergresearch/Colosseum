using System;
using Renoir.Renoir;


public class MainApp {

	public object ToBeExecuted(object param) {
		Console.WriteLine($"Starting: {param}");

		var k = 1.0;

		for (var i = 0; i < 231234234; i++) k = k * i + Math.Pow(k, i);

		return k;
	}


	private static void Main(string[] args) {
		var ma = new MainApp();

		var pe = new BackgroundExecutor(ma.ToBeExecuted);

		pe.Run(123);

		pe.WaitFor();


		Console.WriteLine($"Time is: {pe.Time}");
	}

}
