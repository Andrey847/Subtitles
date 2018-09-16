namespace SubtitlesLearn.Logic.Entities
{
	/// <summary>
	/// Customer = User of the system.
	/// </summary>
	public class Customer
	{
		/// <summary>
		/// Id from the DB.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Email.
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// User Name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Password hash.
		/// </summary>
		public string PasswordHash { get; set; }

		/// <summary>
		/// Plain password. Used for login/password change actions.
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		/// Is login confirmed (by email).
		/// </summary>
		public bool IsConfirmed { get; set; }

		/// <summary>
		/// Role of the customer.
		/// </summary>
		public CustomerRole Role { get; set; }
	}
}
