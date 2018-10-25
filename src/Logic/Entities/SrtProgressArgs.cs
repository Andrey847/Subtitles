using System;

namespace SubtitlesLearn.Logic.Entities
{
	/// <summary>
	/// Args of srt upload progress.
	/// </summary>
	public class SrtProgressArgs : EventArgs
    {
		/// <summary>
		/// Customer, who performs srt upload.
		/// </summary>
		public int CustomerId { get; set; }

		/// <summary>
		/// How many percent completed.
		/// </summary>
		public int PercentCompleted { get; set; }

		/// <summary>
		/// Main constructor.
		/// </summary>
		/// <param name="customerId"></param>
		/// <param name="percentCompleted"></param>
		public SrtProgressArgs(int customerId, int percentCompleted)
		{
			CustomerId = customerId;
			PercentCompleted = percentCompleted;
		}
    }
}
