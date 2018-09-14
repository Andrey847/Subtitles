using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SubtitlesLearn.Logic.Dal;
using SubtitlesLearn.Logic.Entities;
using SubtitlesLearn.Logic.Manager;

namespace SubtitlesLearn.Site.Controllers
{
    public class WorkPlaceController : Controller
    {
		public IActionResult Index()
		{
			return View();
		}

		/// <summary>
		/// Uploads subtitles.
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public IActionResult UploadSrt()
		{
			IActionResult response;

			if (Request.Form.Files.Count > 0)
			{
				IFormFile fileToUpload = Request.Form.Files.First();
				MemoryStream txt = GetFileStream(fileToUpload);

				string srt = Encoding.UTF8.GetString(txt.ToArray());

				Word[] words = SrtManager.Instance.GetWords(srt);

				List<Word> result = new List<Word>();

				// check each word with DB.
				foreach (Word word in words)
				{
					Word fromDb = DbAccess.GetWord(word, fileToUpload.FileName);
					fromDb.Frequency = word.Frequency;

					result.Add(fromDb);
				}

				result = result.OrderByDescending(item => item.Frequency).ToList();

				response = new JsonResult(result);
			}
			else
			{
				response = new OkResult();
			}

			return response;
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
		[HttpGet("[controller]/AllWords")]
		public IActionResult GetAllWords()
		{
			return new JsonResult(DbAccess.GetAllWords());
		}

		/// <summary>
		/// Returns word wav.
		/// </summary>
		/// <returns></returns>
		[HttpGet("[controller]/WordSound/{word}")]
		public IActionResult WordSound(string word)
		{
			byte[] wav = SoundManager.Instance.GetWav(word);

			MemoryStream ms = new MemoryStream(wav);
			ms.Position = 0;
			return new FileStreamResult(ms, "audio/x-wav");
		}
	}
}