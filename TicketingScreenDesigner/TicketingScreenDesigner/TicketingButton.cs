namespace TicketingScreenDesigner
{
	public class TicketingButton
	{
		public string BankName { get; set; }
		public string ScreenId { get; set; }
		public string ButtonId { get; set; }
		public string Type { get; set; }
		public string NameEn { get; set; }
		public string NameAr { get; set; }
		public string? Service { get; set; }
		public string? MessageEn { get; set; }
		public string? MessageAr { get; set; }

		public TicketingButton()
		{
			BankName = string.Empty;
			ScreenId = string.Empty;
			ButtonId = string.Empty;
			Type = string.Empty;
			NameEn = string.Empty;
			NameAr = string.Empty;
			Service = null;
			MessageEn = null;
			MessageAr = null;
		}

		public TicketingButton(string bankName, string screenId, string buttonId, string type, string nameEn, string nameAr, string? service, string? messageEn, string? messageAr)
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
