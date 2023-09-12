using System.Data.SqlClient;
using DataAccessLayer.Constants;
using DataAccessLayer.DataClasses;
using ExceptionUtils;

namespace DataAccessLayer {
	/// <summary>
	/// Contains methods for retrieving and manipulating bank information.
	/// </summary>
	public class BankOperations {
		public static readonly BankOperations Instance = new();
		private SqlConnection? connection;

		private BankOperations() {
			try {
				connection = null;
				connection = DBUtils.CreateConnection();
			}
			catch (Exception ex) {
				Console.Error.WriteLine($"Failed to establish connection. Message: {ex.Message}");
			}
		}

		/// <summary>
		/// Attempts to establish the connection again.
		/// </summary>
		/// <returns><c>true</c> if the connection was established successfully, and <c>false</c> otherwise.</returns>
		public bool ReinitializeConnection() {
			try {
				connection = DBUtils.CreateConnection();
				return true;
			}
			catch {
				return false;
			}
		}

		/// <summary>
		/// Verifies that the database connection is established correctly.
		/// </summary>
		/// <returns><c>true</c> if the connection has been established properly, and <c>false</c> otherwise.</returns>
		public bool VerifyConnection() {
			try {
				if (connection is null)
					return false;

				connection.Open();
				var command = new SqlCommand($"SELECT 1 FROM {BanksConstants.TABLE_NAME};", connection);

				var result = command.ExecuteScalar();
				if (result is null)
					return false;

				return (int) result == 1;
			}
			catch (Exception ex) {
				Console.Error.WriteLine($"Failed to establish connection. Message: {ex.Message}");
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
		public bool? CheckIfBankExists(string bankName) {
			try {
				if (connection is null)
					return null;

				string query = $"SELECT * FROM {BanksConstants.TABLE_NAME} WHERE {BanksConstants.BANK_NAME} = @bankName;";
				var command = new SqlCommand(query, connection);
				command.Parameters.AddWithValue("@bankName", bankName);

				connection.Open();
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
		public bool AddBank(Bank bank) {
			try {
				if (connection is null)
					return false;

				string query = $"INSERT INTO {BanksConstants.TABLE_NAME} ({BanksConstants.BANK_NAME}, {BanksConstants.PASSWORD}) VALUES (@bankName, @password);";
				SqlCommand command = new SqlCommand(query, connection);
				command.Parameters.AddWithValue("@bankName", bank.BankName);
				command.Parameters.AddWithValue("@password", bank.Password);

				connection.Open();
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
		/// Gets all the screens for the bank with the specified name.
		/// </summary>
		/// <param name="bankName">The name of the bank. Case insensitive.</param>
		/// <returns>A list of <c>TicketingScreen</c> objects representing the screens of the bank. If the bank does not exists, an empty list is returned. If the operation fails, <c>null</c> is returned.</returns>
		public List<TicketingScreen>? GetScreens(string bankName) {
			try {
				if (connection is null)
					return null;

				var ret = new List<TicketingScreen>();

				string query = $"SELECT {ScreensConstants.SCREEN_ID}, {ScreensConstants.IS_ACTIVE}, {ScreensConstants.SCREEN_TITLE} FROM {ScreensConstants.TABLE_NAME} WHERE {ScreensConstants.BANK_NAME} = @bankName;";
				var command = new SqlCommand(query, connection);
				command.Parameters.AddWithValue("@bankName", bankName);

				connection.Open();

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
		public string? GetPassword(string bankName) {
			try {
				if (connection is null)
					return null;

				string? password = string.Empty;

				string query = $"SELECT {BanksConstants.PASSWORD} FROM {BanksConstants.TABLE_NAME} WHERE {BanksConstants.BANK_NAME} = @bankName;";
				var command = new SqlCommand(query, connection);
				command.Parameters.AddWithValue("@bankName", bankName);

				connection.Open();

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
	}
}
