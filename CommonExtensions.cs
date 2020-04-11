﻿using System;
using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;
using File = System.IO.File;

namespace Renoir {

	/// <summary>
	///     Extension methods for numeric stuff
	/// </summary>
	public static class RMath {

		/// <summary>
		///     Test for number exceeding a special value
		/// </summary>
		/// <param name="this"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static bool Exceeds<T>(this T @this, T other) where T : IComparable<T>
			=> @this.CompareTo(other) > 0;


		/// <summary>
		/// 
		/// </summary>
		/// <param name="this"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static short Abs(this short @this)
			=> Math.Abs(@this);


		/// <summary>
		/// 
		/// </summary>
		/// <param name="this"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static int Abs(this int @this)
			=> Math.Abs(@this);


		/// <summary>
		/// 
		/// </summary>
		/// <param name="this"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static long Abs(this long @this)
			=> Math.Abs(@this);


		/// <summary>
		/// 
		/// </summary>
		/// <param name="this"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static float Abs(this float @this)
			=> Math.Abs(@this);


		/// <summary>
		/// 
		/// </summary>
		/// <param name="this"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static double Abs(this double @this)
			=> Math.Abs(@this);


		/// <summary>
		/// Test if value is in range.
		/// </summary>
		/// <param name="this"></param>
		/// <param name="start">Start value</param>
		/// <param name="end">End value</param>
		/// <param name="boundary">Include start and end value in test</param>
		/// <returns></returns>
		public static bool InsideRange(this float @this, float start, float end, bool boundary = true)
			=> boundary ? (@this >= start && @this <= end) : (@this > start && @this < end);


		public static bool InsideRange(this double @this, double start, double end, bool boundary = true)
			=> boundary ? (@this >= start && @this <= end) : (@this > start && @this < end);


		/// <summary>
		/// Test if value is in symmetrical range.
		/// </summary>
		/// <param name="this"></param>
		/// <param name="epsilon">+/- distance</param>
		/// <param name="boundary">Include boundary values</param>
		/// <param name="origin">Origin value</param>
		/// <returns></returns>
		public static bool InsideRange(this float @this, float epsilon, bool boundary = true, float origin = 0.0f)
			=> @this.InsideRange(-(epsilon + origin), epsilon + origin, boundary);


		/// <summary>
		/// Round to integer
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public static int Round(this float x)
			=> Mathf.RoundToInt(x);


		public static int Round(this double x)
			=> Mathf.RoundToInt((float) x);


		/// <summary>
		/// Returns the vectors direction as unicode arrow
		/// </summary>
		/// <param name="vector2"></param>
		/// <returns></returns>
		public static string ToDirectionArrow(this Vector2 vector2) {
			var direction = "<odd>";
			if (vector2 == Vector2.Down) direction = "↑";
			if (vector2 == Vector2.Up) direction = "↓";
			if (vector2 == Vector2.Left) direction = "→";
			if (vector2 == Vector2.Right) direction = "←";

			return direction;
		}
	}


	/// <summary>
	///     String extensions
	/// </summary>
	public static class RString {

		/// <summary>
		///     Directly write a string to a file
		/// </summary>
		/// <param name="text">The string</param>
		/// <param name="filePath">The full path of the target filename</param>
		public static void ToTextFile(this string text, string filePath) {
			File.WriteAllText(filePath, text);
		}
	}


	/// <summary>
	///     Object extentions
	/// </summary>
	public static class RObject {

		/// <summary>
		///     Converts the current data of an object to a JSON string
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string ToJson(this object obj) {
			return JsonConvert.SerializeObject(obj);
		}


		/// <summary>
		/// Dump the object's content to string 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string Dump(this object obj)
			=> Util.Dump(obj);
	}


	/// <summary>
	/// Godot classes extensions
	/// </summary>
	public static class RGodot {


		/// <summary>
		/// 	Return all collider since last movement.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<KinematicCollision2D> GetCollider(this KinematicBody2D kinematicBody2D) {
			var collider = new List<KinematicCollision2D>();

			for (var i = 0; i < kinematicBody2D.GetSlideCount(); i++) {
				var coll = kinematicBody2D.GetSlideCollision(i);
				collider.Add(coll);
			}

			return collider;
		}
	}

}
