namespace DataAccessLayer.DataClasses {
	public class TicketingScreen {
		public string BankName { get; set; }
		public int ScreenId { get; set; }
		public string ScreenTitle { get; set; }
		public bool IsActive { get; set; }

		public TicketingScreen() {
			BankName = string.Empty;
			ScreenId = 0;
			ScreenTitle = string.Empty;
			IsActive = false;
		}

		public TicketingScreen(string bankName, int screenId, string screenTitle, bool isActive) {
			BankName = bankName;
			ScreenId = screenId;
			ScreenTitle = screenTitle;
			IsActive = isActive;
		}
	}
}
