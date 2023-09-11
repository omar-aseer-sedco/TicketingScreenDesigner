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

		public bool IsEmpty() {
			return this == EmptyButton.Value;
		}

		public override bool Equals(object? obj) {
			if (obj is null) return false;
			if (obj is not TicketingButton) return false;
			TicketingButton other = (TicketingButton) obj;
			return other.BankName == BankName && other.ScreenId == ScreenId && other.ButtonId == ButtonId && other.Type == Type && other.NameEn == NameEn && other.NameAr == NameAr;
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
