using DataAccessLayer.Constants;
using DataAccessLayer.DataClasses;
using DataAccessLayer.Utils;
using System.Data.SqlClient;
using System.Text;

namespace DataAccessLayer {
	/// <summary>
	/// Contains methods for retrieving and manipulating button information.
	/// </summary>
	public static class ButtonOperations {
		private static readonly SqlConnection connection = DBUtils.CreateConnection();

		/// <summary>
		/// Adds a button to the database.
		/// </summary>
		/// <param name="button">The button to be added to the database.</param>
		/// <returns>The ID of the screen that was added. If it's equal to -1, that means that the operation failed.</returns>
		public static int AddButton(TicketingButton button) {
			int buttonId = -1;

			try {
				string query = $"INSERT INTO {ButtonsConstants.TABLE_NAME} ({ButtonsConstants.BANK_NAME}, {ButtonsConstants.SCREEN_ID}, {ButtonsConstants.NAME_EN}, {ButtonsConstants.NAME_AR}, {ButtonsConstants.TYPE}, " +
					$"{ButtonsConstants.SERVICE}, {ButtonsConstants.MESSAGE_EN}, {ButtonsConstants.MESSAGE_AR}) VALUES (@bankName, @screenId, @nameEn, @nameAr, @type, @service, @messageEn, @messageAr); SELECT CAST(scope_identity() AS int);";
				var command = new SqlCommand(query, connection);
				command.Parameters.AddWithValue("@bankName", button.BankName);
				command.Parameters.AddWithValue("@screenId", button.ScreenId);
				command.Parameters.AddWithValue("@nameEn", button.NameEn);
				command.Parameters.AddWithValue("@nameAr", button.NameAr);
				command.Parameters.AddWithValue("@type", button.Type);
				if (button is ShowMessageButton showMessageButton) {
					command.Parameters.AddWithValue("@service", DBNull.Value);
					command.Parameters.AddWithValue("@messageEn", showMessageButton.MessageEn);
					command.Parameters.AddWithValue("@messageAr", showMessageButton.MessageAr);
				}
				else if (button is IssueTicketButton issueTicketButton) {
					command.Parameters.AddWithValue("@service", issueTicketButton.Service);
					command.Parameters.AddWithValue("@messageEn", DBNull.Value);
					command.Parameters.AddWithValue("@messageAr", DBNull.Value);
				}

				connection.Open();

				buttonId = (int) command.ExecuteScalar();
			}
			catch (SqlException ex) {
				DALExceptionHelper.HandleSqlException(ex, "Button ID");
			}
			catch (Exception ex) {
				DALExceptionHelper.HandleGeneralException(ex);
			}
			finally {
				connection.Close();
			}

			return buttonId;
		}

		/// <summary>
		/// Adds multiple buttons to the database.
		/// </summary>
		/// <param name="buttons">The list of buttons to be added to the database.</param>
		public static void AddButtons(List<TicketingButton> buttons) {
			try {
				var query = new StringBuilder($"INSERT INTO {ButtonsConstants.TABLE_NAME} ({ButtonsConstants.BANK_NAME}, {ButtonsConstants.SCREEN_ID}, {ButtonsConstants.NAME_EN}, " +
					$"{ButtonsConstants.NAME_AR}, {ButtonsConstants.TYPE}, {ButtonsConstants.SERVICE}, {ButtonsConstants.MESSAGE_EN}, {ButtonsConstants.MESSAGE_AR}) VALUES ");
				var command = new SqlCommand() { Connection = connection };

				int i = 0;
				foreach (var button in buttons) {
					query.Append('(');
					query.Append("@bankName").Append(i).Append(',');
					query.Append("@screenId").Append(i).Append(',');
					query.Append("@nameEn").Append(i).Append(',');
					query.Append("@nameAr").Append(i).Append(',');
					query.Append("@type").Append(i).Append(',');
					query.Append("@service").Append(i).Append(',');
					query.Append("@messageEn").Append(i).Append(',');
					query.Append("@messageAr").Append(i);
					query.Append("),");

					command.Parameters.AddWithValue("@bankName" + i, button.BankName);
					command.Parameters.AddWithValue("@screenId" + i, button.ScreenId);
					command.Parameters.AddWithValue("@type" + i, button.Type);
					command.Parameters.AddWithValue("@nameEn" + i, button.NameEn);
					command.Parameters.AddWithValue("@nameAr" + i, button.NameAr);
					if (button is ShowMessageButton showMessageButton) {
						command.Parameters.AddWithValue("@service" + i, DBNull.Value);
						command.Parameters.AddWithValue("@messageEn" + i, showMessageButton.MessageEn);
						command.Parameters.AddWithValue("@messageAr" + i, showMessageButton.MessageAr);
					}
					else if (button is IssueTicketButton issueTicketButton) {
						command.Parameters.AddWithValue("@service" + i, issueTicketButton.Service);
						command.Parameters.AddWithValue("@messageEn" + i, DBNull.Value);
						command.Parameters.AddWithValue("@messageAr" + i, DBNull.Value);
					}

					++i;
				}

				query.Length--;
				command.CommandText = query.ToString();

				command.ExecuteNonQuery();
			}
			catch (SqlException ex) {
				DALExceptionHelper.HandleSqlException(ex, "Button ID");
			}
			catch (Exception ex) {
				DALExceptionHelper.HandleGeneralException(ex);
			}
			finally {
				connection.Close();
			}
		}

