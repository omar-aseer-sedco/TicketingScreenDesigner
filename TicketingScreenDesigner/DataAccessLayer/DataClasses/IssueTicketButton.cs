namespace DataAccessLayer.DataClasses {
	public class IssueTicketButton : TicketingButton {
		public string Service { get; set; }

		public IssueTicketButton() : base() { 
			Service = string.Empty;
		}

		public IssueTicketButton(string bankName, int screenId, int buttonId, string type, string nameEn, string nameAr, string service) : base(bankName, screenId, buttonId, type, nameEn, nameAr) {
			Service = service;
		}
	}
}
