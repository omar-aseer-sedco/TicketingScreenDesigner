using System.Data.SqlClient;

namespace TicketingScreenDesigner {
	public static class DBUtils {
		public static SqlConnection CreateConnection() {
			string connectionString = "server=(local);database=TSD;integrated security=sspi";
			return new SqlConnection(connectionString);
		}
	}

	public static class ExceptionHelper {
		public static void HandleGeneralException(Exception exception) {
			ShowErrorMessageBox($"Unhandled Error.\nType: {exception.GetType()}\nMessage: {exception.Message}");
		}

		public enum SqlErrorCodes {
			UniqueConstraintViolation = 2627,
		}

		public static void HandleSqlException(SqlException exception, string fieldName) {
			foreach (SqlError error in exception.Errors) {
				switch (error.Number) {
					case (int) SqlErrorCodes.UniqueConstraintViolation:
						ShowErrorMessageBox($"The field {fieldName} has to be unique.");
						break;
					default:
						ShowErrorMessageBox($"Unhandled SQL Error. Code: {exception.Number}\nMessage: {exception.Message}", "Oops");
						break;
				}
			}
		}

		private static void ShowErrorMessageBox(string message, string caption = "Error") {
			MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}
