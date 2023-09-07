namespace DataAccessLayer.DataClasses {
	public class ShowMessageButton : TicketingButton {
		public string MessageEn { get; set; }
		public string MessageAr { get; set; }

		public ShowMessageButton() : base() { 
			MessageEn = string.Empty;
			MessageAr = string.Empty;
		}

		public ShowMessageButton(string bankName, int screenId, int buttonId, string type, string nameEn, string nameAr, string messageEn, string messageAr) : base(bankName, screenId, buttonId, type, nameEn, nameAr) {
			MessageEn = messageEn;
			MessageAr = messageAr;
		}
	}
}
