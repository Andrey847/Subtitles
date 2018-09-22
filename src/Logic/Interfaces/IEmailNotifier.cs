using System.Threading.Tasks;

namespace SubtitlesLearn.Logic.Interfaces
{
	/// <summary>
	/// Interface of the object that performs Email notifications.
	/// </summary>
	public interface IEmailNotifier
	{
		/// <summary>
		/// Sends simple email (body is html formatted).
		/// </summary>
		/// <param name="to"></param>
		/// <param name="subject"></param>
		/// <param name="body"></param>
		/// <returns></returns>
		Task<bool> SendSimpleText(string to, string subject, string body);
	}
}
