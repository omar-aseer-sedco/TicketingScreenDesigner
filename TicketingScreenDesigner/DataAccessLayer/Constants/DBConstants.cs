namespace DataAccessLayer.Constants
{
    public static class DBConstants
    {
        public const string DATABASE_NAME = "TSD";
    }

    public static class BanksConstants
    {
        public const string TABLE_NAME = "Banks";
        public const string BANK_NAME = "bank_name";
    }

    public static class ScreensConstants
    {
        public const string TABLE_NAME = "Screens";
        public const string BANK_NAME = "bank_name";
        public const string SCREEN_ID = "screen_id";
        public const string IS_ACTIVE = "is_active";
        public const string SCREEN_TITLE = "screen_title";
    }

    public static class ButtonsConstants
    {
        public const string TABLE_NAME = "Buttons";
        public const string BANK_NAME = "bank_name";
        public const string SCREEN_ID = "screen_id";
        public const string BUTTON_ID = "button_id";
        public const string TYPE = "type";
        public const string NAME_EN = "name_en";
        public const string NAME_AR = "name_ar";
        public const string SERVICE = "service";
        public const string MESSAGE_EN = "message_en";
        public const string MESSAGE_AR = "message_ar";

        public static class Types
        {
            public const string ISSUE_TICKET = "Issue Ticket";
            public const string SHOW_MESSAGE = "Show Message";
        }
    }
}