		/// <summary>
		/// Deletes the button with the given ID from the database.
		/// </summary>
		/// <param name="bankName">The name of the bank that owns the button.</param>
		/// <param name="screenId">The ID of the screen that contains the button.</param>
		/// <param name="buttonId">The ID of the button to be deleted.</param>
		public static void DeleteButton(string bankName, int screenId, int buttonId) {
			try {
				string query = $"DELETE FROM {ButtonsConstants.TABLE_NAME} WHERE {ButtonsConstants.BANK_NAME} = @bankName AND {ButtonsConstants.SCREEN_ID} = @screenId AND {ButtonsConstants.BUTTON_ID} = @buttonId;";
				var command = new SqlCommand(query, connection);
				command.Parameters.AddWithValue("@bankName", bankName);
				command.Parameters.AddWithValue("@screenId", screenId);
				command.Parameters.AddWithValue("@buttonId", buttonId);

				connection.Open();

				command.ExecuteNonQuery();
			}
			catch (SqlException ex) {
				DALExceptionHelper.HandleSqlException(ex, "Button ID");
			}
			catch (Exception ex) {
				DALExceptionHelper.HandleGeneralException(ex);
			}
			finally {
				connection.Close();
			}
		}

		/// <summary>
		/// Deletes the screens with the given IDs from the database.
		/// </summary>
		/// <param name="bankName">The name of the bank that owns the buttons.</param>
		/// <param name="screenId">The ID of the screen that contains the buttons.</param>
		/// <param name="buttonIds">A list containing the IDs of the buttons to be deleted.</param>
		public static void DeleteButtons(string bankName, int screenId, List<int> buttonIds) {
			try {
				var query = new StringBuilder($"DELETE FROM {ButtonsConstants.TABLE_NAME} WHERE {ButtonsConstants.BANK_NAME} = @bankName AND {ButtonsConstants.SCREEN_ID} = @screenId AND {ButtonsConstants.BUTTON_ID} IN (");
				var command = new SqlCommand() { Connection = connection };
				command.Parameters.AddWithValue("@bankName", bankName);
				command.Parameters.AddWithValue("@screenId", screenId);

				int i = 0;
				foreach (var buttonId in buttonIds) {
					query.Append("@buttonId").Append(i).Append(',');
					command.Parameters.AddWithValue("@buttonId" + i, buttonId);
					++i;
				}

				query.Length--;
				query.Append(");");
				command.CommandText = query.ToString();

				command.ExecuteNonQuery();
			}
			catch (SqlException ex) {
				DALExceptionHelper.HandleSqlException(ex, "Button ID");
			}
			catch (Exception ex) {
				DALExceptionHelper.HandleGeneralException(ex);
			}
			finally {
				connection.Close();
			}
		}

