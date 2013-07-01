using System;
using System.Collections.Generic;
using System.Text;

namespace Pistachio
{
	class ProgressEventArgs : EventArgs
	{
		#region Properties
		
		private double _totalProgress;

		public double TotalProgress
		{
			get { return _totalProgress; }
			set { _totalProgress = value; }
		}

		private double _currentProgress;

		public double CurrentProgress
		{
			get { return _currentProgress; }
			set { _currentProgress = value; }
		}

		private int _totalFileCount;

		public int TotalFileCount
		{
			get { return _totalFileCount; }
			set { _totalFileCount = value; }
		}

		private int _totalFilesComplete;

		public int TotalFilesComplete
		{
			get { return _totalFilesComplete; }
			set { _totalFilesComplete = value; }
		}

		private int _currentFileCount;

		public int CurrentFileCount
		{
			get { return _currentFileCount; }
			set { _currentFileCount = value; }
		}

		private int _currentFilesComplete;

		public int CurrentFilesComplete
		{
			get { return _currentFilesComplete; }
			set { _currentFilesComplete = value; }
		}

		private string _notesOnTotal;

		public string NotesOnTotal
		{
			get { return _notesOnTotal; }
			set { _notesOnTotal = value; }
		}
		private string _notesOnCurrent;

		public string NotesOnCurrent
		{
			get { return _notesOnCurrent; }
			set { _notesOnCurrent = value; }
		}

		private int _totalCurrentImageInAllSets;
		public int TotalCurrentImageInAllSets
		{
			get { return _totalCurrentImageInAllSets; }
			set { _totalCurrentImageInAllSets = value; }
		}

		private int _totalImagesInAllSets;
		public int TotalImagesInAllSets
		{
			get { return _totalImagesInAllSets; }
			set { _totalImagesInAllSets = value; }
		}

		#endregion Properties

		#region Constructor
		/// <summary>
		/// This Method Sends information to the Dispatcher about our current progress
		/// </summary>
		/// <param name="totalProgress">The percentage of the total progress (ie. currentSet/totalSets )</param>
		/// <param name="currentProgress">The percentage of the current operation's progress (ie. currentPhoto/totalPhoto )</param>
		/// <param name="totalFileCount">The number of Files to be processed</param>
		/// <param name="totalFilesComplete">The number of Files already completed</param>
		/// <param name="currentFileCount"></param>
		/// <param name="currentFilesComplete"></param>
		/// <param name="notesOnTotal"></param>
		/// <param name="notesOnCurrent"></param>
		public ProgressEventArgs(double totalProgress, double currentProgress, int totalFileCount, int totalFilesComplete,
						int currentFileCount, int currentFilesComplete, string notesOnTotal, string notesOnCurrent, int totalCurrentImageInAllSets,  int totalImagesInAllSets)
		{
			this.TotalProgress = totalProgress;
			this.CurrentProgress = currentProgress;
			this.TotalFileCount = totalFileCount;
			this.TotalFilesComplete = totalFilesComplete;
			this.CurrentFileCount = currentFileCount;
			this.CurrentFilesComplete = currentFilesComplete;
			this.NotesOnTotal = notesOnTotal;
			this.NotesOnCurrent = notesOnCurrent;
			this.TotalCurrentImageInAllSets = totalCurrentImageInAllSets;
			this.TotalImagesInAllSets = totalImagesInAllSets;
		}

		public ProgressEventArgs(string notesOnTotal, string notesOnCurrent)
		{
			this.NotesOnTotal = notesOnTotal;
			this.NotesOnCurrent = notesOnCurrent;
		}
		#endregion Constructor
	}
}
