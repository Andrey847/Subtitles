using SubtitlesLearn.Logic.Dal;
using SubtitlesLearn.Logic.Entities;
using SubtitlesLearn.Logic.Infrastructure;
using System;
using System.Threading.Tasks;

namespace SubtitlesLearn.Logic
{
	/// <summary>
	/// Main system logger.
	/// </summary>
	public class LogManager : ILogging
	{
		#region Events

		/// <summary>
		/// Raised if error (fatal or not) occured.
		/// </summary>
		public event EventHandler<ErrorArgs> Error;

		private void OnError(string message, string details)
		{
			Error?.Invoke(this, new ErrorArgs(message, details));
		}

		#endregion Events

		#region Singleton

		private static Lazy<LogManager> _instance = new Lazy<LogManager>(() => new LogManager());

		/// <summary>
		/// Main class instance.
		/// </summary>
		public static LogManager Instance => _instance.Value;

		/// <summary>
		/// Private constructor (single).
		/// </summary>
		private LogManager()
		{

		}

		#endregion Singleton

		#region Methods

		private async Task Log(string message, string details, LogLevel level)
		{
			if (level == LogLevel.Error || level == LogLevel.FatalError)
			{
				OnError(message, details);
			}

			await DbAccess.LogAsync(message, details, DateTime.Now, level);
		}

		/// <summary>
		/// Logs debug entry.
		/// </summary>
		public async Task LogDebug(string message, string details = null)
		{
			await Log(message, details, LogLevel.Debug);
		}

		/// <summary>
		/// Logs info log message.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="details"></param>
		/// <returns></returns>
		public async Task LogInfo(string message, string details = null)
		{
			await Log(message, details, LogLevel.Information);
		}

		/// <summary>
		/// Logs error message.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="details"></param>
		/// <returns></returns>
		public async Task LogError(string message, string details)
		{
			await Log(message, details, LogLevel.Error);
		}

		/// <summary>
		/// Logs fatal error message.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="details"></param>
		/// <returns></returns>
		public async Task LogFatalError(string message, string details)
		{
			await Log(message, details, LogLevel.FatalError);
		}

		/// <summary>
		/// Logs error message.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="details"></param>
		/// <returns></returns>
		public async Task LogError(string message, Exception ex)
		{
			await LogError(message, ExceptionHelper.GetDetails(ex));
		}

		/// <summary>
		/// Logs fatal error message.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="details"></param>
		/// <returns></returns>
		public async Task LogFatalError(string message, Exception ex)
		{
			await LogFatalError(message, ExceptionHelper.GetDetails(ex));
		}

		#endregion Methods
	}
}
