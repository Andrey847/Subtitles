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
using SubtitlesLearn.Logic;
using SubtitlesLearn.Logic.Dal;
using SubtitlesLearn.Logic.Entities;
using SubtitlesLearn.Logic.Manager;
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

		#endregion Fields

		#region Construction

		/// <summary>
		/// Main constructor.
		/// </summary>
		/// <param name="userManager"></param>
		/// <param name="signInManager"></param>
		/// <param name="recaptchaSettings"></param>
		/// <param name="accessor"></param>
		public WorkPlaceController(ApplicationUserManager userManager)
		{
			_userManager = userManager;			
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

			ViewBag.Movies = (await SrtManager.Instance.GetMovies(customer.Id)).OrderBy(item => item.Name);

			return View();
		}

		/// <summary>
		/// Returns all phrases for required word.
		/// </summary>
		/// <param name="wordId"></param>
		/// <returns></returns>
		[HttpGet("[controller]/[action]/{wordId}")]
		public async Task<IActionResult> GetPhrases(int wordId)
		{
			return new JsonResult(await SrtManager.Instance.GetPhrases(wordId));
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
		public IActionResult MarkLearned([FromBody] Word word)
		{
			DbAccess.MarkLearned(word);

			return new OkResult();
		}

		/// <summary>
		/// Returns all words in the system.
		/// </summary>
		/// <returns></returns>
		[HttpGet("[controller]/AllWords/{movieId}")]
		public async Task <IActionResult> GetAllWords(int movieId)
		{
			Customer customer = await _userManager.GetUserAsync(User);

			return new JsonResult(DbAccess.GetAllWords(customer.Id, movieId == 0 ? null : (int?)movieId));
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

		#endregion Methods
	}
}