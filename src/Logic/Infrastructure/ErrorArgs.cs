using System;

namespace SubtitlesLearn.Logic.Infrastructure
{
	/// <summary>
	/// Args of error of the system error.
	/// </summary>
	public class ErrorArgs : EventArgs
    {
		/// <summary>
		/// Error message.
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// Error details. (usually stack trace).
		/// </summary>
		public string Details { get; set; }

		/// <summary>
		/// Main constructor.
		/// </summary>
		/// <param name="customerId"></param>
		/// <param name="percentCompleted"></param>
		public ErrorArgs(string message, string details)
		{
			Message = message;
			Details = details;
		}
    }
}
