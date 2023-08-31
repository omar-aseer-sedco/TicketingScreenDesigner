using System.Data.SqlClient;

namespace TicketingScreenDesigner {
	public static class DBUtils {
		private const string databaseName = "TSD";

		public static SqlConnection CreateConnection() {
			try {
				CreateDatabase();

				string connectionString = $"server=(local);database={databaseName};integrated security=sspi";
				var connection = new SqlConnection(connectionString);

				CreateTables(connection);

				return connection;
			}
			catch (ArgumentException ex) {
				ExceptionHelper.ShowErrorMessageBox("Failed to connect to database. Please try again.");
				LogsHelper.Log(new LogEvent(ex.Message, DateTime.Now, EventSeverity.Error));
				throw;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				throw;
			}
		}

		private static void CreateDatabase() {
			SqlConnection? connection = null;

			try {
				string connectionString = "server=(local);integrated security=sspi";
				connection = new SqlConnection(connectionString);

				string findDatabaseQuery = "SELECT Name FROM sys.databases WHERE Name = @databaseName;";
				var findDatabaseCommand = new SqlCommand(findDatabaseQuery, connection);
				findDatabaseCommand.Parameters.AddWithValue("@databaseName", databaseName);

				connection.Open();

				var result = findDatabaseCommand.ExecuteScalar();

				if (result is null) {
					string createDatabaseQuery = $"CREATE DATABASE {databaseName};";
					var createDatabaseCommand = new SqlCommand(createDatabaseQuery, connection);
					createDatabaseCommand.ExecuteNonQuery();
				}
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
			finally {
				connection?.Close();
			}
		}

		private static bool CheckIfTableExists(SqlConnection connection, string tableName) {
			string checkQuery = $"SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = @tableName;";
			var checkCommand = new SqlCommand(checkQuery, connection);
			checkCommand.Parameters.AddWithValue("@tableName", tableName);

			return checkCommand.ExecuteScalar() is not null;
		}

		private static void CreateBanksTable(SqlConnection connection) {
			if (!CheckIfTableExists(connection, BanksConstants.TABLE_NAME)) {
				string query = $"CREATE TABLE {BanksConstants.TABLE_NAME} ({BanksConstants.BANK_NAME} VARCHAR(255) NOT NULL, PRIMARY KEY ({BanksConstants.BANK_NAME}));";
				var command = new SqlCommand(query, connection);
				command.ExecuteNonQuery();
			}
		}

		private static void CreateScreensTable(SqlConnection connection) {
			if (!CheckIfTableExists(connection, ScreensConstants.TABLE_NAME)) {
				string query = $"CREATE TABLE {ScreensConstants.TABLE_NAME} ({ScreensConstants.BANK_NAME} VARCHAR(255) NOT NULL, {ScreensConstants.SCREEN_ID} VARCHAR(255) NOT NULL, {ScreensConstants.IS_ACTIVE} BIT NOT NULL, {ScreensConstants.SCREEN_TITLE} VARCHAR(255), " +
					$"FOREIGN KEY ({ScreensConstants.BANK_NAME}) REFERENCES {BanksConstants.TABLE_NAME}({BanksConstants.BANK_NAME}) ON DELETE CASCADE ON UPDATE CASCADE, PRIMARY KEY ({ScreensConstants.BANK_NAME}, {ScreensConstants.SCREEN_ID})); ";
				var command = new SqlCommand(query, connection);
				command.ExecuteNonQuery();
			}
		}

		private static void CreateButtonsTable(SqlConnection connection) {
			if (!CheckIfTableExists(connection, ButtonsConstants.TABLE_NAME)) {
				string query = $"CREATE TABLE {ButtonsConstants.TABLE_NAME} ({ButtonsConstants.BANK_NAME} VARCHAR(255) NOT NULL, {ButtonsConstants.SCREEN_ID} VARCHAR(255) NOT NULL, {ButtonsConstants.BUTTON_ID} VARCHAR(255) NOT NULL, {ButtonsConstants.TYPE} VARCHAR(255) NOT NULL, " +
					$"{ButtonsConstants.NAME_EN} VARCHAR(255) NOT NULL, {ButtonsConstants.NAME_AR} NVARCHAR(255) NOT NULL, {ButtonsConstants.SERVICE} VARCHAR(255), {ButtonsConstants.MESSAGE_EN} VARCHAR(1000), {ButtonsConstants.MESSAGE_AR} NVARCHAR(1000), " +
					$"FOREIGN KEY ({ButtonsConstants.BANK_NAME}, {ButtonsConstants.SCREEN_ID}) REFERENCES {ScreensConstants.TABLE_NAME}({ScreensConstants.BANK_NAME}, {ScreensConstants.SCREEN_ID}) ON DELETE CASCADE ON UPDATE CASCADE," +
					$"PRIMARY KEY ({ButtonsConstants.BANK_NAME}, {ButtonsConstants.SCREEN_ID}, {ButtonsConstants.BUTTON_ID}));";
				var command = new SqlCommand(query, connection);
				command.ExecuteNonQuery();
			}
		}

		private static void CreateTables(SqlConnection connection) {
			try {
				connection.Open();

				CreateBanksTable(connection);
				CreateScreensTable(connection);
				CreateButtonsTable(connection);
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
			finally {
				connection.Close();
			}
		}
	}
}
