using Godot;
using Renoir;
using static Renoir.PropertyPool;


public class PlayerInfoText : ParallaxBackground, IPropertyChangeListener {
	[GNode("CoinsLabel")] private Label CoinsLabel;
	[GNode("LevelNameLabel")] private Label LevelNameLabel;
	[GNode("LivesLabel")] private Label LivesLabel;

	[GNode("ScoreLabel")] private Label ScoreLabel;
	[GNode("TimeLabel")] private Label TimeLabel;
	[Register("main.level.name", "", "$main.playerinfo")]
	public Property<string> pLevelName { get; set; }

	[Register("main.level.time", "{0:D3}", "$main.playerinfo")]
	public Property<int> pTime { get; set; }

	[Register("main.player.coins", "", "$main.playerinfo")]
	public Property<int> pCoins { get; set; }

	[Register("main.player.lives", "", "$main.playerinfo")]
	public Property<int> pLives { get; set; }

	[Register("main.player.score", "", "$main.playerinfo")]
	public Property<int> pScore { get; set; }


	public void OnPropertyChange<T>(Property<T> sender, PropertyEventArgs<T> args) {
		ScoreLabel.Text = pScore;
		TimeLabel.Text = pTime;
		CoinsLabel.Text = pCoins;
		LivesLabel.Text = pLives;
		LevelNameLabel.Text = pLevelName;
	}


	public override void _Ready() {
		this.SetupGlobalProperties();
		this.SetupNodeBindings();

		AddSubscription(this, "$main.playerinfo");
	}


//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
