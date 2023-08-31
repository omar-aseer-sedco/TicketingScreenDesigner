using System.Data.SqlClient;

namespace TicketingScreenDesigner {
	public static class DBUtils {
		public static SqlConnection CreateConnection() {
			try {
				string connectionString = @"server=(local);database=TSD;integrated security=sspi";
				//string connectionString = @"server=11.0.0.151\MSSQLSERVER;database=TSD;integrated security=sspi";
				return new SqlConnection(connectionString);
			}
			catch (ArgumentException ex) {
				ExceptionHelper.ShowErrorMessageBox("Failed to connect to database. Please try again.");
				LogsHelper.Log(new LogEvent(ex.Message, DateTime.Now, EventSeverity.Error));
				throw;
			}
		}
	}
}
