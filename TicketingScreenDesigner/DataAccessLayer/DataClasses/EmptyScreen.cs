namespace DataAccessLayer.DataClasses {
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
