using System.Data.SqlClient;
using DataAccessLayer.Constants;
using DataAccessLayer.DataClasses;
using DataAccessLayer.Utils;

namespace DataAccessLayer {
	/// <summary>
	/// Contains methods for retrieving and manipulating bank information.
	/// </summary>
	public static class BankOperations {
		private static readonly SqlConnection connection = DBUtils.CreateConnection();

		/// <summary>
		/// Checks if the bank with the specified <c>bankName</c> exists in the database.
		/// </summary>
		/// <param name="bankName">The name of the bank. Case insensitive.</param>
		/// <returns><c>true</c> if the bank exists in the database, and <c>false</c> otherwise.</returns>
		public static bool CheckIfBankExists(string bankName) {
			bool exists = false;

			try {
				string query = $"SELECT * FROM {BanksConstants.TABLE_NAME} WHERE {BanksConstants.BANK_NAME} = @bankName;";
				var command = new SqlCommand(query, connection);
				command.Parameters.AddWithValue("@bankName", bankName);

				connection.Open();
				exists = command.ExecuteReader().HasRows;
			}
			catch (SqlException ex) {
				DALExceptionHelper.HandleSqlException(ex, "Bank Name");
			}
			catch (Exception ex) {
				DALExceptionHelper.HandleGeneralException(ex);
			}
			finally {
				connection.Close();
			}

			return exists;
		}

		/// <summary>
		/// Adds a bank to the database.
		/// </summary>
		/// <param name="bank">The bank to be added to the database.</param>
		public static void AddBank(Bank bank) {
			try {
				string query = $"INSERT INTO {BanksConstants.TABLE_NAME} ({BanksConstants.BANK_NAME}, {BanksConstants.PASSWORD}) VALUES (@bankName, @password);";
				SqlCommand command = new SqlCommand(query, connection);
				command.Parameters.AddWithValue("@bankName", bank.BankName);
				command.Parameters.AddWithValue("@password", bank.Password);

				connection.Open();
				command.ExecuteNonQuery();
			}
			catch (SqlException ex) {
				DALExceptionHelper.HandleSqlException(ex, "Bank Name");
			}
			catch (Exception ex) {
				DALExceptionHelper.HandleGeneralException(ex);
			}
			finally {
				connection.Close();
			}
		}

		/// <summary>
		/// Gets all the screens for the bank with the specified name.
		/// </summary>
		/// <param name="bankName">The name of the bank. Case insensitive.</param>
		/// <returns>A list of <c>TicketingScreen</c> objects representing the screens of the bank. If the bank does not exists, an empty list is returned.</returns>
		public static List<TicketingScreen> GetScreens(string bankName) {
			var ret = new List<TicketingScreen>();

			try {
				string query = $"SELECT {ScreensConstants.SCREEN_ID}, {ScreensConstants.IS_ACTIVE}, {ScreensConstants.SCREEN_TITLE} FROM {ScreensConstants.TABLE_NAME} WHERE {ScreensConstants.BANK_NAME} = @bankName;";
				var command = new SqlCommand(query, connection);
				command.Parameters.AddWithValue("@bankName", bankName);

				connection.Open();

				var reader = command.ExecuteReader();

				while (reader.Read()) {
					ret.Add(new TicketingScreen(bankName, (int) reader[ScreensConstants.SCREEN_ID], (string) reader[ScreensConstants.SCREEN_TITLE], (bool) reader[ScreensConstants.IS_ACTIVE]));
				}

				reader.Close();
			}
			catch (SqlException ex) {
				DALExceptionHelper.HandleSqlException(ex, "Screen ID");
			}
			catch (Exception ex) {
				DALExceptionHelper.HandleGeneralException(ex);
			}
			finally {
				connection.Close();
			}

			return ret;
		}

		/// <summary>
		/// Gets the password for the specified bank.
		/// </summary>
		/// <param name="bankName">The name of the bank.</param>
		/// <returns>The password. If the bank does not exist, an empty string is returned.</returns>
		public static string GetPassword(string bankName) {
			string password = string.Empty;

			try {
				string query = $"SELECT {BanksConstants.PASSWORD} FROM {BanksConstants.TABLE_NAME} WHERE {BanksConstants.BANK_NAME} = @bankName;";
				var command = new SqlCommand(query, connection);
				command.Parameters.AddWithValue("@bankName", bankName);

				connection.Open();

				var reader = command.ExecuteReader();

				if (reader.Read()) {
					password = reader.GetString(0);
				}
			}
			catch (SqlException ex) {
				DALExceptionHelper.HandleSqlException(ex, "Screen ID");
			}
			catch (Exception ex) {
				DALExceptionHelper.HandleGeneralException(ex);
			}
			finally {
				connection.Close();
			}

			return password;
		}
	}
}
