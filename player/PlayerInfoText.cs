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
using Godot;
using Renoir;

#endregion


public class PlayerInfoText : ParallaxBackground {
	[GNode("Coins")]
	private BitmapFont2D Coins;

	[GNode("ColorRectDebug/FPS")]
	private BitmapFont2D FPS;

	[GNode("LevelName")]
	private BitmapFont2D LevelName;

	[GNode("LivesLeft")]
	private BitmapFont2D LivesLeft;

	[GNode("ColorRectDebug/Mem")]
	private BitmapFont2D MemInfo;

	[GNode("Score")]
	private BitmapFont2D Score;

	[GNode("Time")]
	private BitmapFont2D Time;


	public void Update() {
		Score.Text = Mario2D.TotalScore;
		Time.Text = Level.Time.ToString() + "s";
		Coins.Text = Mario2D.CollectedCoins;
		LivesLeft.Text = "x " + Mario2D.LivesLeft;
		LevelName.Text = Level.Name;

		FPS.Text = $"FPS: {Node2D.FPS}";
		MemInfo.Text = $"MEM: {GC.GetTotalMemory(false) / 1000}K";
	}


	public override void _Ready() {
		this.Init();

		Parameter.AddActionSources(
			Update,
			Mario2D.TotalScore,
			Level.Time,
			Level.Name,
			Mario2D.CollectedCoins,
			Mario2D.LivesLeft
		);
	}
}
