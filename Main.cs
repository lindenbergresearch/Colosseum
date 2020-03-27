using System;
using Godot;
using static System.Console;
using static GlobalState;

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

		NativeStates["Test"].Add(() => DateTime.Now.Minute > 34);

		var news = NativeStates["Test"] + test;

		AddNativeState(news);

		foreach (var nativeState in GlobalState.NativeStates) {
			WriteLine($"State: {nativeState.Key} = {nativeState.Value.Resolve()}");
		}
	}
}
