using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;

namespace Renoir {


	/// <summary>
	/// Helper class for managed Executors.
	/// </summary>
	public static class ExecutorPool {

		/// <summary>
		/// Executor pool
		/// </summary>
		private static List<Executor> pool;

		/// <summary>
		/// Check if all executors succeeded.
		/// </summary>
		public static bool AllOk {
			get => pool.All(e => e.Succeeded);
		}

		/// <summary>
		/// 
		/// </summary>
		public static bool Running {
			get => pool.All(e => e.Running); //???
		}


		/// <summary>
		/// Run all 
		/// </summary>
		public static void RunAll()
			=> pool.Each(e => e.Run());


		/// <summary>
		/// Init
		/// </summary>
		static ExecutorPool() {
			pool = new List<Executor>();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="background"></param>
		/// <param name="_execDelegate"></param>
		public static void Add(string name, bool background, Executor.ExecDelegate _execDelegate) {
			Executor ex; // = background ? new BackgroundExecutor(_execDelegate, name) : new PlainExecutor(_execDelegate, name);

			if (background) ex = new BackgroundExecutor(_execDelegate, name);
			else ex = new PlainExecutor(_execDelegate, name);

			pool.Add(ex);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="background"></param>
		/// <param name="_execDelegate"></param>
		public static void Add(string name, bool background, Executor.ExecDelegateVoid _execDelegate) {
			Executor ex; // = background ? new BackgroundExecutor(_execDelegate, name) : new PlainExecutor(_execDelegate, name);

			if (background) ex = new BackgroundExecutor(_execDelegate, name);
			else ex = new PlainExecutor(_execDelegate, name);

			pool.Add(ex);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="ex"></param>
		public static void Add(Executor ex)
			=> pool.Add(ex);
	}


	public class ExecutorException : Exception {

		/// <inheritdoc />
		public ExecutorException() { }


		/// <inheritdoc />
		protected ExecutorException(SerializationInfo info, StreamingContext context) : base(info, context) { }


		/// <inheritdoc />
		public ExecutorException(string message) : base(message) { }


		/// <inheritdoc />
		public ExecutorException(string message, Exception innerException) : base(message, innerException) { }

	}


	/// <summary>
	/// </summary>
	public abstract class Executor {

		/// <summary>
		/// </summary>
		protected enum ExecState {
			Ready,
			Running,
			Failed,
			Succeeded
		}


		/// <summary>
		/// 
		/// </summary>
		private string name = "";

		/// <summary>
		/// The name of the executor
		/// </summary>
		public string Name {
			get => name.Length == 0 ? "untitled" : name;
			set => name = value;
		}

		/// <summary>
		/// </summary>
		public TimeSpan Time;

		protected Stopwatch sw = new Stopwatch();

		public object Result { get; set; }

		/// <summary>
		/// The executors current state
		/// </summary>
		protected ExecState State { get; set; }


		public bool Ready => State == ExecState.Ready;
		public bool Running => State == ExecState.Running;
		public bool Failed => State == ExecState.Failed;
		public bool Succeeded => State == ExecState.Succeeded;


		/// <summary>
		/// Execution delegate with return value
		/// </summary>
		public delegate object ExecDelegate(object param);


		/// <summary>
		/// Execution delegate without return value
		/// </summary>
		/// <param name="param"></param>
		public delegate void ExecDelegateVoid(object param);


		/// <summary>
		/// </summary>
		public ExecDelegate _ExecDelegate { get; set; }
		public ExecDelegateVoid _ExecDelegateVoid { get; set; }


		/// <summary>
		/// Action delegate (no parameter, no return value)
		/// </summary>
		public delegate void EventDelegate(Executor executor);


		/// <summary>
		/// Exception delegate
		/// </summary>
		/// <param name="e"></param>
		public delegate void ExceptionDelegate(Exception e);


		public EventDelegate OnBefore { get; set; }
		public EventDelegate OnAfter { get; set; }
		public ExceptionDelegate OnException { get; set; }


		/// <summary>
		/// Constructor with the execution delegate.
		/// </summary>
		/// <param name="execDelegate"></param>
		/// <param name="name"></param>
		public Executor(ExecDelegate execDelegate, string name = "") {
			_ExecDelegate = execDelegate;
			State = ExecState.Ready;
			Name = name;
		}


		/// <summary>
		/// Constructor with the execution delegate.
		/// </summary>
		/// <param name="execDelegateVoid"></param>
		/// <param name="name"></param>
		public Executor(ExecDelegateVoid execDelegateVoid, string name = "") {
			_ExecDelegateVoid = execDelegateVoid;
			State = ExecState.Ready;
			Name = name;
		}


		/// <summary>
		/// Plain constructor - delegate have to be setup later.
		/// </summary>
		public Executor() {
			State = ExecState.Ready;
		}


		/// <summary>
		/// Execution method to be implemented
		/// </summary>
		/// <param name="param"></param>
		/// <returns></returns>
		public abstract void Run(object param = null);


		#region Overrides of Object

		/// <inheritdoc />
		public override string ToString()
			=> $"{this.GetType().Name}: name={Name} state={State} time={Time}";

		#endregion

	}


	/// <summary>
	/// </summary>
	public class PlainExecutor : Executor {


		/// <inheritdoc />
		public PlainExecutor(ExecDelegate execDelegate, string name = "") : base(execDelegate, name) { }


		/// <inheritdoc />
		public PlainExecutor(ExecDelegateVoid execDelegateVoid, string name = "") : base(execDelegateVoid, name) { }


		/// <inheritdoc />
		public PlainExecutor() { }


		#region Overrides of Executor

		/// <inheritdoc />
		public override void Run(object param = null) {
			OnBefore?.Invoke(this);

			if (_ExecDelegate == null && _ExecDelegateVoid == null)
				throw new ExecutorException("Delegate not setup! Nothing to execute.");

			State = ExecState.Running;

			try {
				sw.Start();

				if (_ExecDelegate == null) _ExecDelegateVoid(param);
				else Result = _ExecDelegate(param);

				sw.Stop();
			} catch (Exception e) {
				State = ExecState.Failed;
				Result = e;
				OnException?.Invoke(e);
			}

			Time = sw.Elapsed;
			State = ExecState.Succeeded;
			OnAfter?.Invoke(this);
		}

		#endregion

	}


	/// <summary>
	/// </summary>
	public class BackgroundExecutor : Executor {


		/// <inheritdoc />
		public BackgroundExecutor(ExecDelegate execDelegate, string name = "") : base(execDelegate, name) { }


		/// <inheritdoc />
		public BackgroundExecutor(ExecDelegateVoid execDelegateVoid, string name = "") : base(execDelegateVoid, name) { }


		/// <inheritdoc />
		public BackgroundExecutor() { }


		/// <summary>
		/// </summary>
		private Thread ExecutorThread;


		/// <summary>
		/// </summary>
		/// <param name="param"></param>
		private void _Run(object param) {
			OnBefore?.Invoke(this);

			try {
				sw.Start();

				if (_ExecDelegate == null) _ExecDelegateVoid(param);
				else Result = _ExecDelegate(param);

				sw.Stop();
			} catch (Exception e) {
				State = ExecState.Failed;
				Result = e;
				OnException?.Invoke(e);
			}

			Time = sw.Elapsed;
			State = ExecState.Succeeded;
			OnAfter?.Invoke(this);
		}


		#region Overrides of Executor

		/// <inheritdoc />
		public override void Run(object param = null) {
			if (_ExecDelegate == null && _ExecDelegateVoid == null)
				throw new ExecutorException("Delegate not setup! Nothing to execute.");

			State = ExecState.Running;
			ExecutorThread ??= new Thread(_Run);
			ExecutorThread.Start(param);
		}

		#endregion


		/// <summary>
		/// Pause the current thread until the execution is done
		/// </summary>
		public object WaitFor() {
			while (State == ExecState.Running)
				Thread.Sleep(0);

			return Result;
		}

	}


}
