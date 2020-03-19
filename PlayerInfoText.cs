using Godot;
using System;

public class PlayerInfoText : ParallaxBackground, IPropertyChangeListener {
	public Godot.Label ScoreLabel { get; set; }
	public Godot.Label TimeLabel { get; set; }
	public Godot.Label CoinsLabel { get; set; }
	public Godot.Label LivesLabel { get; set; }
	public Godot.Label LevelNameLabel { get; set; }

	private Property<int> pScore, pTime, pCoins, pLives;
	private Property<string> pLevelName;


	public void OnPropertyChange<T>(Property<T> sender, PropertyEventArgs<T> args) {
		ScoreLabel.Text = pScore.Formatted();
		TimeLabel.Text = pTime.Formatted();
		CoinsLabel.Text = pCoins.Formatted();
		LivesLabel.Text = pLives.Formatted();
		LevelNameLabel.Text = pLevelName;
	}


	public override void _Ready() {
		PropertyPool.AddSubscription(this, "$main.playerinfo");

		pScore = PropertyPool.Pull<int>("main.player.score");
		pTime = PropertyPool.Pull<int>("main.level.time");
		pCoins = PropertyPool.Pull<int>("main.player.coins");
		pLives = PropertyPool.Pull<int>("main.player.lives");
		pLevelName = PropertyPool.Pull<string>("main.level.name");

		ScoreLabel = GetNode<Godot.Label>("ScoreLabel");
		TimeLabel = GetNode<Godot.Label>("TimeLabel");
		CoinsLabel = GetNode<Godot.Label>("CoinsLabel");
		LivesLabel = GetNode<Godot.Label>("LivesLabel");
		LevelNameLabel = GetNode<Godot.Label>("LevelNameLabel");
	}


//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
