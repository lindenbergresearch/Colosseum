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


	static void Main(string[] args) {
		var p = new MainApp();

		Func<bool> test = () => true;

		p.SetupNativeStates();


		var news = NativeStates["Test"] + test;



		foreach (var nativeState in NativeStates) {
			WriteLine($"State: {nativeState.Key} = {nativeState.Value.Resolve()}");
		}
	}
}
