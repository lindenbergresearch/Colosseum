#region header

// 
//    _____
//   (, /   )            ,
//     /__ /  _ __   ___   __
//  ) /   \__(/_/ (_(_)_(_/ (_  CORE LIBRARY
// (_/ ______________________________________/
// 
// 
// Renoir Core Library for the Godot Game-Engine.
// Copyright 2020-2022 by Lindenberg Research.
// 
// www.lindenberg-research.com
// www.godotengine.org
// 

#endregion

#region

using System;
using Renoir;

#endregion


internal class Foo {


	/// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
	public Foo() {
		this.Init();
	}

	[Register("main.foo.tester")]
	public static Property<int> Tester { get; set; }
}



public class MainApp {


	private static void Main(string[] args) {
		var foo = new Foo();

		Foo.Tester.Value = 100;


		var json = JBuilder.FromProperties(typeof(Level));

		Console.Out.WriteLine($"Json: {json}");
	}
}