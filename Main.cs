using static System.Console;


public class Foo {

	public bool Flag { get; set; }


	public bool IsOnFloor() {
		return Flag;
	}

}


public class MainApp : Foo {

	[NativeState("IsOnFloor")]
	private readonly State Grounded;


	public MainApp() {
		this.SetupNativeStates();
	}


	private bool Foo => IsOnFloor();


	private static void Main(string[] args) {
		var ma = new MainApp();


		ma.Flag = true;

		WriteLine($"result: {ma.Grounded} foo: {ma.Foo}");

		ma.Flag = false;


		WriteLine($"result: {ma.Grounded} foo: {ma.Foo}");
	}

}