		/// <summary>
		/// Updates the button with the given ID to <c>newButton</c>.
		/// </summary>
		/// <param name="bankName">The name of the bank that owns the button.</param>
		/// <param name="screenId">The ID of the screen that contains the button.</param>
		/// <param name="buttonId">The ID of the button to be updated.</param>
		/// <param name="newButton">A <c>TicketingButton</c> object containing the updated button information.</param>
		public static void UpdateButton(string bankName, int screenId, int buttonId, TicketingButton newButton) {
			try {
				string query = $"UPDATE {ButtonsConstants.TABLE_NAME} SET {ButtonsConstants.NAME_EN} = @nameEn, {ButtonsConstants.NAME_AR} = @nameAr, {ButtonsConstants.TYPE} = @type, {ButtonsConstants.SERVICE} = @service, " +
					$"{ButtonsConstants.MESSAGE_EN} = @messageEn, {ButtonsConstants.MESSAGE_AR} = @messageAr WHERE {ButtonsConstants.BANK_NAME} = @bankName AND {ButtonsConstants.SCREEN_ID} = @screenId AND {ButtonsConstants.BUTTON_ID} = @buttonId;";
				var command = new SqlCommand(query, connection);
				command.Parameters.AddWithValue("@bankName", bankName);
				command.Parameters.AddWithValue("@screenId", screenId);
				command.Parameters.AddWithValue("@buttonId", buttonId);
				command.Parameters.AddWithValue("@nameEn", newButton.NameEn);
				command.Parameters.AddWithValue("@nameAr", newButton.NameAr);
				command.Parameters.AddWithValue("@type", newButton.Type);
				if (newButton is ShowMessageButton showMessageButton) {
					command.Parameters.AddWithValue("@service", DBNull.Value);
					command.Parameters.AddWithValue("@messageEn", showMessageButton.MessageEn);
					command.Parameters.AddWithValue("@messageAr", showMessageButton.MessageAr);
				}
				else if (newButton is IssueTicketButton issueTicketButton) {
					command.Parameters.AddWithValue("@service", issueTicketButton.Service);
					command.Parameters.AddWithValue("@messageEn", DBNull.Value);
					command.Parameters.AddWithValue("@messageAr", DBNull.Value);
				}

				connection.Open();

				command.ExecuteNonQuery();
			}
			catch (SqlException ex) {
				DALExceptionHelper.HandleSqlException(ex, "Button ID");
			}
			catch (Exception ex) {
				DALExceptionHelper.HandleGeneralException(ex);
			}
			finally {
				connection.Close();
			}
		}

		/// <summary>
		/// Updates multiple buttons given a dictionary containing button IDs and corresponding updated buttons.
		/// </summary>
		/// <param name="bankName">The name of the bank that owns the buttons.</param>
		/// <param name="screenId">The ID of the screen that contains the buttons.</param>
		/// <param name="buttons">A dictionary where the keys are button IDs and the corresponding values are the updated buttons.</param>
		public static void UpdateButtons(string bankName, int screenId, Dictionary<int, TicketingButton> buttons) {
			try {
				var query = new StringBuilder($"UPDATE {ButtonsConstants.TABLE_NAME} SET ");
				var command = new SqlCommand() { Connection = connection };

				var fields = new Dictionary<string, StringBuilder>()
				{
					{"@type", new StringBuilder($"{ButtonsConstants.TYPE} = CASE {ButtonsConstants.BUTTON_ID} ")},
					{"@nameEn", new StringBuilder($"{ButtonsConstants.NAME_EN} = CASE {ButtonsConstants.BUTTON_ID} ")},
					{"@nameAr", new StringBuilder($"{ButtonsConstants.NAME_AR} = CASE {ButtonsConstants.BUTTON_ID} ")},
					{"@service", new StringBuilder($"{ButtonsConstants.SERVICE} = CASE {ButtonsConstants.BUTTON_ID} ")},
					{"@messageEn", new StringBuilder($"{ButtonsConstants.MESSAGE_EN} = CASE {ButtonsConstants.BUTTON_ID} ")},
					{"@messageAr", new StringBuilder($"{ButtonsConstants.MESSAGE_AR} = CASE {ButtonsConstants.BUTTON_ID} ")},
				};

				var buttonIds = new StringBuilder("");

				int i = 0;
				foreach (var button in buttons) {
					foreach (var field in fields)
						field.Value.Append($" WHEN @buttonId").Append(i).Append(" THEN ").Append(field.Key).Append(i);

					command.Parameters.AddWithValue("@buttonId" + i, button.Key);
					command.Parameters.AddWithValue("@nameEn" + i, button.Value.NameEn);
					command.Parameters.AddWithValue("@nameAr" + i, button.Value.NameAr);
					command.Parameters.AddWithValue("@type" + i, button.Value.Type);
					if (button.Value is ShowMessageButton showMessageButton) {
						command.Parameters.AddWithValue("@service" + i, DBNull.Value);
						command.Parameters.AddWithValue("@messageEn" + i, showMessageButton.MessageEn);
						command.Parameters.AddWithValue("@messageAr" + i, showMessageButton.MessageAr);
					}
					else if (button.Value is IssueTicketButton issueTicketButton) {
						command.Parameters.AddWithValue("@service" + i, issueTicketButton.Service);
						command.Parameters.AddWithValue("@messageEn" + i, DBNull.Value);
						command.Parameters.AddWithValue("@messageAr" + i, DBNull.Value);
					}

					buttonIds.Append("@buttonId" + i).Append(',');

					++i;
				}

				foreach (var field in fields) {
					field.Value.Append(" ELSE @default END,");
					query.Append(field.Value);
				}

				buttonIds.Length--;
				query.Length--;

				query.Append($" WHERE {ButtonsConstants.BANK_NAME} = @bankName AND {ButtonsConstants.SCREEN_ID} = @screenId AND {ButtonsConstants.BUTTON_ID} IN (").Append(buttonIds).Append(");");

				command.Parameters.AddWithValue("@bankName", bankName);
				command.Parameters.AddWithValue("@screenId", screenId);
				command.Parameters.AddWithValue("@default", "Update error");

				command.CommandText = query.ToString();

				connection.Open();

				command.ExecuteNonQuery();
			}
			catch (SqlException ex) {
				DALExceptionHelper.HandleSqlException(ex, "Button ID");
			}
			catch (Exception ex) {
				DALExceptionHelper.HandleGeneralException(ex);
			}
			finally {
				connection.Close();
			}
		}

