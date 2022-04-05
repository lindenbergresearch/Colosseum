#region header

// 
//    _____
//   (, /   )            ,
//     /__ /  _ __   ___   __
//  ) /   \__(/_/ (_(_)_(_/ (_  CORE LIBRARY
// (_/ ______________________________________/
// 
// 
// Renoir Core Library for the Godot Game-Engine.
// Copyright 2020-2022 by Lindenberg Research.
// 
// www.lindenberg-research.com
// www.godotengine.org
// 

#endregion

#region

using System;

#endregion

namespace Renoir {

	/// <summary>
	/// </summary>
	public static class Initializer {
		/// <summary>
		///     List of static initializers
		/// </summary>
		public static readonly Type[] staticInitList = {
			typeof(Logger),
			typeof(MainApp),
			typeof(Level)
		};


		/// <summary>
		///     Static initialisation
		/// </summary>
		static Initializer() {
			Logger.info("performing static module initialisation...");
			staticInitList.Each(
				type => RunInit(type)
			);
		}


		/// <summary>
		///     Instance initialisation
		/// </summary>
		/// <param name="obj"></param>
		public static void Init(this object obj) {
			Logger.info($"init object: {obj}");
			DynamicBindings.InitNodeBindings(obj);
		}


		/// <summary>
		///     Examine type and run all init methods
		/// </summary>
		/// <param name="type"></param>
		/// <param name="methodPrefix"></param>
		private static void RunInit(Type type, string methodPrefix = "Init") {
			foreach (var methodInfo in type.GetMethods()) {
				if (!methodInfo.Name.StartsWith(methodPrefix) || methodInfo.GetParameters().Length != 0) continue;
				Logger.info($"[init: {type.FullName} => {methodInfo.Name}]");
				methodInfo.Invoke(null, new object[] { });
			}
		}
	}

}
