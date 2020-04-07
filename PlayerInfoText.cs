using Godot;
using static PropertyPool;

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
        AddSubscription(this, "$main.playerinfo");

        pScore = GetProperty<int>("main.player.score");
        pTime = GetProperty<int>("main.level.time");
        pCoins = GetProperty<int>("main.player.coins");
        pLives = GetProperty<int>("main.player.lives");
        pLevelName = GetProperty<string>("main.level.name");

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