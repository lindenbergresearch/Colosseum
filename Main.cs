using System;
using Godot;
using Renoir;
using Renoir;


namespace Renoir {

	/// <summary>
	/// 
	/// </summary>
	public static class Initializer {

		/// <summary>
		/// List of static initializers
		/// </summary>
		public static Type[] staticInitList = {
			typeof(Logger),
			typeof(MainApp)
		};


		/// <summary>
		/// Static initialisation
		/// </summary>
		static Initializer() {
			staticInitList.Each(type => RunInit(type));
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		public static void Init(this object obj) {
			obj.SetupGlobalProperties();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="node"></param>
		public static void Init(this Node node) {
			Action global = () => node.SetupGlobalProperties();
			Action nodes = () => node.SetupNodeBindings();

			ExecutorPool.Add("Global", true, () => node.SetupGlobalProperties());
			
		}


		/// <summary>
		/// Examine type and run all init methods
		/// </summary>
		/// <param name="type"></param>
		/// <param name="methodPrefix"></param>
		private static void RunInit(Type type, string methodPrefix = "Init") {
			foreach (var methodInfo in type.GetMethods()) {
				if (methodInfo.Name.StartsWith(methodPrefix) && methodInfo.GetParameters().Length == 0) {
					Console.WriteLine($"Invoking init method: {methodInfo}");
					methodInfo.Invoke(null, new object[] { });
				}
			}
		}

	}


}


public class MainApp {

	public object ToBeExecuted(object param) {
		Console.WriteLine($"Starting: {param}");

		var k = 1.0;

		for (var i = 0; i < 231234234; i++) k = k * i + Math.Pow(k, i);

		return k;
	}


	private void Start(Executor executor) {
		Console.WriteLine($"Start: {executor}");
	}


	private void Stop(Executor executor) {
		Console.WriteLine($"Stop: {executor}");
	}


	public static void Init() {
		Console.WriteLine("Init MainApp");
	}


	private static void Main(string[] args) {
		var ma = new MainApp();
		var pe = new BackgroundExecutor(ma.ToBeExecuted, "Tester");


		Initializer.InitInstance(ma);

		pe.OnBefore = ma.Start;
		pe.OnAfter = ma.Stop;

		pe.Run();
		pe.WaitFor();


		Console.WriteLine($"Time is: {pe.Time} ");
	}

}
