using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SubtitlesLearn.Logic.Entities
{
	/// <summary>
	/// Main logging methods.
	/// </summary>
	public interface ILogging
	{
		Task LogDebug(string message, string details = null);

		Task LogInfo(string message, string details = null);

		Task LogError(string message, string details);

		Task LogFatalError(string message, string details);

		Task LogError(string message, Exception ex);

		Task LogFatalError(string message, Exception ex);
	}
}
