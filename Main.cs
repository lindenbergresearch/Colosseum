using static System.Console;


public class Foo {


	public bool IsOnFloor() {
		return true;
	}

}


public class MainApp : Foo {


	[NativeState("IsOnFloor")]
	private readonly State Grounded = null;


	public MainApp() {
		this.SetupNativeStates();
	}


	private bool Foo => !IsOnFloor();


	private static void Main(string[] args) {
		var ma = new MainApp();


		WriteLine($"result: {ma.Grounded}");
	}

}
