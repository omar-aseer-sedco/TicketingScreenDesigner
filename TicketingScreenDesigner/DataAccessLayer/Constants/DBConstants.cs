using System.Data.SqlClient;

namespace DataAccessLayer.Constants {
	public static class BanksConstants {
		public const string TABLE_NAME = "Banks";
		public const string BANK_NAME = "bank_name";
		public const string PASSWORD = "password";
		public const int BANK_NAME_SIZE = 255;
		public const int PASSWORD_SIZE = 255;
	}

	public static class ScreensConstants {
		public const string TABLE_NAME = "TicketingScreens";
		public const string BANK_NAME = "bank_name";
		public const string SCREEN_ID = "screen_id";
		public const string IS_ACTIVE = "is_active";
		public const string SCREEN_TITLE = "screen_title";
		public const int BANK_NAME_SIZE = 255;
		public const int SCREEN_TITLE_SIZE = 255;
	}

	public static class ButtonsConstants {
		public const string TABLE_NAME = "TicketingButtons";
		public const string BANK_NAME = "bank_name";
		public const string SCREEN_ID = "screen_id";
		public const string BUTTON_ID = "button_id";
		public const string TYPE = "type";
		public const string NAME_EN = "name_en";
		public const string NAME_AR = "name_ar";
		public const string SERVICE = "service";
		public const string SERVICE_ID = "service_id";
		public const string MESSAGE_EN = "message_en";
		public const string MESSAGE_AR = "message_ar";
		public const int BANK_NAME_SIZE = 255;
		public const int NAME_EN_SIZE = 255;
		public const int NAME_AR_SIZE = 255;
		public const int SERVICE_SIZE = 255;
		public const int MESSAGE_EN_SIZE = 1000;
		public const int MESSAGE_AR_SIZE = 1000;

		public const string TMP_ID = "tmp_id";
		public const string ADD_BUTTONS_PROCEDURE = "AddButtons";
		public const string ADD_BUTTONS_PARAMETER_NAME = "@buttons";
		public const string ADD_BUTTONS_PARAMETER_TYPE = "dbo.add_buttons_parameter";
		public const int ADD_BUTTONS_TMP_ID_INDEX = 2;
		public const int ADD_BUTTONS_SUCCESS_INDEX = 3;
		public const int ADD_BUTTONS_ERROR_NUMBER_INDEX = 4;
		public const int ADD_BUTTONS_ERROR_MESSAGE_INDEX = 5;

		public const string UPDATE_BUTTONS_PROCEDURE = "UpdateButtons";
		public const string UPDATE_BUTTONS_PARAMETER_NAME = "@buttons";
		public const string UPDATE_BUTTONS_PARAMETER_TYPE = "dbo.update_buttons_parameter";
		public const int UPDATE_BUTTONS_ID_INDEX = 2;
		public const int UPDATE_BUTTONS_SUCCESS_INDEX = 3;
		public const int UPDATE_BUTTONS_ERROR_NUMBER_INDEX = 4;
		public const int UPDATE_BUTTONS_ERROR_MESSAGE_INDEX = 5;

		public enum Types {
			UNDEFINED = 0,
			ISSUE_TICKET = 1,
			SHOW_MESSAGE = 2,
		}
	}

	public static class BankServicesConstants {
		public const string TABLE_NAME = "BankServices";

		public const string BANK_NAME = "bank_name";
		public const string BANK_SERVICE_ID = "service_id";
		public const string NAME_EN = "name_en";
		public const string NAME_AR = "name_ar";
		public const string ACTIVE = "active";
		public const string MAX_DAILY_TICKETS = "max_daily_tickets";
		public const string MIN_SERVICE_TIME = "min_service_time";
		public const string MAX_SERVICE_TIME = "max_service_time";

		public const int BANK_NAME_SIZE = 255;
		public const int NAME_EN_SIZE = 100;
		public const int NAME_AR_SIZE = 100;
	}

	public delegate void DatabaseNotificationDelegate(SqlNotificationInfo info);
}
