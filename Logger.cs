using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

/// <summary>
/// Interface to output write for log messages.
/// </summary>
public interface ILogWriter {
	/// <summary>
	/// Simple interface method for string output.
	/// </summary>
	/// <param name="message"></param>
	void Write(String message);
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
	/// Enable/Disable debug messages
	/// </summary>
	public static bool PrintDebug { get; set; } = true;

	/// <summary>
	/// Current loglevel
	/// </summary>
	public static LogLevel Level { get; set; } = LogLevel.DEBUG;

	/// <summary>
	/// Holds all pushed messages.
	/// </summary>
	public static List<string> messages = new List<string>();

	/// <summary>
	/// Holds all output writers
	/// </summary>
	public static List<ILogWriter> LogWriters { get; set; } = new List<ILogWriter>();


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
		var m = $"[{dt}] DEBUG <{filename}:{lineno} {method}()> {msg}";
		//GD.Print(m);

		foreach (var writer in LogWriters) {
			writer.Write(m);
		}

		messages.Add(m);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="message"></param>
	public static void trace(string message) {
		if (Level > LogLevel.TRACE) return;
		log(Level, message);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="message"></param>
	public static void debug(string message) {
		if (Level > LogLevel.DEBUG) return;
		log(Level, message);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="message"></param>
	public static void info(string message) {
		if (Level > LogLevel.INFO) return;
		log(Level, message);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="message"></param>
	public static void warn(string message) {
		if (Level > LogLevel.WARN) return;
		log(Level, message);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="message"></param>
	public static void error(string message) {
		if (Level > LogLevel.ERROR) return;
		log(Level, message);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="message"></param>
	public static void fatal(string message) {
		if (Level > LogLevel.FATAL) return;
		log(Level, message);
	}
}
