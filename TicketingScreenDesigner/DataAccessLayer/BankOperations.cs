using System.Data.SqlClient;
using DataAccessLayer.Constants;
using DataAccessLayer.DataClasses;
using DataAccessLayer.Utils;
using ExceptionUtils;
using System.Data;

namespace DataAccessLayer {
	/// <summary>
	/// Contains methods for retrieving and manipulating bank information.
	/// </summary>
	public static class BankOperations {
		private static SqlConnection? connection = null;

		private static bool Initialize() {
			try {
				connection ??= DBUtils.CreateConnection();
				return true;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return false;
			}
		}

		/// <summary>
		/// Verifies that the database connection is established correctly.
		/// </summary>
		/// <returns><c>true</c> if the connection has been established properly, and <c>false</c> otherwise.</returns>
		public static bool VerifyConnection() {
			try {
				if (!Initialize())
					return false;

				connection!.Open();

				return true;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return false;
			}
			finally {
				connection?.Close();
			}
		}

		/// <summary>
		/// Checks if the bank with the specified <c>bankName</c> exists in the database.
		/// </summary>
		/// <param name="bankName">The name of the bank. Case insensitive.</param>
		/// <returns><c>true</c> if the bank exists in the database, and <c>false</c> if it does not. If the operation fails, <c>null</c> is returned.</returns>
		public static bool? CheckIfBankExists(string bankName) {
			try {
				if (!Initialize())
					return null;

				string query = $"SELECT * FROM {BanksConstants.TABLE_NAME} WHERE {BanksConstants.BANK_NAME} = @bankName;";
				var command = new SqlCommand(query, connection);
				command.Parameters.Add("@bankName", SqlDbType.VarChar, BanksConstants.BANK_NAME_SIZE).Value = bankName;

				connection!.Open();
				return command.ExecuteReader().HasRows;
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex, "Bank Name");
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
			finally {
				connection?.Close();
			}

			return null;
		}

		/// <summary>
		/// Adds a bank to the database.
		/// </summary>
		/// <param name="bank">The bank to be added to the database.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public static bool AddBank(Bank bank) {
			try {
				if (!Initialize())
					return false;

				string query = $"INSERT INTO {BanksConstants.TABLE_NAME} ({BanksConstants.BANK_NAME}, {BanksConstants.PASSWORD}) VALUES (@bankName, @password);";
				SqlCommand command = new SqlCommand(query, connection);
				command.Parameters.Add("@bankName", SqlDbType.VarChar, BanksConstants.BANK_NAME_SIZE).Value = bank.BankName;
				command.Parameters.Add("@password", SqlDbType.VarChar, BanksConstants.PASSWORD_SIZE).Value = bank.Password;

				connection!.Open();
				return command.ExecuteNonQuery() == 1;
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex, "Bank Name");
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
			finally {
				connection?.Close();
			}

			return false;
		}

		/// <summary>
		/// Asynchronously gets all the screens for the bank with the specified name.
		/// </summary>
		/// <param name="bankName">The name of the bank. Case insensitive.</param>
		/// <returns>A task containing a list of <c>TicketingScreen</c> objects representing the screens of the bank. If the bank does not exists, an empty list is returned. If the operation fails, <c>null</c> is returned.</returns>
		public async static Task<List<TicketingScreen>?> GetScreensAsync(string bankName) {
			try {
				if (!Initialize())
					return null;

				var ret = new List<TicketingScreen>();

				string query = $"SELECT {ScreensConstants.SCREEN_ID}, {ScreensConstants.IS_ACTIVE}, {ScreensConstants.SCREEN_TITLE} FROM {ScreensConstants.TABLE_NAME} WHERE {ScreensConstants.BANK_NAME} = @bankName;";
				var command = new SqlCommand(query, connection);
				command.Parameters.Add("@bankName", SqlDbType.VarChar, BanksConstants.BANK_NAME_SIZE).Value = bankName;

				connection!.Open();

				var reader = await command.ExecuteReaderAsync();

				while (await reader.ReadAsync()) {
					ret.Add(new TicketingScreen(bankName, (int) reader[ScreensConstants.SCREEN_ID], (string) reader[ScreensConstants.SCREEN_TITLE], (bool) reader[ScreensConstants.IS_ACTIVE]));
				}

				reader.Close();

				return ret;
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex, "Screen ID");
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
			finally {
				connection?.Close();
			}

			return null;
		}

