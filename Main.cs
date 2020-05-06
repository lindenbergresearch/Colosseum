using System;
using Godot;
using Renoir;
using Renoir;





class Foo {

	[Register("main.foo.tester")]
	public static Property<int> Tester { get; set; }


	/// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
	public Foo() {
		this.Init();
	}

}


public class MainApp {


	private static void Main(string[] args) {
		var foo = new Foo();
		
		
		Logger.debug("Hello world!");
	}

}
