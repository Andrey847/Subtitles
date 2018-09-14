using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubtitlesLearn.Logic.Dal
{
	/// <summary>
	/// Object with identity field.
	/// </summary>
	public interface IIdentifiable : IDatabaseObject
	{
		/// <summary>
		/// Identity.
		/// </summary>
		int Id { get; set; }
	}
}
