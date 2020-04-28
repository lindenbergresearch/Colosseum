using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using static System.Console;
using static Renoir.Logger.LogLevel;
using File = System.IO.File;

namespace Renoir {

	/// <summary>
	/// I/O Interface for log messages.
	/// </summary>
	public interface ILogWriter {

		/// <summary>
		/// Simple interface method for string output.
		/// </summary>
		/// <param name="level"></param>
		/// <param name="message"></param>
		void Write(Logger.LogLevel level, string message);
	}


	/// <summary>
	/// Helper class for qualified log output
	/// </summary>
	public static class Logger {

		/// <summary>
		/// Available log levels.
		/// </summary>
		public enum LogLevel {
			TRACE,
			DEBUG,
			INFO,
			WARN,
			ERROR,
			FATAL
		}


		/// <summary>
		/// Shorthand flags as functional bool properties
		/// </summary>
		public static bool IsTrace => Level == TRACE;
		public static bool IsDebug => Level == DEBUG;
		public static bool IsInfo => Level == INFO;
		public static bool IsWarn => Level == WARN;
		public static bool IsError => Level == ERROR;
		public static bool IsFatal => Level == FATAL;


		/// <summary>
		/// Holds all pushed messages.
		/// </summary>
		public static List<string> messages = new List<string>(4096);


		/// <summary>
		/// Enable/Disable debug messages
		/// </summary>
		public static bool PrintDebug { get; set; } = true;

		/// <summary>
		/// Current loglevel
		/// </summary>
		public static LogLevel Level { get; set; } = DEBUG;

		/// <summary>
		/// Holds all output writers
		/// </summary>
		public static List<ILogWriter> LogWriters { get; set; } = new List<ILogWriter>();


		/// <summary>
		/// Remove all messages.
		/// </summary>
		public static void ResetMessageBuffer() {
			messages.Clear();
		}


		/// <summary>
		/// Print debug log message to GD console
		/// </summary>
		/// <param name="msg"></param>
		private static void log(LogLevel level, string msg) {
			var st = new StackTrace(true);
			var sf = st.GetFrame(2);

			var filename = sf.GetFileName()?.Split('/').Last();
			var lineno = sf.GetFileLineNumber();
			var method = sf.GetMethod().Name;


			var dt = DateTime.Now.ToString("HH:mm:ss.fff");
			var m = $"[{dt}] {level} <{filename}:{lineno} {method}()> {msg}";


			//TODO: find a better solution to add the standard logger
			if (LogWriters.Count == 0) {
				//LogWriters.Add(new GodotConsoleLogWriter());

				LogWriters.Add(new SystemConsoleLogWriter());
				LogWriters.Add(new FileLogWriter());

				foreach (var writer in LogWriters) writer.Write(INFO, $"--- INIT LOGGER {DateTime.Now} ---");
			}

			foreach (var writer in LogWriters) writer.Write(level, m);

			messages.Add(m);
		}


		/// <summary>
		/// </summary>
		/// <param name="message"></param>
		public static void trace(string message) {
			if (Level > TRACE) return;
			log(TRACE, message);
		}


		/// <summary>
		/// </summary>
		/// <param name="message"></param>
		public static void debug(string message) {
			if (Level > DEBUG) return;
			log(DEBUG, message);
		}


		/// <summary>
		/// </summary>
		/// <param name="message"></param>
		public static void info(string message) {
			if (Level > INFO) return;
			log(INFO, message);
		}


		/// <summary>
		/// </summary>
		/// <param name="message"></param>
		public static void warn(string message) {
			if (Level > WARN) return;
			log(WARN, message);
		}


		/// <summary>
		/// </summary>
		/// <param name="message"></param>
		public static void error(string message) {
			if (Level > ERROR) return;
			log(ERROR, message);
		}


		/// <summary>
		/// </summary>
		/// <param name="message"></param>
		public static void fatal(string message) {
			if (Level > FATAL) return;
			log(FATAL, message);
		}
	}


	/// <summary>
	/// Standard Godot console log-writer
	/// </summary>
	public class GodotConsoleLogWriter : ILogWriter {

		/// <summary>
		/// Write to Godot console.
		/// </summary>
		/// <param name="level"></param>
		/// <param name="message"></param>
		public void Write(Logger.LogLevel level, string message) {
			if (level == ERROR || level == FATAL)
				GD.PrintErr(message);
			else
				GD.Print(message);
		}
	}


	/// <summary>
	/// System console log-writer
	/// </summary>
	public class SystemConsoleLogWriter : ILogWriter {

		/// <summary>
		/// Write to system console.
		/// </summary>
		/// <param name="level"></param>
		/// <param name="message"></param>
		public void Write(Logger.LogLevel level, string message) {
			if (level == ERROR || level == FATAL)
				Console.Error.WriteLine(message);
			else
				Out.WriteLine(message);
		}
	}


	/// <summary>
	/// File log write
	/// Quick and diry
	/// </summary>
	public class FileLogWriter : ILogWriter {

		private bool active = true;


		/// <summary>
		/// Write to logfile
		/// </summary>
		/// <param name="level"></param>
		/// <param name="message"></param>
		public void Write(Logger.LogLevel level, string message) {
			if (!active) return;

			try {
				File.AppendAllText(GetFileName, message + '\n');
			} catch (Exception e) {
				WriteLine($"Unable to write log to file: {GetFileName}");
				active = false;
			}
		}


		private string GetFileName
			=> $"log/renoir_{DateTime.Now.Date:yy-MM-dd}.log";

	}

}
