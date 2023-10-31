using DataAccessLayer.Constants;

namespace DataAccessLayer.DataClasses {
	public class IssueTicketButton : TicketingButton {
		public int ServiceId { get; set; }

		public IssueTicketButton() : base() { 
			ServiceId = 0;
		}

		public IssueTicketButton(string bankName, int screenId, int buttonId, ButtonsConstants.Types type, string nameEn, string nameAr, int serviceId) : base(bankName, screenId, buttonId, type, nameEn, nameAr) {
			ServiceId = serviceId;
		}
	}
}
