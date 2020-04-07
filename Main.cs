using System;
using Renoir;

public class MainApp : IPropertyChangeListener {
    private int foo = 12;

    [Register("main.play.foo", "$main.foo")]
    public Property<int> Foo { get; set; }


    public void OnPropertyChange<T>(Property<T> sender, PropertyEventArgs<T> args) {
        Console.WriteLine($"Property changed: {args}");
    }

    private static void Main(string[] args) {
        var ma = new MainApp();

        ma.SetupPropertyBroker();

        PropertyPool.AddSubscription(ma, "$main.*");


        ma.Foo.Value = 123;
    }
}