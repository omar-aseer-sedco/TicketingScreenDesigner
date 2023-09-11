using DataAccessLayer.Constants;

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

		public bool IsEmpty() {
			return this == EmptyScreen.Value;
		}

		public override bool Equals(object? obj) {
			if (obj is null) return false;
			if (obj is not TicketingScreen) return false;
			TicketingScreen other = (TicketingScreen) obj;
			return other.BankName == BankName && other.ScreenId == ScreenId && other.IsActive == IsActive && other.ScreenTitle == ScreenTitle;
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