		/// <summary>
		/// Gets the button with the given bank name, screen ID, and button ID.
		/// </summary>
		/// <param name="bankName">The name of the bank that owns the button.</param>
		/// <param name="screenId">The ID of the screen that contains the button.</param>
		/// <param name="buttonId">The ID of the button.</param>
		/// <returns>A <c>TicketingButton</c> object representing the button. If the button does not exist, <c>null</c> is returned.</returns>
		public static TicketingButton? GetButtonById(string bankName, int screenId, int buttonId) {
			TicketingButton? button = null;

			try {
				string query = $"SELECT {ButtonsConstants.NAME_EN}, {ButtonsConstants.NAME_AR}, {ButtonsConstants.TYPE}, {ButtonsConstants.SERVICE}, {ButtonsConstants.MESSAGE_EN}, {ButtonsConstants.MESSAGE_AR} " +
					$"FROM {ButtonsConstants.TABLE_NAME} WHERE {ButtonsConstants.BANK_NAME} = @bankName AND {ButtonsConstants.SCREEN_ID} = @screenId AND {ButtonsConstants.BUTTON_ID} = @buttonId;";
				var command = new SqlCommand(query, connection);
				command.Parameters.AddWithValue("@bankName", bankName);
				command.Parameters.AddWithValue("@screenId", screenId);
				command.Parameters.AddWithValue("@buttonId", buttonId);

				connection.Open();

				var reader = command.ExecuteReader();

				if (reader.Read()) {
					string nameEn = (string) reader[ButtonsConstants.NAME_EN];
					string nameAr = (string) reader[ButtonsConstants.NAME_AR];
					string type = (string) reader[ButtonsConstants.TYPE];

					if (type == ButtonsConstants.Types.ISSUE_TICKET) {
						string service = (string) reader[ButtonsConstants.SERVICE];

						button = new IssueTicketButton(bankName, screenId, buttonId, type, nameEn, nameAr, service);
					}
					else if (type == ButtonsConstants.Types.SHOW_MESSAGE) {
						string messageEn = (string) reader[ButtonsConstants.MESSAGE_EN];
						string messageAr = (string) reader[ButtonsConstants.MESSAGE_AR];

						button = new ShowMessageButton(bankName, screenId, buttonId, type, nameEn, nameAr, messageEn, messageAr);
					}
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

			return button;
		}

		/// <summary>
		/// Checks if the button with the given bank name, screen ID, and button ID exists in the database.
		/// </summary>
		/// <param name="bankName">The name of the bank that owns the button.</param>
		/// <param name="screenId">The ID of the screen that contains the button.</param>
		/// <param name="buttonId">The ID of the button.</param>
		/// <returns>Returns <c>true</c> if a matching button exists, and <c>false</c> otherwise.</returns>
		public static bool CheckIfButtonExists(string bankName, int screenId, int buttonId) {
			bool exists = false;

			try {
				string query = $"SELECT COUNT({ButtonsConstants.BUTTON_ID}) FROM {ButtonsConstants.TABLE_NAME} WHERE {ButtonsConstants.BANK_NAME} = @bankName AND {ButtonsConstants.SCREEN_ID} = @screenId AND {ButtonsConstants.BUTTON_ID} = @buttonId;";
				var command = new SqlCommand(query, connection);
				command.Parameters.AddWithValue("@bankName", bankName);
				command.Parameters.AddWithValue("@screenId", screenId);
				command.Parameters.AddWithValue("@buttonId", buttonId);

				connection.Open();

				exists = (int) command.ExecuteScalar() == 1;
			}
			catch (SqlException ex) {
				DALExceptionHelper.HandleSqlException(ex, "Button ID");
			}
			catch (Exception ex) {
				DALExceptionHelper.HandleGeneralException(ex);
			}
			finally {
				connection.Close();
			}

			return exists;
		}
	}
}
