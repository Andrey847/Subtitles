using System;
using System.Collections.Generic;
using System.Text;

namespace SubtitlesLearn.Logic.Infrastructure
{
	/// <summary>
	/// Storage of state for different data, related to UI. For example, choosed movies etc. 
	/// Splitted by blocks and these blocks usually related to pages.
	/// </summary>
    public class CustomerState
    {
		/// <summary>
		/// Id of customer.
		/// </summary>
		public int CustomerId { get; set; }

		/// <summary>
		/// State for workplace page.
		/// </summary>
		public string WorkPlace { get; set; }
    }
}
