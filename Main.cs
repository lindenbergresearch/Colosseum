using System;
using Renoir;

public class MainApp : IPropertyChangeListener {
    [Register("main.play.foo", "{0:D6}", "$main.foo")]
    public Property<int> Foo { get; set; }


    public void OnPropertyChange<T>(Property<T> sender, PropertyEventArgs<T> args) {
        Console.WriteLine($"Property changed: {args}");
    }

    private static void Main(string[] args) {
        var ma = new MainApp();

        ma.SetupGlobalProperties();

        PropertyPool.AddSubscription(ma, "$main.*");
        ma.Foo.Value = 123;


        Console.WriteLine($"{ma.Foo.Formatted()}");
    }
}