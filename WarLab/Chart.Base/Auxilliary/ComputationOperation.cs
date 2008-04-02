using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using ScientificStudio.Charting.GraphicalObjects;
using System.Windows;

namespace ScientificStudio.Charting.Auxilliary {
	public delegate object ComputationCallback(IAbortable abortable);
	
	public sealed class ComputationOperation : DispatcherObject {

		internal ComputationOperation(ComputationCallback method,
			GraphicalObject graph, IAbortable abortable) {
			this.method = method;
			this.graph = graph;
			this.abortable = abortable;

			status = ComputationStatus.Pending;
		}

		#region Completed event

		private void RaiseCompletedEvent() {
			EventHandler temp = Completed;
			if (temp != null) {
				temp(this, EventArgs.Empty);
			}
		}

		private delegate void Invoker();
		internal void RaiseCompletedEventSyncronized() {
			Dispatcher.BeginInvoke(
				DispatcherPriority.Send,
				(Invoker)RaiseCompletedEvent);
		}

		public event EventHandler Completed;

		#endregion

		#region AbortedEvent

		private void RaiseAbortedEvent() {
			EventHandler temp = Aborted;
			if (temp != null) {
				temp(null, EventArgs.Empty);
			}
		}

		internal void RaiseAbortedEventSyncronized() {
			Dispatcher.BeginInvoke(
				DispatcherPriority.Normal,
				(Invoker)RaiseAbortedEvent);
		}

		public event EventHandler Aborted;

		#endregion

		private readonly GraphicalObject graph;
		internal GraphicalObject Graph {
			get { return graph; }
		}

		private Exception exc = null;
		internal void SetException(Exception exc) {
			this.exc = exc;
		}

		private ComputationStatus status;
		public ComputationStatus ComputationStatus {
			get { return status; }
			internal set { status = value; }
		}

		private readonly ComputationCallback method;
		public ComputationCallback Method {
			get { return method; }
		}

		private readonly IAbortable abortable;
		internal IAbortable State {
			get { return abortable; }
		}

		public void Abort() {
			abortable.BeginAbort();
			status = ComputationStatus.Aborted;
			RaiseAbortedEventSyncronized();
		}

		private object result = null;
		public object Result {
			get {
				if (exc == null) {
					return result;
				}
				else {
					throw exc;
				}
			}
			internal set { result = value; }
		}
	}
}
