namespace DataAccessLayer.DataClasses {
	public class TicketingButtonTMP
	{
		public string BankName { get; set; }
		public int ScreenId { get; set; }
		public int ButtonId { get; set; }
		public string Type { get; set; }
		public string NameEn { get; set; }
		public string NameAr { get; set; }
		public string? Service { get; set; }
		public string? MessageEn { get; set; }
		public string? MessageAr { get; set; }

		public TicketingButtonTMP()
		{
			BankName = string.Empty;
			ScreenId = 0;
			ButtonId = 0;
			Type = string.Empty;
			NameEn = string.Empty;
			NameAr = string.Empty;
			Service = null;
			MessageEn = null;
			MessageAr = null;
		}

		public TicketingButtonTMP(string bankName, int screenId, int buttonId, string type, string nameEn, string nameAr, string? service, string? messageEn, string? messageAr)
		{
			BankName = bankName;
			ScreenId = screenId;
			ButtonId = buttonId;
			Type = type;
			NameEn = nameEn;
			NameAr = nameAr;
			Service = service;
			MessageEn = messageEn;
			MessageAr = messageAr;
		}
	}
}