		/// <summary>
		/// Gets all the screens for the bank with the specified name.
		/// </summary>
		/// <param name="bankName">The name of the bank. Case insensitive.</param>
		/// <returns>A list of <c>TicketingScreen</c> objects representing the screens of the bank. If the bank does not exists, an empty list is returned. If the operation fails, <c>null</c> is returned.</returns>
		public static List<TicketingScreen>? GetScreens(string bankName) {
			try {
				if (!Initialize())
					return null;

				var ret = new List<TicketingScreen>();

				string query = $"SELECT {ScreensConstants.SCREEN_ID}, {ScreensConstants.IS_ACTIVE}, {ScreensConstants.SCREEN_TITLE} FROM {ScreensConstants.TABLE_NAME} WHERE {ScreensConstants.BANK_NAME} = @bankName;";
				var command = new SqlCommand(query, connection);
				command.Parameters.Add("@bankName", SqlDbType.VarChar, BanksConstants.BANK_NAME_SIZE).Value = bankName;

				connection!.Open();

				var reader = command.ExecuteReader();

				while (reader.Read()) {
					ret.Add(new TicketingScreen(bankName, (int) reader[ScreensConstants.SCREEN_ID], (string) reader[ScreensConstants.SCREEN_TITLE], (bool) reader[ScreensConstants.IS_ACTIVE]));
				}

				reader.Close();

				return ret;
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex, "Screen ID");
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
			finally {
				connection?.Close();
			}

			return null;
		}

		/// <summary>
		/// Gets the password for the specified bank.
		/// </summary>
		/// <param name="bankName">The name of the bank.</param>
		/// <returns>The password. If the bank does not exist, an empty string is returned. If the operation fails, <c>null</c> is returned.</returns>
		public static string? GetPassword(string bankName) {
			try {
				if (!Initialize())
					return null;

				string? password = string.Empty;

				string query = $"SELECT {BanksConstants.PASSWORD} FROM {BanksConstants.TABLE_NAME} WHERE {BanksConstants.BANK_NAME} = @bankName;";
				var command = new SqlCommand(query, connection);
				command.Parameters.Add("@bankName", SqlDbType.VarChar, BanksConstants.BANK_NAME_SIZE).Value = bankName;

				connection!.Open();

				var reader = command.ExecuteReader();

				if (reader.Read()) {
					password = reader.GetString(0);
				}

				return password;
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex, "Screen ID");
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
			finally {
				connection?.Close();
			}

			return null;
		}

		public static bool ChangePassword(string bankName, string newPassword) {
			try {
				if (!Initialize())
					return false;

				newPassword = newPassword.Trim();
				if (newPassword == string.Empty)
					return false;

				string query = $"UPDATE {BanksConstants.TABLE_NAME} SET {BanksConstants.PASSWORD} = @password WHERE {BanksConstants.BANK_NAME} = @bankName;";
				var command = new SqlCommand(query, connection);
				command.Parameters.Add("@password", SqlDbType.VarChar, BanksConstants.PASSWORD_SIZE).Value = newPassword;
				command.Parameters.Add("@bankName", SqlDbType.VarChar, BanksConstants.BANK_NAME_SIZE).Value = bankName;

				connection!.Open();
				command.ExecuteNonQuery();

				return true;
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex, "Screen ID");
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
			finally {
				connection?.Close();
			}

			return false;
		}
	}
}
