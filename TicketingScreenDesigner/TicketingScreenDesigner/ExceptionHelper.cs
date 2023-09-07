using System.Data.SqlClient;
using LogUtils;

namespace TicketingScreenDesigner {
	public static class ExceptionHelper {
		public enum SqlErrorCodes {
			UniqueConstraintViolation = 2627,
			StatementTerminated = 3621,
			MissingQueryParamters = 8178,
			UnableToConnect = 53,
			UnableToOpenDatabase = 4060,
			LoginFailed = 18456,
		}

		public static void HandleGeneralException(Exception exception) {
			string message = $"Unhandled Error.\nType: {exception.GetType()}\nMessage: {exception.Message}";
			LogsHelper.Log(new LogEvent(message, DateTime.Now, EventSeverity.Error, exception.Source, exception.StackTrace));
			ShowErrorMessageBox(message);
		}

		public static void HandleSqlException(SqlException exception, string fieldName = "") {
			bool suppressStatementTermination = false;
			bool suppressLoginFailure = false;

			foreach (SqlError error in exception.Errors) {
				switch (error.Number) {
					case (int) SqlErrorCodes.UniqueConstraintViolation:
						ShowErrorMessageBox($"The field {fieldName} has to be unique.");
						LogsHelper.Log(new LogEvent("Duplicate key insertion rejected. " + error.Message, DateTime.Now, EventSeverity.Info, error.Source, exception.StackTrace));
						suppressStatementTermination = true;
						break;
					case (int) SqlErrorCodes.StatementTerminated:
						LogsHelper.Log(new LogEvent(error.Message, DateTime.Now, EventSeverity.Warning, error.Source, exception.StackTrace));
						if (!suppressStatementTermination)
							ShowErrorMessageBox("Statement terminated.");
						break;
					case (int) SqlErrorCodes.MissingQueryParamters:
						LogsHelper.Log(new LogEvent(error.Message, DateTime.Now, EventSeverity.Warning, error.Source, exception.StackTrace));
						ShowErrorMessageBox("Missing query parameters.");
						break;
					case (int) SqlErrorCodes.UnableToConnect:
						LogsHelper.Log(new LogEvent(error.Message, DateTime.Now, EventSeverity.Warning, error.Source, exception.StackTrace));
						ShowErrorMessageBox("Unable to connect to the server. It may have been configured incorrectly.");
						break;
					case (int) SqlErrorCodes.UnableToOpenDatabase:
						LogsHelper.Log(new LogEvent(error.Message, DateTime.Now, EventSeverity.Warning, error.Source, exception.StackTrace));
						ShowErrorMessageBox("Unable to open the database. It may have been configured incorrectly.");
						suppressLoginFailure = true;
						break;
					case (int) SqlErrorCodes.LoginFailed:
						LogsHelper.Log(new LogEvent(error.Message, DateTime.Now, EventSeverity.Warning, error.Source, exception.StackTrace));
						if (!suppressLoginFailure)
							ShowErrorMessageBox("Unable to log in to the database server.");
						break;
					default:
						string message = $"Unhandled SQL Error. Code: {error.Number}\nMessage: {error.Message}";
						LogsHelper.Log(new LogEvent(message, DateTime.Now, EventSeverity.Error, error.Source, exception.StackTrace));
						ShowErrorMessageBox(message);
						break;
				}
			}
		}

		public static void ShowErrorMessageBox(string message, string caption = "Error") {
			MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}
