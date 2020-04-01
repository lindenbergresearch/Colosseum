using System;
using Godot;
using static System.Console;
using static State;


public class MainApp {
	[NativeState]
	public bool Test { get; set; } = true;

	[NativeState]
	public bool Foo { get; set; } = true;

	[NativeState]
	public bool Bar { get; set; }


	[NativeState]
	public bool IsGrounded() => true;
	
	
	static void Main(string[] args) {
		var p = new MainApp();
		p.SetupNativeStates();

		Func<bool> f = p.IsGrounded;
		
		PolyState foo = (PolyState)p.IsGrounded + p.IsGrounded + p.IsGrounded;


		foreach (var nativeState in NativeStates) {
			WriteLine($"State: {nativeState.Key} = {nativeState.Value.Resolve()}");
		}
	}
}
