using Godot;
using static PropertyPool;

public class PlayerInfoText : ParallaxBackground, IPropertyChangeListener {
    private Property<string> pLevelName;

    private Property<int> pScore, pTime, pCoins, pLives;

    [GNode("ScoreLabel")]
    private Label ScoreLabel;

    [GNode("TimeLabel")]
    private Label TimeLabel;

    [GNode("CoinsLabel")]
    private Label CoinsLabel;

    [GNode("LivesLabel")]
    private Label LivesLabel;

    [GNode("LevelNameLabel")]
    private Label LevelNameLabel;


    public void OnPropertyChange<T>(Property<T> sender, PropertyEventArgs<T> args) {
        ScoreLabel.Text = pScore.Formatted();
        TimeLabel.Text = pTime.Formatted();
        CoinsLabel.Text = pCoins.Formatted();
        LivesLabel.Text = pLives.Formatted();
        LevelNameLabel.Text = pLevelName;
    }


    public override void _Ready() {
        AddSubscription(this, "$main.playerinfo");

        pScore = GetProperty<int>("main.player.score");
        pTime = GetProperty<int>("main.level.time");
        pCoins = GetProperty<int>("main.player.coins");
        pLives = GetProperty<int>("main.player.lives");
        pLevelName = GetProperty<string>("main.level.name");
    }


//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}