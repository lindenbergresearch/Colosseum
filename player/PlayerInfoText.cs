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
	[GNode("BitmapFont2D")]
	private BitmapFont2D _BitmapFont2D;

	[GNode("BitmapFont2D2")]
	private BitmapFont2D _BitmapFont2D2;

	[GNode("CoinsLabel")]
	private Label CoinsLabel;

	[GNode("LevelNameLabel")]
	private Label LevelNameLabel;

	[GNode("LivesLabel")]
	private Label LivesLabel;

	[GNode("ScoreLabel")]
	private Label ScoreLabel;

	[GNode("TimeLabel")]
	private Label TimeLabel;

	public void Update() {
		ScoreLabel.Text = Mario2D.TotalScore;
		TimeLabel.Text = Level.Time;
		CoinsLabel.Text = Mario2D.CollectedCoins;
		LivesLabel.Text = Mario2D.LivesLeft;
		LevelNameLabel.Text = Level.Name;

		_BitmapFont2D.Text = $"> FPS: {Node2D.FPS}";
		_BitmapFont2D2.Text = $"> MEM: {GC.GetTotalMemory(false) / 1000}K";
	}


	public override void _Ready() {
		this.Init();

		//Level.Time.AddAction(Update);
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
