using System;
using System.Text;

namespace SubtitlesLearn.Logic
{
	/// <summary>
	/// Explode all exception stack to correct message.
	/// </summary>
	public static class ExceptionHelper
	{
		/// <summary>
		/// Returns all exception details.
		/// </summary>
		/// <param name="ex"></param>
		/// <returns></returns>
		public static string GetDetails(Exception ex)
		{
			if (ex == null)
			{
				throw new ArgumentNullException(nameof(ex));
			}

			StringBuilder result = new StringBuilder();

			Exception current = ex;

			while (current != null)
			{
				result.AppendLine(ex.Message);
				result.AppendLine(ex.StackTrace);

				current = ex.InnerException;
			}

			return result.ToString();
		}
	}
}
