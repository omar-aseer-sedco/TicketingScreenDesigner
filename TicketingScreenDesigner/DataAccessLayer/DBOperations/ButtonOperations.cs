using DataAccessLayer.Constants;
using DataAccessLayer.DataClasses;
using DataAccessLayer.Utils;
using ExceptionUtils;
using LogUtils;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text;

namespace DataAccessLayer.DBOperations {
	/// <summary>
	/// Contains methods for retrieving and manipulating button information.
	/// </summary>
	public static class ButtonOperations {
		/// <summary>
		/// Verifies that the database connection is established correctly.
		/// </summary>
		/// <returns><c>true</c> if the connection has been established properly, and <c>false</c> otherwise.</returns>
		public static InitializationStatus VerifyConnection() {
			try {
				return DBUtils.VerifyConnection();
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex);
				return InitializationStatus.FAILED_TO_CONNECT;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return InitializationStatus.UNDEFINED_ERROR;
			}
		}

		/// <summary>
		/// Checks if the button with the given bank name, screen ID, and button ID exists in the database.
		/// </summary>
		/// <param name="bankName">The name of the bank that owns the button.</param>
		/// <param name="screenId">The ID of the screen that contains the button.</param>
		/// <param name="buttonId">The ID of the button.</param>
		/// <returns><c>true</c> if a matching button exists, and <c>false</c> if it does not. If the operation fails, <c>null</c> is returned.</returns>
		public static bool? CheckIfButtonExists(string bankName, int screenId, int buttonId) {
			try {
				string query = $"SELECT COUNT({ButtonsConstants.BUTTON_ID}) FROM {ButtonsConstants.TABLE_NAME} WHERE {ButtonsConstants.BANK_NAME} = @bankName AND {ButtonsConstants.SCREEN_ID} = @screenId AND {ButtonsConstants.BUTTON_ID} = @buttonId;";
				var command = new SqlCommand(query);
				command.Parameters.Add("@bankName", SqlDbType.VarChar, ButtonsConstants.BANK_NAME_SIZE).Value = bankName;
				command.Parameters.Add("@screenId", SqlDbType.Int).Value = screenId;
				command.Parameters.Add("@buttonId", SqlDbType.Int).Value = buttonId;

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
		/// Adds, deletes, and updates the provided buttons. If one of the operations fails, all of the other operations are rolled back.
		/// </summary>
		/// <param name="bankName">The name of the bank that owns the buttons.</param>
		/// <param name="screenId">The ID of the screen that contains the buttons.</param>
		/// <param name="buttonsToAdd">The list of buttons to be added.</param>
		/// <param name="buttonsToDelete">The list of button IDs to be deleted.</param>
		/// <param name="buttonsToUpdate">The dictionary of button IDs and the new button to which to update.</param>
		/// <returns>A dictionary where the keys are the IDs of the buttons, and the values are <c>true</c> if the operation succeeded, and 
		/// <c>false</c> if the operation failed (and caused the others to be rolled back). If the lists were empty, and empty dictionary is returned. If the entire operation fails, <c>null</c> is returned.</returns>
		public static Dictionary<int, bool>? AtomicCommit(string bankName, int screenId, List<TicketingButton> buttonsToAdd, Dictionary<int, TicketingButton> buttonsToUpdate, List<int> buttonsToDelete) {
			SqlTransaction? transaction = null;

			try {
				using var connection = DBUtils.CreateConnection(out InitializationStatus status);

				if (status != InitializationStatus.SUCCESS || connection is null) {
					return default;
				}

				var success = new Dictionary<int, bool>();
				if (buttonsToAdd.Count == 0 && buttonsToUpdate.Count == 0 && buttonsToDelete.Count == 0)
					return success;

				bool allSucceeded = true;

				var addButtonsCommand = CreateAddButtonsCommand(bankName, screenId, buttonsToAdd);
				var updateButtonsCommand = CreateUpdateButtonsCommand(bankName, screenId, buttonsToUpdate);
				var deleteButtonsCommands = CreateDeleteButtonsCommandList(bankName, screenId, buttonsToDelete);

				if (addButtonsCommand is null || updateButtonsCommand is null || deleteButtonsCommands is null) {
					LogsHelper.Log("Failed to create command(s).", DateTime.Now, EventSeverity.Error);
					return default;
				}

				addButtonsCommand.Connection = connection;
				updateButtonsCommand.Connection = connection;

				connection.Open();
				transaction = connection.BeginTransaction();

				addButtonsCommand.Transaction = transaction;
				updateButtonsCommand.Transaction = transaction;

				if (buttonsToAdd.Count > 0) {
					var addButtonsReader = addButtonsCommand.ExecuteReader();
					while (addButtonsReader.Read()) {
						int tmpId = addButtonsReader.GetInt32(ButtonsConstants.ADD_BUTTONS_TMP_ID_INDEX);
						bool buttonSuccess = addButtonsReader.GetBoolean(ButtonsConstants.ADD_BUTTONS_SUCCESS_INDEX);
						string errorMessage = string.Empty;

						try {
							errorMessage = addButtonsReader.GetString(ButtonsConstants.ADD_BUTTONS_ERROR_MESSAGE_INDEX);
						}
						catch (SqlNullValueException) {
							errorMessage = "No error message";
						}

						if (!buttonSuccess) {
							LogsHelper.Log($"Button insert error: {errorMessage}", DateTime.Now, EventSeverity.Error);
							allSucceeded = false;
						}

						success.Add(tmpId, buttonSuccess);
					}
					addButtonsReader.Close();
				}

				if (buttonsToUpdate.Count > 0) {
					var updateButtonsReader = updateButtonsCommand.ExecuteReader();
					while (updateButtonsReader.Read()) {
						int buttonId = updateButtonsReader.GetInt32(ButtonsConstants.UPDATE_BUTTONS_ID_INDEX);
						bool buttonSuccess = updateButtonsReader.GetBoolean(ButtonsConstants.UPDATE_BUTTONS_SUCCESS_INDEX);
						string errorMessage = string.Empty;

						try {
							errorMessage = updateButtonsReader.GetString(ButtonsConstants.UPDATE_BUTTONS_ERROR_MESSAGE_INDEX);
						}
						catch (SqlNullValueException) {
							errorMessage = "No error message";
						}

						if (!buttonSuccess) {
							LogsHelper.Log($"Button update error: {errorMessage}", DateTime.Now, EventSeverity.Error);
							allSucceeded = false;
						}

						success.Add(buttonId, buttonSuccess);
					}
					updateButtonsReader.Close();
				}

				if (buttonsToDelete.Count > 0) {
					foreach (var command in deleteButtonsCommands) {
						command.Connection = connection;
						command.Transaction = transaction;

						command.ExecuteNonQuery();
					}
				}

				if (allSucceeded) {
					transaction.Commit();
				}
				else {
					transaction.Rollback();
					LogsHelper.Log("Transaction rolled back.", DateTime.Now, EventSeverity.Info);
				}

				return success;
			}
			catch (SqlException ex) {
				transaction?.Rollback();
				LogsHelper.Log("Transaction rolled back.", DateTime.Now, EventSeverity.Info);
				ExceptionHelper.HandleSqlException(ex);
			}
			catch (Exception ex) {
				transaction?.Rollback();
				LogsHelper.Log("Transaction rolled back.", DateTime.Now, EventSeverity.Info);
				ExceptionHelper.HandleGeneralException(ex);
			}

			return default;
		}

		private static SqlCommand? CreateAddButtonsCommand(string bankName, int screenId, List<TicketingButton> buttons) {
			try {
				DataTable dataTable = new();
				dataTable.Columns.Add(ButtonsConstants.BANK_NAME, typeof(string));
				dataTable.Columns.Add(ButtonsConstants.SCREEN_ID, typeof(int));
				dataTable.Columns.Add(ButtonsConstants.TMP_ID, typeof(int));
				dataTable.Columns.Add(ButtonsConstants.TYPE, typeof(int));
				dataTable.Columns.Add(ButtonsConstants.NAME_EN, typeof(string));
				dataTable.Columns.Add(ButtonsConstants.NAME_AR, typeof(string));
				dataTable.Columns.Add(ButtonsConstants.SERVICE, typeof(string));
				dataTable.Columns.Add(ButtonsConstants.MESSAGE_EN, typeof(string));
				dataTable.Columns.Add(ButtonsConstants.MESSAGE_AR, typeof(string));

				foreach (var button in buttons) {
					DataRow row = dataTable.NewRow();

					row[ButtonsConstants.BANK_NAME] = bankName;
					row[ButtonsConstants.SCREEN_ID] = screenId;
					row[ButtonsConstants.TMP_ID] = button.ButtonId;
					row[ButtonsConstants.TYPE] = (int) button.Type;
					row[ButtonsConstants.NAME_EN] = button.NameEn;
					row[ButtonsConstants.NAME_AR] = button.NameAr;

					if (button is IssueTicketButton issueTicketButton) {
						row[ButtonsConstants.SERVICE] = issueTicketButton.Service;
						row[ButtonsConstants.MESSAGE_EN] = null;
						row[ButtonsConstants.MESSAGE_AR] = null;
					}
					else if (button is ShowMessageButton showMessageButton) {
						row[ButtonsConstants.SERVICE] = null;
						row[ButtonsConstants.MESSAGE_EN] = showMessageButton.MessageEn;
						row[ButtonsConstants.MESSAGE_AR] = showMessageButton.MessageAr;
					}

					dataTable.Rows.Add(row);
				}

				var command = new SqlCommand(ButtonsConstants.ADD_BUTTONS_PROCEDURE);
				command.CommandType = CommandType.StoredProcedure;
				SqlParameter addButtonsParameter = command.Parameters.AddWithValue(ButtonsConstants.ADD_BUTTONS_PARAMETER_NAME, dataTable);
				addButtonsParameter.SqlDbType = SqlDbType.Structured;
				addButtonsParameter.TypeName = ButtonsConstants.ADD_BUTTONS_PARAMETER_TYPE;

				return command;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}

			return null;
		}

		private static SqlCommand? CreateUpdateButtonsCommand(string bankName, int screenId, Dictionary<int, TicketingButton> buttons) {
			try {
				DataTable dataTable = new();
				dataTable.Columns.Add(ButtonsConstants.BANK_NAME, typeof(string));
				dataTable.Columns.Add(ButtonsConstants.SCREEN_ID, typeof(int));
				dataTable.Columns.Add(ButtonsConstants.BUTTON_ID, typeof(int));
				dataTable.Columns.Add(ButtonsConstants.TYPE, typeof(int));
				dataTable.Columns.Add(ButtonsConstants.NAME_EN, typeof(string));
				dataTable.Columns.Add(ButtonsConstants.NAME_AR, typeof(string));
				dataTable.Columns.Add(ButtonsConstants.SERVICE, typeof(string));
				dataTable.Columns.Add(ButtonsConstants.MESSAGE_EN, typeof(string));
				dataTable.Columns.Add(ButtonsConstants.MESSAGE_AR, typeof(string));

				foreach (var button in buttons) {
					DataRow row = dataTable.NewRow();

					row[ButtonsConstants.BANK_NAME] = bankName;
					row[ButtonsConstants.SCREEN_ID] = screenId;
					row[ButtonsConstants.BUTTON_ID] = button.Key;
					row[ButtonsConstants.TYPE] = (int) button.Value.Type;
					row[ButtonsConstants.NAME_EN] = button.Value.NameEn;
					row[ButtonsConstants.NAME_AR] = button.Value.NameAr;

					if (button.Value is IssueTicketButton issueTicketButton) {
						row[ButtonsConstants.SERVICE] = issueTicketButton.Service;
						row[ButtonsConstants.MESSAGE_EN] = null;
						row[ButtonsConstants.MESSAGE_AR] = null;
					}
					else if (button.Value is ShowMessageButton showMessageButton) {
						row[ButtonsConstants.SERVICE] = null;
						row[ButtonsConstants.MESSAGE_EN] = showMessageButton.MessageEn;
						row[ButtonsConstants.MESSAGE_AR] = showMessageButton.MessageAr;
					}

					dataTable.Rows.Add(row);
				}

				var command = new SqlCommand(ButtonsConstants.UPDATE_BUTTONS_PROCEDURE);
				command.CommandType = CommandType.StoredProcedure;
				SqlParameter updateButtonsParameter = command.Parameters.AddWithValue(ButtonsConstants.UPDATE_BUTTONS_PARAMETER_NAME, dataTable);
				updateButtonsParameter.SqlDbType = SqlDbType.Structured;
				updateButtonsParameter.TypeName = ButtonsConstants.UPDATE_BUTTONS_PARAMETER_TYPE;

				return command;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}

			return null;
		}

		private static List<SqlCommand>? CreateDeleteButtonsCommandList(string bankName, int screenId, List<int> buttonIds) {
			try {
				const int MAX_PARAMETERS = 1000;

				List<SqlCommand> commandList = new();

				while (buttonIds.Count > 0) {
					if (buttonIds.Count > MAX_PARAMETERS) {
						var nextCommand = CreateDeleteButtonsCommand(bankName, screenId, buttonIds.GetRange(0, MAX_PARAMETERS));
						buttonIds = buttonIds.GetRange(MAX_PARAMETERS, buttonIds.Count - MAX_PARAMETERS);
						if (nextCommand is null)
							return null;

						commandList.Add(nextCommand);
					}
					else {
						var nextCommand = CreateDeleteButtonsCommand(bankName, screenId, buttonIds);
						if (nextCommand is null)
							return null;

						commandList.Add(nextCommand);
						break;
					}
				}

				return commandList;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}

			return null;
		}

		private static SqlCommand? CreateDeleteButtonsCommand(string bankName, int screenId, List<int> buttonIds) {
			try {
				var query = new StringBuilder($"DELETE FROM {ButtonsConstants.TABLE_NAME} WHERE {ButtonsConstants.BANK_NAME} = @bankName AND {ButtonsConstants.SCREEN_ID} = @screenId AND {ButtonsConstants.BUTTON_ID} IN (");
				var command = new SqlCommand();
				command.Parameters.Add("@bankName", SqlDbType.VarChar, ButtonsConstants.BANK_NAME_SIZE).Value = bankName;
				command.Parameters.Add("@screenId", SqlDbType.Int).Value = screenId;

				int i = 0;
				foreach (var buttonId in buttonIds) {
					query.Append("@buttonId").Append(i).Append(',');
					command.Parameters.Add("@buttonId" + i, SqlDbType.Int).Value = buttonId;
					++i;
				}

				query.Length--;
				query.Append(");");
				command.CommandText = query.ToString();

				return command;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}

			return null;
		}
	}
}
