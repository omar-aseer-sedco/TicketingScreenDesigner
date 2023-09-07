using System.Data.SqlClient;
using LogUtils;

namespace DataAccessLayer.Utils
{
    public static class DALExceptionHelper
    {
        public enum SqlErrorCodes
        {
            UniqueConstraintViolation = 2627,
            StatementTerminated = 3621,
            MissingQueryParameters = 8178,
            UnableToConnect = 53,
            UnableToOpenDatabase = 4060,
            LoginFailed = 18456,
        }

        public static void HandleGeneralException(Exception exception)
        {
            string message = $"Unhandled Error.\nType: {exception.GetType()}\nMessage: {exception.Message}";
            LogsHelper.Log(new LogEvent(message, DateTime.Now, EventSeverity.Error, exception.Source, exception.StackTrace));
        }

        public static void HandleSqlException(SqlException exception, string fieldName = "")
        {
            foreach (SqlError error in exception.Errors)
            {
                switch (error.Number)
                {
                    case (int)SqlErrorCodes.UniqueConstraintViolation:
                        LogsHelper.Log(new LogEvent("Duplicate key insertion rejected. " + error.Message, DateTime.Now, EventSeverity.Info, error.Source, exception.StackTrace));
                        break;
                    case (int)SqlErrorCodes.StatementTerminated:
                    case (int)SqlErrorCodes.MissingQueryParameters:
                    case (int)SqlErrorCodes.UnableToConnect:
                    case (int)SqlErrorCodes.UnableToOpenDatabase:
                    case (int)SqlErrorCodes.LoginFailed:
                        LogsHelper.Log(new LogEvent(error.Message, DateTime.Now, EventSeverity.Error, error.Source, exception.StackTrace));
                        break;
                    default:
                        LogsHelper.Log(new LogEvent($"Unhandled SQL Error. Code: {error.Number}\nMessage: {error.Message}", DateTime.Now, EventSeverity.Error, error.Source, exception.StackTrace));
                        break;
                }
            }
        }
    }
}
