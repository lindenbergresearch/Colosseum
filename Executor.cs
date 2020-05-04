using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading;

namespace Renoir {

	namespace Renoir {


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
			public enum ExecState {
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


			/// <summary>
			/// Execution delegate
			/// </summary>
			public delegate object ExecDelegate(object param);


			/// <summary>
			/// </summary>
			public ExecDelegate _ExecDelegate { get; set; }


			/// <summary>
			/// Constructor with the execution delegate.
			/// </summary>
			/// <param name="execDelegate"></param>
			public Executor(ExecDelegate execDelegate) {
				_ExecDelegate = execDelegate;
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
			public PlainExecutor() { }


			#region Overrides of Executor

			/// <inheritdoc />
			public override void Run(object param = null) {
				if (_ExecDelegate == null)
					throw new ExecutorException("Delegate not setup! Nothing to execute.");

				State = ExecState.Running;

				try {
					sw.Start();
					Result = _ExecDelegate(param);
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
					Result = _ExecDelegate(param);
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
				if (_ExecDelegate == null)
					throw new ExecutorException("Delegate not setup! Nothing to execute.");

				State = ExecState.Running;
				ExecutorThread ??= new Thread(_Run);
				ExecutorThread.Start(param);
			}

			#endregion


			/// <summary>
			/// Pause the current thread until the execution is done
			/// </summary>
			public void WaitFor() {
				while (State == ExecState.Running) Thread.Sleep(0);
			}

		}

	}

}
