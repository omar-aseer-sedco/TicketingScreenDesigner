using DataAccessLayer.Constants;
using DataAccessLayer.DataClasses;
using DataAccessLayer.Utils;
using ExceptionUtils;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace DataAccessLayer.DBOperations {
	/// <summary>
	/// Contains methods for retrieving and manipulating screen information.
	/// </summary>
	public static class ScreenOperations {
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
		/// Adds a screen to the database.
		/// </summary>
		/// <param name="screen">The screen to be added to the database</param>
		/// <returns>The ID of the screen that was added. If the operation fails, <c>null</c> is returned.</returns>
		public static int? AddScreen(TicketingScreen screen) {
			try {
				if (screen.IsActive) {
					int? activeScreenId = GetActiveScreenId(screen.BankName);
					if (activeScreenId is null)
						return null;
					if (activeScreenId != -1)
						SetIsActive(screen.BankName, (int) activeScreenId, false);
				}

				string query = $"INSERT INTO {ScreensConstants.TABLE_NAME} ({ScreensConstants.BANK_NAME}, {ScreensConstants.IS_ACTIVE}, {ScreensConstants.SCREEN_TITLE}) VALUES (@bankName, @isActive, @screenTitle); SELECT CAST(IDENT_CURRENT('{ScreensConstants.TABLE_NAME}') AS int);";
				var command = new SqlCommand(query);
				command.Parameters.Add("@bankName", SqlDbType.VarChar, ScreensConstants.BANK_NAME_SIZE).Value = screen.BankName;
				command.Parameters.Add("@isActive", SqlDbType.Bit).Value = screen.IsActive;
				command.Parameters.Add("@screenTitle", SqlDbType.VarChar, ScreensConstants.SCREEN_TITLE_SIZE).Value = screen.ScreenTitle;

				int result = (int) DBUtils.ExecuteScalar(command);
				return result == -1 ? null : result;
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
		/// Gets the ID of the active screen.
		/// </summary>
		/// <param name="bankName">The name of the bank.</param>
		/// <returns>The ID of the active screen. If there is no active screen, <c>-1</c> is returned. If the operation fails, <c>null</c> is returned.</returns>
		public static int? GetActiveScreenId(string bankName) {
			try {
				int? ret = -1;

				string query = $"SELECT {ScreensConstants.SCREEN_ID} FROM {ScreensConstants.TABLE_NAME} WHERE {ScreensConstants.BANK_NAME} = @bankName AND {ScreensConstants.IS_ACTIVE} = 1;";
				var command = new SqlCommand(query);
				command.Parameters.Add("@bankName", SqlDbType.VarChar, ScreensConstants.BANK_NAME_SIZE).Value = bankName;

				DBUtils.ExecuteReader(command, reader => {
					if (reader.Read())
						ret = reader.GetInt32(0);
				});

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
		/// Sets the screen with the given bank name and ID to active or inactive. If another screen is already active, it gets deactivated.
		/// </summary>
		/// <param name="bankName">The name of the bank that owns the screen.</param>
		/// <param name="screenId">The ID of the screen.</param>
		/// <param name="active"><c>true</c> to set the screen to active, and <c>false</c> to set it to inactive.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public static bool SetIsActive(string bankName, int screenId, bool active) {
			try {
				int? currentlyActiveScreenId = GetActiveScreenId(bankName);
				if (currentlyActiveScreenId is null) {
					return false;
				}

				if (!active && currentlyActiveScreenId == -1) {
					return true;
				}

				if (active && currentlyActiveScreenId != screenId) {
					if (!SetIsActive(bankName, (int) currentlyActiveScreenId, false)) {
						return false;
					}
				}

				string query = $"UPDATE {ScreensConstants.TABLE_NAME} SET {ScreensConstants.IS_ACTIVE} = @isActive WHERE {ScreensConstants.BANK_NAME} = @bankName AND {ScreensConstants.SCREEN_ID} = @screenId";
				var command = new SqlCommand(query);
				command.Parameters.Add("@isActive", SqlDbType.Bit).Value = active;
				command.Parameters.Add("@bankName", SqlDbType.VarChar, ScreensConstants.BANK_NAME_SIZE).Value = bankName;
				command.Parameters.Add("@screenId", SqlDbType.Int).Value = screenId;

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
		/// Deletes the screens with the given IDs from the database.
		/// </summary>
		/// <param name="bankName">The name of the bank that owns the screens.</param>
		/// <param name="screenIds">The IDs of the screen to be deleted</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public static bool DeleteScreens(string bankName, List<int> screenIds) {
			try {
				var query = new StringBuilder($"DELETE FROM {ScreensConstants.TABLE_NAME} WHERE {ScreensConstants.BANK_NAME} = @bankName AND {ScreensConstants.SCREEN_ID} IN (");
				var command = new SqlCommand();
				command.Parameters.Add("@bankName", SqlDbType.VarChar, ScreensConstants.BANK_NAME_SIZE).Value = bankName;

				int i = 0;
				foreach (var screenId in screenIds) {
					query.Append("@screenId").Append(i).Append(',');
					command.Parameters.Add("@screenId" + i, SqlDbType.Int).Value = screenId;

					++i;
				}

				query.Length--;
				query.Append(");");
				command.CommandText = query.ToString();

				return DBUtils.ExecuteNonQuery(command) == screenIds.Count;
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
		/// Updates the screen with the given ID to <c>newScreen</c>.
		/// </summary>
		/// <param name="bankName">The name of the bank that owns the screen.</param>
		/// <param name="screenId">The ID of the screen.</param>
		/// <param name="newScreen">A <c>TicketingScreen</c> object containing the updated screen information.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public static bool UpdateScreen(string bankName, int screenId, TicketingScreen newScreen) {
			try {
				int? activeScreenId = GetActiveScreenId(bankName);
				if (activeScreenId is not null && activeScreenId != screenId) {
					SetIsActive(bankName, (int) activeScreenId, false);
				}

				string query = $"UPDATE {ScreensConstants.TABLE_NAME} SET {ScreensConstants.SCREEN_TITLE} = @screenTitle, {ScreensConstants.IS_ACTIVE} = @isActive WHERE {ScreensConstants.SCREEN_ID} = @screenId AND {ScreensConstants.BANK_NAME} = @bankName";
				var command = new SqlCommand(query);
				command.Parameters.Add("@screenTitle", SqlDbType.VarChar, ScreensConstants.SCREEN_TITLE_SIZE).Value = newScreen.ScreenTitle;
				command.Parameters.Add("@isActive", SqlDbType.Bit).Value = newScreen.IsActive;
				command.Parameters.Add("@screenId", SqlDbType.Int).Value = screenId;
				command.Parameters.Add("@bankName", SqlDbType.VarChar, ScreensConstants.BANK_NAME_SIZE).Value = bankName;

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
		/// Checks if the screen with the given bank name and ID exists.
		/// </summary>
		/// <param name="bankName">The name of the bank that owns the screen.</param>
		/// <param name="screenId">The ID of the screen.</param>
		/// <returns><c>true</c> if a matching screen exists, and <c>false</c> if it does not. If the operation fails, <c>null</c> is returned.</returns>
		public static bool? CheckIfScreenExists(string bankName, int screenId) {
			try {
				string query = $"SELECT COUNT({ScreensConstants.SCREEN_ID}) FROM {ScreensConstants.TABLE_NAME} WHERE {ScreensConstants.BANK_NAME} = @bankName AND {ScreensConstants.SCREEN_ID} = @screenId;";
				var command = new SqlCommand(query);
				command.Parameters.Add("@bankName", SqlDbType.VarChar, ScreensConstants.BANK_NAME_SIZE).Value = bankName;
				command.Parameters.Add("@screenId", SqlDbType.Int).Value = screenId;

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
		/// Asynchronously gets all the buttons associated with the specified screen.
		/// </summary>
		/// <param name="bankName">The name of the bank that owns the screen.</param>
		/// <param name="screenId">The ID of the screen.</param>
		/// <returns>A list of <c>TicketingButton</c> objects representing the buttons on the screen. If there are no buttons, an empty list is returned. If the operation fails, <c>null</c> is returned.</returns>
		public static async Task<List<TicketingButton>?> GetButtonsAsync(string bankName, int screenId) {
			try {
				var buttons = new List<TicketingButton>();

				string query = $"SELECT {ButtonsConstants.BUTTON_ID}, {ButtonsConstants.NAME_EN}, {ButtonsConstants.NAME_AR}, {ButtonsConstants.TYPE}, {ButtonsConstants.SERVICE}, {ButtonsConstants.MESSAGE_EN}, " +
					$"{ButtonsConstants.MESSAGE_AR} FROM {ButtonsConstants.TABLE_NAME} WHERE {ButtonsConstants.BANK_NAME} = @bankName AND {ButtonsConstants.SCREEN_ID} = @screenId";
				var command = new SqlCommand(query);
				command.Parameters.Add("@bankName", SqlDbType.VarChar, ButtonsConstants.BANK_NAME_SIZE).Value = bankName;
				command.Parameters.Add("@screenId", SqlDbType.Int).Value = screenId;

				await DBUtils.ExecuteReaderAsync(command, async reader => {
					while (await reader.ReadAsync()) {
						int buttonId = (int) reader[ButtonsConstants.BUTTON_ID];
						string nameEn = (string) reader[ButtonsConstants.NAME_EN];
						string nameAr = (string) reader[ButtonsConstants.NAME_AR];
						ButtonsConstants.Types type = (ButtonsConstants.Types) reader[ButtonsConstants.TYPE];

						if (type == ButtonsConstants.Types.ISSUE_TICKET) {
							string service = (string) reader[ButtonsConstants.SERVICE];

							buttons.Add(new IssueTicketButton(bankName, screenId, buttonId, type, nameEn, nameAr, service));
						}
						else if (type == ButtonsConstants.Types.SHOW_MESSAGE) {
							string messageEn = (string) reader[ButtonsConstants.MESSAGE_EN];
							string messageAr = (string) reader[ButtonsConstants.MESSAGE_AR];

							buttons.Add(new ShowMessageButton(bankName, screenId, buttonId, type, nameEn, nameAr, messageEn, messageAr));
						}
					}

					reader.Close();
				});

				return buttons;
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}

			return null;
		}
	}
}
