using System.Data.SqlClient;
using DataAccessLayer.Constants;
using DataAccessLayer.DataClasses;
using DataAccessLayer.Utils;
using ExceptionUtils;
using System.Data;
using LogUtils;

namespace DataAccessLayer {
	/// <summary>
	/// Contains methods for retrieving and manipulating bank information.
	/// </summary>
	public static class BankOperations {
		/// <summary>
		/// Verifies that the database connection is established correctly.
		/// </summary>
		/// <returns><c>true</c> if the connection has been established properly, and <c>false</c> otherwise.</returns>
		public static bool VerifyConnection() {
			try {
				return DBUtils.VerifyConnection();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return false;
			}
		}

		/// <summary>
		/// Checks if the bank with the specified <c>bankName</c> exists in the database.
		/// </summary>
		/// <param name="bankName">The name of the bank. Case insensitive.</param>
		/// <returns><c>true</c> if the bank exists in the database, and <c>false</c> if it does not. If the operation fails, <c>null</c> is returned.</returns>
		public static bool? CheckIfBankExists(string bankName) {
			try {
				string query = $"SELECT COUNT({BanksConstants.BANK_NAME}) FROM {BanksConstants.TABLE_NAME} WHERE {BanksConstants.BANK_NAME} = @bankName;";
				var command = new SqlCommand(query);
				command.Parameters.Add("@bankName", SqlDbType.VarChar, BanksConstants.BANK_NAME_SIZE).Value = bankName;

				return (int) DBUtils.ExecuteScalar(command) == 1;
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
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
				string query = $"INSERT INTO {BanksConstants.TABLE_NAME} ({BanksConstants.BANK_NAME}, {BanksConstants.PASSWORD}) VALUES (@bankName, @password);";
				SqlCommand command = new SqlCommand(query);
				command.Parameters.Add("@bankName", SqlDbType.VarChar, BanksConstants.BANK_NAME_SIZE).Value = bank.BankName;
				command.Parameters.Add("@password", SqlDbType.VarChar, BanksConstants.PASSWORD_SIZE).Value = bank.Password;

				return DBUtils.ExecuteNonQuery(command) == 1;
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
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
				var ret = new List<TicketingScreen>();

				string query = $"SELECT {ScreensConstants.SCREEN_ID}, {ScreensConstants.IS_ACTIVE}, {ScreensConstants.SCREEN_TITLE} FROM {ScreensConstants.TABLE_NAME} WHERE {ScreensConstants.BANK_NAME} = @bankName;";
				var command = new SqlCommand(query);
				command.Parameters.Add("@bankName", SqlDbType.VarChar, BanksConstants.BANK_NAME_SIZE).Value = bankName;

				await DBUtils.ExecuteReaderAsync(command, async reader => {
					while (await reader.ReadAsync()) {
						ret.Add(new TicketingScreen(bankName, (int) reader[ScreensConstants.SCREEN_ID], (string) reader[ScreensConstants.SCREEN_TITLE], (bool) reader[ScreensConstants.IS_ACTIVE]));
					}

					reader.Close();
				});

				if (ret.Count == 0) {
					LogsHelper.Log("BankOperations.GetScreensAsync is returning empty", DateTime.Now, EventSeverity.Info);
				}

				return ret;
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
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
				string? password = string.Empty;

				string query = $"SELECT {BanksConstants.PASSWORD} FROM {BanksConstants.TABLE_NAME} WHERE {BanksConstants.BANK_NAME} = @bankName;";
				var command = new SqlCommand(query);
				command.Parameters.Add("@bankName", SqlDbType.VarChar, BanksConstants.BANK_NAME_SIZE).Value = bankName;

				DBUtils.ExecuteReader(command, reader => {
					if (reader.Read()) {
						password = reader.GetString(0);
					}
				});

				return password;
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}

			return null;
		}

		/// <summary>
		/// Changes the password for the specifed bank.
		/// </summary>
		/// <param name="bankName">The name of the bank.</param>
		/// <param name="newPassword">The new password.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public static bool ChangePassword(string bankName, string newPassword) {
			try {
				newPassword = newPassword.Trim();
				if (newPassword == string.Empty)
					return false;

				string query = $"UPDATE {BanksConstants.TABLE_NAME} SET {BanksConstants.PASSWORD} = @password WHERE {BanksConstants.BANK_NAME} = @bankName;";
				var command = new SqlCommand(query);
				command.Parameters.Add("@password", SqlDbType.VarChar, BanksConstants.PASSWORD_SIZE).Value = newPassword;
				command.Parameters.Add("@bankName", SqlDbType.VarChar, BanksConstants.BANK_NAME_SIZE).Value = bankName;

				DBUtils.ExecuteNonQuery(command);

				return true;
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}

			return false;
		}
	}
}
