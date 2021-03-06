﻿using SubtitlesLearn.Logic.Dal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace SubtitlesLearn.Logic
{
	/// <summary>
	/// Controls sound (play, get , store etc).
	/// </summary>
	public class SoundManager : ManagerBase
	{
		#region Fields

		private Dictionary<string, byte[]> _wavCache = new Dictionary<string, byte[]>();

		#endregion Fields

		#region Singleton

		private static Lazy<SoundManager> _instance = new Lazy<SoundManager>(() => new SoundManager());

		/// <summary>
		/// Main class instance.
		/// </summary>
		public static SoundManager Instance => _instance.Value;

		/// <summary>
		/// Private constructor (single).
		/// </summary>
		private SoundManager()
		{

		}

		#endregion Singleton

		#region Methods

		/// <summary>
		/// Gets wav from internal storage or from web.
		/// </summary>
		/// <param name="word"></param>
		/// <returns></returns>
		public byte[] GetWav(int customerId, string word)
		{
			string key = $"{customerId}${word}";
			byte[] result = null;

			if (!_wavCache.ContainsKey(key))
			{
				result = DbAccess.GetSound(customerId, word);

				if (result == null)
				{
					result = GetGoogleWav(word);
					DbAccess.AddSound(word, result);
					_wavCache.Add(key, result);
				}
				else
				{
					_wavCache.Add(key, result);
				}
			}

			result = _wavCache[key];

			return result;
		}

		/// <summary>
		/// Gets the wav file from twilio storage (direct URL).
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		private byte[] GetGoogleWav(string word)
		{
			byte[] buffer = new byte[16 * 1024];
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"https://translate.google.com/translate_tts?ie=UTF-8&q={word}&tl=en&client=tw-ob");
			request.Headers.Add("Referer", "http://translate.google.com/");
			request.Headers.Add("User-Agent", "stagefright/1.2 (Linux;Android 5.0)");

			using (WebResponse response = request.GetResponse())
			{
				using (MemoryStream ms = new MemoryStream())
				{
					using (Stream stream = response.GetResponseStream())
					{
						int read;
						while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
						{
							ms.Write(buffer, 0, read);
						}
					}

					return ms.ToArray();
				}
			}
		}

		#endregion Methods
	}
}
