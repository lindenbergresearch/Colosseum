using System;
using Renoir;


/// <summary>
/// Custom attribute to specify a global property
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class TesterAttribute : Attribute {


	public TesterAttribute(string hello) {
		Console.WriteLine("boom" + hello);
	}


}


public class MainApp {

	public static int field1 = 1;
	public static string field2 = "hello";
	public static float field3 = 2.3124f;
	private static int k = 1;

	[Tester("dsdsd")]
	public string Foo { get; set; } = "bar";

	[Tester("dsdsd")]
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

		foreach (var propertyInfo in ma.GetType().GetProperties()) {
			foreach (var customAttribute in propertyInfo.GetCustomAttributes(true)) Console.WriteLine(customAttribute.ToString());
		}
	}

}
