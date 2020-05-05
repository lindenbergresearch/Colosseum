using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;

namespace Renoir {

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
				get => pool.All(e => e.Running);//???
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
			/// Execution delegate
			/// </summary>
			public delegate object ExecDelegate(object param);


			/// <summary>
			/// </summary>
			/// <param name="param"></param>
			public delegate void ExecDelegateVoid(object param);


			/// <summary>
			/// </summary>
			public ExecDelegate _ExecDelegate { get; set; }
			public ExecDelegateVoid _ExecDelegateVoid { get; set; }


			/// <summary>
			/// Constructor with the execution delegate.
			/// </summary>
			/// <param name="execDelegate"></param>
			public Executor(ExecDelegate execDelegate) {
				_ExecDelegate = execDelegate;
				State = ExecState.Ready;
			}


			/// <summary>
			/// Constructor with the execution delegate.
			/// </summary>
			/// <param name="execDelegateVoid"></param>
			public Executor(ExecDelegateVoid execDelegateVoid) {
				_ExecDelegateVoid = execDelegateVoid;
				State = ExecState.Ready;
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
		}


		/// <summary>
		/// </summary>
		public class PlainExecutor : Executor {


			/// <inheritdoc />
			public PlainExecutor(ExecDelegate execDelegate) : base(execDelegate) { }


			/// <inheritdoc />
			public PlainExecutor(ExecDelegateVoid execDelegateVoid) : base(execDelegateVoid) { }


			/// <inheritdoc />
			public PlainExecutor() { }


			#region Overrides of Executor

			/// <inheritdoc />
			public override void Run(object param = null) {
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
				}

				Time = sw.Elapsed;
				State = ExecState.Succeeded;
			}

			#endregion

		}


		/// <summary>
		/// </summary>
		public class BackgroundExecutor : Executor {


			/// <inheritdoc />
			public BackgroundExecutor(ExecDelegate execDelegate) : base(execDelegate) { }


			/// <inheritdoc />
			public BackgroundExecutor(ExecDelegateVoid execDelegateVoid) : base(execDelegateVoid) { }


			/// <inheritdoc />
			public BackgroundExecutor() { }


			/// <summary>
			/// </summary>
			private Thread ExecutorThread;


			/// <summary>
			/// </summary>
			/// <param name="param"></param>
			private void _Run(object param) {
				try {
					sw.Start();

					if (_ExecDelegate == null) _ExecDelegateVoid(param);
					else Result = _ExecDelegate(param);

					sw.Stop();
				} catch (Exception e) {
					State = ExecState.Failed;
					Result = e;
				}

				Time = sw.Elapsed;
				State = ExecState.Succeeded;
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

}
