using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SubtitlesLearn.Logic;
using SubtitlesLearn.Logic.Dal;
using SubtitlesLearn.Logic.Entities;
using SubtitlesLearn.Logic.Infrastructure;
using SubtitlesLearn.Logic.Manager;
using SubtitlesLearn.Site.Models;
using SubtitlesLearn.Site.Services;
using SubtitlesLearn.Site.Services.Identity;

namespace SubtitlesLearn.Site.Controllers
{
	/// <summary>
	/// Main work place.
	/// </summary>
	[Authorize]
	public class WorkPlaceController : Controller
	{
		#region Fields

		private readonly ApplicationUserManager _userManager;

		private static IHubContext<NotificationHub> _hubContext = null;
		private static object _syncHub = new object();

		#endregion Fields

		#region Construction

		/// <summary>
		/// Main constructor.
		/// </summary>
		/// <param name="userManager"></param>
		/// <param name="signInManager"></param>
		/// <param name="recaptchaSettings"></param>
		/// <param name="accessor"></param>
		public WorkPlaceController(ApplicationUserManager userManager,
			IHubContext<NotificationHub> hubContext)
		{
			_userManager = userManager;

			// initialize SignalR context and subscribe on load changes			
			if (_hubContext == null)
			{
				lock (_syncHub)
				{
					if (_hubContext == null)
					{
						_hubContext = hubContext;

						SrtManager.Instance.SrtUploadProgress += SrtUploadProgress;
					}
				}
			}
		}

		#endregion Construction

		#region Methods

		/// <summary>
		/// Main page.
		/// </summary>
		/// <returns></returns>
		public async Task<IActionResult> Index()
		{
			Customer customer = await _userManager.GetUserAsync(User);
			CustomerSettings settings = await UserManager.Instance.GetSettings(customer.Id);

			ViewBag.CurrentPageType = PageType.WorkPlace;
			ViewBag.Movies = (await SrtManager.Instance.GetMovies(customer.Id, settings.ShowArchivedMovies, settings.CurrentLanguageCode));

			return View();
		}

		/// <summary>
		/// Returns all movies for the Customer.
		/// </summary>
		/// <returns></returns>
		[HttpGet("[controller]/[action]")]
		public async Task<IActionResult> GetMovies()
		{
			Customer customer = await _userManager.GetUserAsync(User);
			CustomerSettings settings = await UserManager.Instance.GetSettings(customer.Id);

			Movie[] result = await SrtManager.Instance.GetMovies(customer.Id, settings.ShowArchivedMovies, settings.CurrentLanguageCode);

			return new JsonResult(result);
		}

		/// <summary>
		/// Returns all phrases for required word.
		/// </summary>
		/// <param name="wordId"></param>
		/// <returns></returns>
		[HttpGet("[controller]/[action]/{wordId}/{movieId?}")]
		public async Task<IActionResult> GetPhrases(int wordId, int? movieId)
		{
			if (movieId == 0)
			{
				movieId = null;
			}

			return new JsonResult(await SrtManager.Instance.GetPhrases(wordId, movieId));
		}

		/// <summary>
		/// Uploads subtitles.
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> UploadSrt()
		{
			IActionResult result;
			Customer customer = await _userManager.GetUserAsync(User);

			if (Request.Form.Files.Count > 0)
			{
				IFormFile fileToUpload = Request.Form.Files.First();
				MemoryStream txt = GetFileStream(fileToUpload);

				result = new JsonResult(await SrtManager.Instance.ImportWords(customer.Id, txt, fileToUpload.FileName));
			}
			else
			{
				result = new OkResult();
			}

			return result;
		}

		private MemoryStream GetFileStream(IFormFile file)
		{
			MemoryStream ms = new MemoryStream();
			file.CopyTo(ms);
			ms.Position = 0;

			if (Path.GetExtension(file.Name).ToLower() == ".zip")
			{
				using (ZipArchive zip = new ZipArchive(ms))
				{
					// Always take first file from archive.
					ZipArchiveEntry entry = zip.Entries.First();

					using (BinaryReader sr = new BinaryReader(entry.Open()))
					{
						byte[] buffer = new byte[entry.Length];
						sr.Read(buffer, 0, buffer.Length);
						ms = new MemoryStream(buffer);
						ms.Position = 0;
					}
				}
			}

			return ms;
		}

