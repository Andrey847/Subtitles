using System;
using System.Collections.Generic;
using System.Text;

namespace SubtitlesLearn.Logic
{
	/// <summary>
	/// Performs aux transcribes operations.
	/// </summary>
	public class TranscribeManager
	{
		#region Singleton

		private static Lazy<TranscribeManager> _instance = new Lazy<TranscribeManager>(() => new TranscribeManager());

		/// <summary>
		/// Main class instance.
		/// </summary>
		public static TranscribeManager Instance => _instance.Value;

		/// <summary>
		/// Private constructor (single).
		/// </summary>
		private TranscribeManager()
		{

		}

		#endregion Singleton

		#region Methods

		#endregion Methods
	}
}
