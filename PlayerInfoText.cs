using Godot;

public class PlayerInfoText : ParallaxBackground, IPropertyChangeListener {
    private Property<string> pLevelName;

    private Property<int> pScore, pTime, pCoins, pLives;
    public Label ScoreLabel { get; set; }
    public Label TimeLabel { get; set; }
    public Label CoinsLabel { get; set; }
    public Label LivesLabel { get; set; }
    public Label LevelNameLabel { get; set; }


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

        ScoreLabel = GetNode<Label>("ScoreLabel");
        TimeLabel = GetNode<Label>("TimeLabel");
        CoinsLabel = GetNode<Label>("CoinsLabel");
        LivesLabel = GetNode<Label>("LivesLabel");
        LevelNameLabel = GetNode<Label>("LevelNameLabel");
    }


//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}