		/// <summary>
		/// Saves translation.
		/// </summary>
		/// <param name="word"></param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult SaveTranslation([FromBody] Word word)
		{
			DbAccess.SaveTranslation(word);

			return new OkResult();
		}

		/// <summary>
		/// Marks the word as learned.
		/// </summary>
		/// <param name="word"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> MarkLearned([FromBody] Word word)
		{
			return await SetLearned(word, true);
		}

		/// <summary>
		/// Marks the word as UNlearned.
		/// </summary>
		/// <param name="word"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> MarkUnlearned([FromBody] Word word)
		{
			return await SetLearned(word, false);
		}

		private async Task<IActionResult> SetLearned(Word word, bool isLearned)
		{
			Customer customer = await _userManager.GetUserAsync(User);
			word.CustomerId = customer.Id;
			await SrtManager.Instance.SetLearned(word, isLearned);
			return new OkResult();
		}

		/// <summary>
		/// Returns all words in the system.
		/// </summary>
		/// <returns></returns>
		[HttpGet("[controller]/AllWords/{movieId}")]
		public async Task<IActionResult> GetAllWords(int movieId)
		{
			Customer customer = await _userManager.GetUserAsync(User);

			List<Word> words = await SrtManager.Instance.GetAllWords(customer.Id, movieId == 0 ? null : (int?)movieId);

			return new JsonResult(words);
		}

		/// <summary>
		/// Returns word wav.
		/// </summary>
		/// <returns></returns>
		[HttpGet("[controller]/WordSound/{word}")]
		public async Task<IActionResult> WordSound(string word)
		{
			Customer customer = await _userManager.GetUserAsync(User);

			byte[] wav = SoundManager.Instance.GetWav(customer.Id, word);

			MemoryStream ms = new MemoryStream(wav);
			ms.Position = 0;
			return new FileStreamResult(ms, "audio/x-wav");
		}

		/// <summary>
		/// Removes movie completly from  DB.
		/// </summary>
		/// <param name="movieId"></param>
		/// <returns></returns>
		[HttpDelete("[controller]/[action]/{movieId}")]
		public async Task<IActionResult> DeleteMovie(int movieId)
		{
			Customer customer = await _userManager.GetUserAsync(User);
			await SrtManager.Instance.DeleteMovie(customer.Id, movieId);

			return Ok();
		}

		/// <summary>
		/// Renames movide.
		/// </summary>
		/// <param name="movieId"></param>
		/// <param name="newName"></param>
		/// <returns></returns>
		[HttpPost("[controller]/[action]/{movieId}/{newName}")]
		public async Task<IActionResult> RenameMovie(int movieId, string newName)
		{
			Customer customer = await _userManager.GetUserAsync(User);
			string error = await SrtManager.Instance.RenameMovie(customer.Id, movieId, newName);

			return new JsonResult(error);
		}

		/// <summary>
		/// Sets archive state for movie (sets or unsets).
		/// </summary>
		/// <param name="movieId"></param>
		/// <returns></returns>
		[HttpPost("[controller]/[action]/{movieId}/{archive}")]
		public async Task<IActionResult> SetArchiveState(int movieId, bool archive)
		{
			Customer customer = await _userManager.GetUserAsync(User);

			await SrtManager.Instance.SetArchiveState(customer.Id, movieId, archive);

			return Ok();
		}

		private void SrtUploadProgress(object sender, SrtProgressArgs e)
		{
			_hubContext.Clients.User(e.CustomerId.ToString()).SendAsync("UploadProgress", e.PercentCompleted);
		}

		/// <summary>
		/// Gets layout for the search page.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<JsonResult> GetLayout()
		{
			Customer customer = await _userManager.GetUserAsync(User);

			CustomerState state = await UserManager.Instance.GetState(customer.Id);

			return new JsonResult(state.WorkPlace);
		}

		/// <summary>
		/// Saves layout to the database.
		/// </summary>
		/// <param name="layout"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<JsonResult> SaveLayout([FromBody] object layout)
		{
			Customer customer = await _userManager.GetUserAsync(User);

			CustomerState state = new CustomerState();
			state.CustomerId = customer.Id;
			state.WorkPlace = layout.ToString();

			await UserManager.Instance.UpdateState(state);

			return new JsonResult("Saved");
		}

		#endregion Methods
	}
}