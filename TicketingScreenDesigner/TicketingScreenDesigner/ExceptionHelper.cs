using System.Data.SqlClient;

namespace TicketingScreenDesigner {
	public static class ExceptionHelper {
		public enum SqlErrorCodes {
			UniqueConstraintViolation = 2627,
			StatementTerminated = 3621,
			MissingQueryParamters = 8178,
		}

		public static void HandleGeneralException(Exception exception) {
			string message = $"Unhandled Error.\nType: {exception.GetType()}\nMessage: {exception.Message}";
			ShowErrorMessageBox(message);
			LogsHelper.Log(new LogEvent(message, DateTime.Now, EventSeverity.Error, exception.Source, exception.StackTrace));
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
						if (!suppressStatementTermination)
							ShowErrorMessageBox("Statement terminated.");
						LogsHelper.Log(new LogEvent(error.Message, DateTime.Now, EventSeverity.Warning, error.Source, exception.StackTrace));
						break;
					case (int) SqlErrorCodes.MissingQueryParamters:
						ShowErrorMessageBox("Missing query parameters.");
						LogsHelper.Log(new LogEvent(error.Message, DateTime.Now, EventSeverity.Warning, error.Source, exception.StackTrace));
						break;
					default:
						string message = $"Unhandled SQL Error. Code: {error.Number}\nMessage: {error.Message}";
						ShowErrorMessageBox(message);
						LogsHelper.Log(new LogEvent(message, DateTime.Now, EventSeverity.Error, error.Source, exception.StackTrace));
						break;
				}
			}
		}

		public static void ShowErrorMessageBox(string message, string caption = "Error") {
			MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}
