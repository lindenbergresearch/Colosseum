using System;
using Newtonsoft.Json.Linq;
using Renoir;


public class MainApp {

	public static int field1 = 1;
	public static string field2 = "hello";
	public static float field3 = 2.3124f;
	private static int k = 1;


	private static void Main(string[] args) {
		var d = Util.ListProperties(typeof(Level));


		foreach (var pair in d) {
			Console.WriteLine($"Key: {pair.Key} Value: {pair.Value}");
		}
		
	}

}
