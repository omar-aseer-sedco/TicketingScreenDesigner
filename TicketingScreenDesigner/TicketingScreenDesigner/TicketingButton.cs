using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketingScreenDesigner
{
	public class TicketingButton
	{
		public string BankName { get; private set; }
		public string ScreenId { get; private set; }
		public string ButtonId { get; private set; }
		public string Type { get; private set; }
		public string NameEn { get; private set; }
		public string NameAr { get; private set; }
		public string? Service { get; private set; }
		public string? MessageEn { get; private set; }
		public string? MessageAr { get; private set; }

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
