using System.Data.SqlClient;

namespace TicketingScreenDesigner {
	public static class ExceptionHelper {
		public static void HandleGeneralException(Exception exception) {
			string message = $"Unhandled Error.\nType: {exception.GetType()}\nMessage: {exception.Message}";
			ShowErrorMessageBox(message);
			LogsHelper.Log(new LogEvent(message, DateTime.Now, EventSeverity.Error));
		}

		public enum SqlErrorCodes {
			UniqueConstraintViolation = 2627,
			StatementTerminated = 3621,
		}

		public static void HandleSqlException(SqlException exception, string fieldName) {
			foreach (SqlError error in exception.Errors) {
				switch (error.Number) {
					case (int) SqlErrorCodes.UniqueConstraintViolation:
						ShowErrorMessageBox($"The field {fieldName} has to be unique.");
						LogsHelper.Log(new LogEvent("Duplicate primary key operation rejected", DateTime.Now, EventSeverity.Info));
						break;
					case (int) SqlErrorCodes.StatementTerminated:
						LogsHelper.Log(new LogEvent("Statement terminated", DateTime.Now, EventSeverity.Warning));
						break;
					default:
						string message = $"Unhandled SQL Error. Code: {error.Number}\nMessage: {error.Message}";
						ShowErrorMessageBox(message);
						LogsHelper.Log(new LogEvent(message, DateTime.Now, EventSeverity.Error));
						break;
				}
			}
		}

		private static void ShowErrorMessageBox(string message, string caption = "Error") {
			MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}
