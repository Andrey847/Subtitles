using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using SubtitlesLearn.Logic;
using System.Net;

namespace SubtitlesLearn.Site.Services
{
	/// <summary>
	/// Global exception filter to catch and log all unhandled exceptions.
	/// </summary>
	public class GlobalExceptionFilter : IExceptionFilter
	{
		/// <summary>
		/// <see cref="IExceptionFilter.OnException(ExceptionContext)"/>
		/// </summary>
		/// <param name="context"></param>
		public void OnException(ExceptionContext context)
		{
			HttpResponse response = context.HttpContext.Response;
			response.StatusCode = (int)HttpStatusCode.InternalServerError;
			response.ContentType = "application/json";
			response.WriteAsync("A server error occurred. Please try to repeat the operation again.");

			LogManager.Instance.LogFatalError("Global unhandled exception", context.Exception).GetAwaiter().GetResult();			
		}
	}
}
