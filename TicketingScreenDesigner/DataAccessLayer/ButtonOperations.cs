using DataAccessLayer.Constants;
using DataAccessLayer.DataClasses;
using ExceptionUtils;
using System.Data.SqlClient;
using System.Text;

namespace DataAccessLayer {
	/// <summary>
	/// Contains methods for retrieving and manipulating button information.
	/// </summary>
	public class ButtonOperations {
		public static readonly ButtonOperations Instance = new();
		private SqlConnection? connection;

		private ButtonOperations() {
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
				var command = new SqlCommand($"SELECT 1 FROM {ButtonsConstants.TABLE_NAME};", connection);

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
		/// Adds a button to the database.
		/// </summary>
		/// <param name="button">The button to be added to the database.</param>
		/// <returns>The ID of the screen that was added. If the operation fails, <c>null</c> is returned.</returns>
		public int? AddButton(TicketingButton button) {
			try {
				if (connection is null)
					return null;

				string query = $"INSERT INTO {ButtonsConstants.TABLE_NAME} ({ButtonsConstants.BANK_NAME}, {ButtonsConstants.SCREEN_ID}, {ButtonsConstants.NAME_EN}, {ButtonsConstants.NAME_AR}, {ButtonsConstants.TYPE}, " +
					$"{ButtonsConstants.SERVICE}, {ButtonsConstants.MESSAGE_EN}, {ButtonsConstants.MESSAGE_AR}) VALUES (@bankName, @screenId, @nameEn, @nameAr, @type, @service, @messageEn, @messageAr); SELECT CAST(scope_identity() AS int);";
				var command = new SqlCommand(query, Instance.connection);
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

				return (int) command.ExecuteScalar();
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex, "Button ID");
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
		/// Adds multiple buttons to the database.
		/// </summary>
		/// <param name="buttons">The list of buttons to be added to the database.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public bool AddButtons(List<TicketingButton> buttons) {
			try {
				if (connection is null)
					return false;

				if (buttons.Count == 0)
					return true;

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
					command.Parameters.AddWithValue("@nameEn" + i, button.NameEn);
					command.Parameters.AddWithValue("@nameAr" + i, button.NameAr);
					command.Parameters.AddWithValue("@type" + i, button.Type);
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

				connection.Open();
				return command.ExecuteNonQuery() == buttons.Count;
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex, "Button ID");
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
		/// Adds multiple buttons to the screen with the given ID.
		/// </summary>
		/// <param name="screenId">The ID of the screen.</param>
		/// <param name="buttons">A list of buttons to be added to the database.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public bool AddButtons(int screenId, List<TicketingButton> buttons) {
			try {
				if (connection is null)
					return false;

				if (buttons.Count == 0)
					return true;

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
					command.Parameters.AddWithValue("@screenId" + i, screenId);
					command.Parameters.AddWithValue("@nameEn" + i, button.NameEn);
					command.Parameters.AddWithValue("@nameAr" + i, button.NameAr);
					command.Parameters.AddWithValue("@type" + i, button.Type);
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

				connection.Open();
				return command.ExecuteNonQuery() == buttons.Count;
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex, "Button ID");
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
		/// Deletes the button with the given ID from the database.
		/// </summary>
		/// <param name="bankName">The name of the bank that owns the button.</param>
		/// <param name="screenId">The ID of the screen that contains the button.</param>
		/// <param name="buttonId">The ID of the button to be deleted.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public bool DeleteButton(string bankName, int screenId, int buttonId) {
			try {
				if (connection is null)
					return false;

				string query = $"DELETE FROM {ButtonsConstants.TABLE_NAME} WHERE {ButtonsConstants.BANK_NAME} = @bankName AND {ButtonsConstants.SCREEN_ID} = @screenId AND {ButtonsConstants.BUTTON_ID} = @buttonId;";
				var command = new SqlCommand(query, connection);
				command.Parameters.AddWithValue("@bankName", bankName);
				command.Parameters.AddWithValue("@screenId", screenId);
				command.Parameters.AddWithValue("@buttonId", buttonId);

				connection.Open();

				return command.ExecuteNonQuery() == 1;
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex, "Button ID");
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
		/// Deletes the screens with the given IDs from the database.
		/// </summary>
		/// <param name="bankName">The name of the bank that owns the buttons.</param>
		/// <param name="screenId">The ID of the screen that contains the buttons.</param>
		/// <param name="buttonIds">A list containing the IDs of the buttons to be deleted.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public bool DeleteButtons(string bankName, int screenId, List<int> buttonIds) {
			try {
				if (connection is null)
					return false;

				if (buttonIds.Count == 0)
					return true;

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

				connection.Open();
				return command.ExecuteNonQuery() == buttonIds.Count;
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex, "Button ID");
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
		/// Updates the button with the given ID to <c>newButton</c>.
		/// </summary>
		/// <param name="bankName">The name of the bank that owns the button.</param>
		/// <param name="screenId">The ID of the screen that contains the button.</param>
		/// <param name="buttonId">The ID of the button to be updated.</param>
		/// <param name="newButton">A <c>TicketingButton</c> object containing the updated button information.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public bool UpdateButton(string bankName, int screenId, int buttonId, TicketingButton newButton) {
			try {
				if (connection is null)
					return false;

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

				return command.ExecuteNonQuery() == 1;
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex, "Button ID");
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
		/// Updates multiple buttons given a dictionary containing button IDs and corresponding updated buttons.
		/// </summary>
		/// <param name="bankName">The name of the bank that owns the buttons.</param>
		/// <param name="screenId">The ID of the screen that contains the buttons.</param>
		/// <param name="buttons">A dictionary where the keys are button IDs and the corresponding values are the updated buttons.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public bool UpdateButtons(string bankName, int screenId, Dictionary<int, TicketingButton> buttons) {
			try {
				if (connection is null)
					return false;

				if (buttons.Count == 0)
					return true;

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

				return command.ExecuteNonQuery() == buttons.Count;
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex, "Button ID");
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
		/// Gets the button with the given bank name, screen ID, and button ID.
		/// </summary>
		/// <param name="bankName">The name of the bank that owns the button.</param>
		/// <param name="screenId">The ID of the screen that contains the button.</param>
		/// <param name="buttonId">The ID of the button.</param>
		/// <returns>A <c>TicketingButton</c> object representing the button. If the button does not exist, <EmptyButton.Value cref="EmptyButton.Value"/> is returned. If the operation fails, <c>null</c> is returned.</returns>
		public TicketingButton? GetButtonById(string bankName, int screenId, int buttonId) {
			try {
				if (connection is null)
					return null;

				TicketingButton? button = EmptyButton.Value;

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

				return button;
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
		/// Checks if the button with the given bank name, screen ID, and button ID exists in the database.
		/// </summary>
		/// <param name="bankName">The name of the bank that owns the button.</param>
		/// <param name="screenId">The ID of the screen that contains the button.</param>
		/// <param name="buttonId">The ID of the button.</param>
		/// <returns><c>true</c> if a matching button exists, and <c>false</c> if it does not. If the operation fails, <c>null</c> is returned.</returns>
		public bool? CheckIfButtonExists(string bankName, int screenId, int buttonId) {
			try {
				if (connection is null)
					return null;

				string query = $"SELECT COUNT({ButtonsConstants.BUTTON_ID}) FROM {ButtonsConstants.TABLE_NAME} WHERE {ButtonsConstants.BANK_NAME} = @bankName AND {ButtonsConstants.SCREEN_ID} = @screenId AND {ButtonsConstants.BUTTON_ID} = @buttonId;";
				var command = new SqlCommand(query, connection);
				command.Parameters.AddWithValue("@bankName", bankName);
				command.Parameters.AddWithValue("@screenId", screenId);
				command.Parameters.AddWithValue("@buttonId", buttonId);

				connection.Open();

				return (int) command.ExecuteScalar() == 1;
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex, "Button ID");
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
