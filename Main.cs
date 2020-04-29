using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;
using Renoir;


//https://www.codeproject.com/Questions/1256693/How-do-I-detect-a-property-change-in-3rd-party-for
public class MainApp : INotifyPropertyChanged {

	public static int field1 = 1;
	public static string field2 = "hello";
	public static float field3 = 2.3124f;
	private static int k = 1;

	/// <summary>Occurs when a property value changes.</summary>
	public event PropertyChangedEventHandler PropertyChanged;


	// This method is called by the Set accessor of each property.
	// The CallerMemberName attribute that is applied to the optional propertyName
	// parameter causes the property name of the caller to be substituted as an argument.
	private void NotifyPropertyChanged([CallerMemberName] String propertyName = "") {
		Console.WriteLine("Change event");
		if (PropertyChanged != null) {
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		Console.WriteLine(propertyName);
	}


	public string Foo { get; set; } = "bar";
	
	public int Num {
		get => k;
		set => k = value;
	}


	private static void Main(string[] args) {
		var ma = new MainApp {Foo = "Patrick", Num = 41};

		ma.Foo = "Muahjah";

		var jb = JBuilder.Create();
		jb.Add(typeof(MainApp));

		ma.Num = 123;
		
		Console.WriteLine(jb);

		Console.WriteLine(ma.GetType().GetFields().MkString());



	}

}
