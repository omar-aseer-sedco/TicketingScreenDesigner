using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TicketingScreenDesigner {
	public static class ExceptionHelper {
		public enum SqlErrorCodes {
			UniqueConstraintViolation = 2627,
			StatementTerminated = 3621,
			MissingQueryParamters = 8178,
		}

		public static void HandleGeneralException(Exception exception) {
			string message = $"Unhandled Error.\nType: {exception.GetType()}\nMessage: {exception.Message}";
			LogsHelper.Log(new LogEvent(message, DateTime.Now, EventSeverity.Error, exception.Source, exception.StackTrace));
			ShowErrorMessageBox(message);
		}

		public static void HandleConfigReadError(Exception exception) {
			string message = $"Unable to read configuration file. Message: {exception.Message}";
			LogsHelper.Log(new LogEvent(message, DateTime.Now, EventSeverity.Error, exception.Source, exception.StackTrace));
			ShowErrorMessageBox(message);
		}

		public static void HandleSqlException(SqlException exception, string fieldName = "") {
			bool suppressStatementTermination = false;

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
