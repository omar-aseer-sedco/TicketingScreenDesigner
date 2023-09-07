namespace DataAccessLayer.DataClasses {
	public abstract class TicketingButton {
		public string BankName { get; set; }
		public int ScreenId { get; set; }
		public int ButtonId { get; set; }
		public string Type { get; set; }
		public string NameEn { get; set; }
		public string NameAr { get; set; }

		public TicketingButton() {
			BankName = string.Empty;
			ScreenId = 0;
			ButtonId = 0;
			Type = string.Empty;
			NameEn = string.Empty;
			NameAr = string.Empty;
		}

		public TicketingButton(string bankName, int screenId, int buttonId, string type, string nameEn, string nameAr) {
			BankName = bankName;
			ScreenId = screenId;
			ButtonId = buttonId;
			Type = type;
			NameEn = nameEn;
			NameAr = nameAr;
		}
	}
}
