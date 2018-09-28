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
		public CustomerRole Role { get; set; } = new CustomerRole();

		/// <summary>
		/// Role of the customer as enum.
		/// </summary>
		public RoleType RoleType => (RoleType)Role.Id;

		/// <summary>
		/// IS customer blocked?
		/// </summary>
		public bool IsBlocked { get; set; }

		/// <summary>
		/// Confirmation code (if account is not confirmed yet).
		/// </summary>
		public string ConfirmationCode { get; set; }

		/// <summary>
		/// Restore password code.
		/// </summary>
		public string RestorePasswordCode { get; set; }
	}
}
