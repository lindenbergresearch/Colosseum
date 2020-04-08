using Renoir;
using static System.Console;


public class MainApp {


	private static void Main(string[] args) {
		var foo = new Mario2D.PlayerParameter();
		
		foo.Serialize().ToTextFile("parameter.json");
		
	}

}