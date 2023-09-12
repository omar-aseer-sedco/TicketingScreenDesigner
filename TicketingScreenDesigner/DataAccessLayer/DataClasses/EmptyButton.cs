namespace DataAccessLayer.DataClasses {
	/// <summary>
	/// Singleton class representing an empty button object.
	/// </summary>
	public sealed class EmptyButton : TicketingButton {
		private EmptyButton() { 
			BankName = string.Empty;
			ScreenId = -1;
			ButtonId = -1;
			Type = string.Empty;
			NameEn = string.Empty;
			NameAr = string.Empty;
		}

		public static readonly EmptyButton Value = new();
	}
}
