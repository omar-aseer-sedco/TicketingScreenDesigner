namespace DataAccessLayer.DataClasses {
	/// <summary>
	/// Singleton class representing an empty screen object.
	/// </summary>
	public sealed class EmptyScreen : TicketingScreen {
		private EmptyScreen() {
			BankName = string.Empty;
			ScreenId = -1;
			ScreenTitle = string.Empty;
			IsActive = false;
		}

		public static readonly EmptyScreen Value = new();
	}
}
