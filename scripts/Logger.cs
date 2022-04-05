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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using static System.Console;
using static Renoir.Logger.LoggerLevel;
using File = System.IO.File;

#endregion

namespace Renoir {

	/// <summary>
	///     I/O Interface for log messages.
	/// </summary>
	public interface ILogWriter {
		/// <summary>
		///     Simple interface method for string output.
		/// </summary>
		/// <param name="level"></param>
		/// <param name="message"></param>
		void Write(Logger.LoggerLevel level, string message);
	}
	/// <summary>
	///     Helper class for qualified log output
	/// </summary>
	public static class Logger {
		/// <summary>
		///     Available log levels.
		/// </summary>
		public enum LoggerLevel {
			TRACE,
			DEBUG,
			INFO,
			WARN,
			ERROR,
			FATAL
		}

		public const int MAX_BUFFER_LINES = 200;

		/// <summary>
		///     Holds all pushed messages.
		/// </summary>
		public static List<string> messages = new(MAX_BUFFER_LINES);


		/// <summary>
		///     Init logger
		/// </summary>
		static Logger() {
			LogLevel = DEBUG;

			LogWriters.Add(new GodotConsoleLogWriter());
			//LogWriters.Add(new SystemConsoleLogWriter());
			//LogWriters.Add(new FileLogWriter());

			log(INFO, $"Logger started on: {DateTime.Now}");
		}


		/// <summary>
		///     Shorthand flags as functional bool properties
		/// </summary>
		public static bool IsTrace => LogLevel == TRACE;
		public static bool IsDebug => LogLevel == DEBUG;
		public static bool IsInfo => LogLevel == INFO;
		public static bool IsWarn => LogLevel == WARN;
		public static bool IsError => LogLevel == ERROR;
		public static bool IsFatal => LogLevel == FATAL;


		/// <summary>
		///     Enable/Disable debug messages
		/// </summary>
		public static bool PrintDebug { get; set; } = true;

		/// <summary>
		///     Current loglevel
		/// </summary>
		public static LoggerLevel LogLevel { get; set; }

		/// <summary>
		///     Holds all output writers
		/// </summary>
		public static List<ILogWriter> LogWriters { get; set; } = new();


		/// <summary>
		///     Remove all messages.
		/// </summary>
		public static void ResetMessageBuffer() {
			messages.Clear();
		}

		private static void CheckBufferOverflow() {
			if (messages.Count >= MAX_BUFFER_LINES) {
				messages.RemoveRange(0, MAX_BUFFER_LINES / 10);
			}
		}


		/// <summary>
		///     Print debug log message to GD console
		/// </summary>
		/// <param name="level"></param>
		/// <param name="msg"></param>
		private static void log(LoggerLevel level, string msg) {
			var st = new StackTrace(true);
			var sf = st.GetFrame(2);

			var filename = sf.GetFileName()?.Split('/').Last();
			var lineno = sf.GetFileLineNumber();
			var method = sf.GetMethod().Name;


			var dt = DateTime.Now.ToString("HH:mm:ss.fff");


			var m = $"[{dt}] {level} ";

			if (lineno > 0) m += $"({filename}:{lineno} {method}) {msg}";
			else m += $"({method}) {msg}";

			LogWriters.Each(writer => writer.Write(level, m));

			CheckBufferOverflow();
			messages.Add(m);
		}


		/// <summary>
		/// </summary>
		/// <param name="message"></param>
		public static void trace(string message) {
			if (LogLevel > TRACE) return;
			log(TRACE, message);
		}


		/// <summary>
		/// </summary>
		/// <param name="message"></param>
		public static void debug(string message) {
			if (LogLevel > DEBUG) return;
			log(DEBUG, message);
		}


		/// <summary>
		/// </summary>
		/// <param name="message"></param>
		public static void info(string message) {
			if (LogLevel > INFO) return;
			log(INFO, message);
		}


		/// <summary>
		/// </summary>
		/// <param name="message"></param>
		public static void warn(string message) {
			if (LogLevel > WARN) return;
			log(WARN, message);
		}


		/// <summary>
		/// </summary>
		/// <param name="message"></param>
		public static void error(string message) {
			if (LogLevel > ERROR) return;
			log(ERROR, message);
		}


		/// <summary>
		/// </summary>
		/// <param name="message"></param>
		public static void fatal(string message) {
			if (LogLevel > FATAL) return;
			log(FATAL, message);
		}
	}
	/// <summary>
	///     Standard Godot console log-writer
	/// </summary>
	public class GodotConsoleLogWriter : ILogWriter {
		/// <summary>
		///     Write to Godot console.
		/// </summary>
		/// <param name="level"></param>
		/// <param name="message"></param>
		public void Write(Logger.LoggerLevel level, string message) {
			if (level == ERROR || level == FATAL)
				GD.PrintErr(message);
			else
				GD.Print(message);
		}
	}
	/// <summary>
	///     System console log-writer
	/// </summary>
	public class SystemConsoleLogWriter : ILogWriter {
		/// <summary>
		///     Write to system console.
		/// </summary>
		/// <param name="level"></param>
		/// <param name="message"></param>
		public void Write(Logger.LoggerLevel level, string message) {
			if (level == ERROR || level == FATAL)
				Console.Error.WriteLine(message);
			else
				Out.WriteLine(message);
		}
	}
	/// <summary>
	///     File log write
	///     Quick and diry
	/// </summary>
	public class FileLogWriter : ILogWriter {
		private bool active = true;


		private string GetFileName
			=> $"log/renoir_{DateTime.Now.Date:yy-MM-dd}.log";


		/// <summary>
		///     Write to logfile
		/// </summary>
		/// <param name="level"></param>
		/// <param name="message"></param>
		public void Write(Logger.LoggerLevel level, string message) {
			if (!active) return;

			try {
				File.AppendAllText(GetFileName, message + '\n');
			} catch (Exception e) {
				WriteLine($"Unable to write log to file: {GetFileName}");
				active = false;
			}
		}
	}

}
