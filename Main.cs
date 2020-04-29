using System;
using Newtonsoft.Json.Linq;
using Renoir;

//https://www.codeproject.com/Questions/1256693/How-do-I-detect-a-property-change-in-3rd-party-for
public class MainApp {

	public static int field1 = 1;
	public static string field2 = "hello";
	public static float field3 = 2.3124f;
	private static int k = 1;


	private static void Main(string[] args) {
		var jb = JBuilder.Create();
		
		jb.Add(typeof(MainApp));
		jb.Add(typeof(Level));
		
		Console.WriteLine(jb.Json);
		

	}

}
