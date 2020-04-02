using System;
using Godot;
using static System.Console;
using static State;


public class Foo {


	public bool IsOnFloor() {
		return true;
	}
	
	

}

public class MainApp : Foo {


	[NativeState(bind:"IsOnFloor")]
	 State Grounded = null;

	
	public MainApp() {
		this.SetupNativeStates();
	}


	static void Main(string[] args) {
		
		var ma = new MainApp();
		
		
		
		WriteLine($"result: {ma.Grounded}");
		
	}
}
