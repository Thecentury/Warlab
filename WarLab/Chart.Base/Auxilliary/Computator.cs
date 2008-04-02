using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ScientificStudio.Charting.GraphicalObjects;

namespace ScientificStudio.Charting.Auxilliary {
	public sealed class Computator {
		private static ComputationOperation[] runningOps;
		private static Thread[] threads;
		private static AutoResetEvent[] semaphores;

		static Computator() {
			int num = Environment.ProcessorCount;
			runningOps = new ComputationOperation[num];
			semaphores = new AutoResetEvent[num];

			threads = new Thread[num];
			for (int i = 0; i < num; i++) {
				threads[i] = new Thread(ThreadProc);
				threads[i].IsBackground = true;
				threads[i].Name = "Computator thread #" + i;
				threads[i].Priority = ThreadPriority.Normal;

				semaphores[i] = new AutoResetEvent(false);
			}
			// here we can possibly decrease priority of ine of threads not to interfere with 
			// threads of WPF and WinForms.
			threads[0].Priority = ThreadPriority.BelowNormal;
		}

		private static object locker = new Object();
		private static List<ComputationOperation> pendingOps = new List<ComputationOperation>(20);

		[System.Diagnostics.Conditional("DEBUG")]
		private static void WriteLog(string message) {
			string color = "DarkGreen";
			string name = Thread.CurrentThread.Name;
			if (name == "Computator thread #0")
				color = "Red";
			else if (name == "Computator thread #1")
				color = "Blue";

			string logMessage = "";
			if (message != "")
				logMessage = name + ": " + message;

			System.Diagnostics.Debug.WriteLine(logMessage, color);
		}

		private static void ThreadProc() {
			int selfIndex = Array.IndexOf<Thread>(threads, Thread.CurrentThread);
			WriteLog("Started - " + selfIndex);
			while (true) {
				ComputationOperation op = null;
				object result = null;
				try {
					lock (locker) {
						// trying to get operation, added in AddOperation method.
						op = runningOps[selfIndex];
						if (op != null) {
							WriteLog("Taken operation from running ops");
						}

						// if no operation was set by AddOperation method,
						// try get operation from pending operations.
						if (op == null && pendingOps.Count > 0) {
							// choosing lastly added operation
							int index = pendingOps.Count - 1;

							op = pendingOps[index];
							pendingOps.RemoveAt(index);

							runningOps[selfIndex] = op;

							WriteLog("Taken operation from queue");
						}
					}
					if (op == null) {
						WriteLog("Waiting for semaphore...");
						semaphores[selfIndex].WaitOne();
						// after waiting, thread will be released in AddOperation method,
						// where all necessary preparations will be done
						lock (locker) {
							op = runningOps[selfIndex];
						}
						if (op == null) {
							WriteLog("**************** Null operation!");
							continue;
						}
					}
					// here we have smth to execute in any case

					WriteLog("Beginning computation");
					if (!op.State.AbortPending) {
						op.ComputationStatus = ComputationStatus.Executing;
						result = op.Method(op.State);
					}
				}
				catch (ThreadAbortException) {
					break;
					throw;
				}
				// any other exception, not ThreadAbortException =>
				// it was thrown in executing method.
				catch (Exception exc) {
					WriteLog("Exception was thrown: " + exc.Message);
					op.SetException(exc);
				}
				// we've got a result, if method really returned a result,
				// or there was thrown exception.
				// we've not got a result, if executing was aborted.
				if (!op.State.AbortPending) {
					op.Result = result;
					op.ComputationStatus = ComputationStatus.Completed;
					op.RaiseCompletedEventSyncronized();
					WriteLog("Job completed");
				}
				else {
					WriteLog("Job aborted");
				}

				lock (locker) {
					runningOps[selfIndex] = null;
				}
			}
		}

		private static void AbortOperations(GraphicalObject graph) {
			lock (locker) {

				int i = 0;
				// aborting pending operations
				while (i < pendingOps.Count) {
					ComputationOperation operation = pendingOps[i];
					if (operation.Graph == graph) {
						pendingOps.RemoveAt(i);
						operation.Abort();
					}
					else {
						i++;
					}
				}

				for (i = 0; i < runningOps.Length; i++) {
					if (runningOps[i] != null && runningOps[i].Graph == graph) {
						semaphores[i].Reset();
						runningOps[i].Abort();
					}
				}
			}
		}

		private void AddOperation(ComputationOperation op) {
			lock (locker) {
				WriteLog("Operation was added");
				WriteLog("Pending operations: " + pendingOps.Count);

				if (pendingOps.Count == 0) {
					for (int i = 0; i < threads.Length; i++) {
						// thread is not working
						if (runningOps[i] == null) {

							runningOps[i] = op;

							ThreadState state = threads[i].ThreadState;
							// thread number 'i' was unstarted -> starting it
							if ((state | ThreadState.Unstarted) == state) {
								threads[i].Start();
							}
							else {
								// unblocking thread
								WriteLog("Unblocking thread #" + i);
								semaphores[i].Set();
							}
							return;
						}
					}
				}
				// all threads are busy
				pendingOps.Add(op);
				op.ComputationStatus = ComputationStatus.Pending;
			}
		}

		private readonly GraphicalObject graph;
		// todo make ctor internal
#if DEBUG
		public
#else
		internal
#endif
 Computator(GraphicalObject graph) {
			if (graph == null)
				throw new ArgumentNullException("graph");

			this.graph = graph;
		}

		public void AbortAllOperations() {
			AbortOperations(graph);
		}

		public ComputationOperation BeginInvoke(ComputationCallback method, IAbortable abortable) {
			ComputationOperation op = new ComputationOperation(method, graph, abortable);
			AddOperation(op);
			return op;
		}

		public ComputationOperation BeginInvoke(ComputationCallback method, EventHandler completedCallback, IAbortable abortable) {
			ComputationOperation op = new ComputationOperation(method, graph, abortable);
			op.Completed += completedCallback;
			AddOperation(op);
			return op;
		}
	}
}
