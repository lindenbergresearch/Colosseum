using Godot;

namespace Renoir {


	/// <summary>
	/// Property class:
	/// Holds all properties for the current level
	/// </summary>
	public static class Level {

		[Register("Main.Level.Name")]
		public static string Name { get; set; }

		[Register("Main.Level.Gravity")]
		public static Vector2 Gravity { get; set; }


		static Level() {
			PropertyExtensions.SetupGlobalProperties(typeof(Level));
		}

	}

}
