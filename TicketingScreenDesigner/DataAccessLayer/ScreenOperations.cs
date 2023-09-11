using DataAccessLayer.Constants;
using DataAccessLayer.DataClasses;
using ExceptionUtils;
using System.Data.SqlClient;
using System.Text;

namespace DataAccessLayer {
	/// <summary>
	/// Contains methods for retrieving and manipulating screen information.
	/// </summary>
	public class ScreenOperations {
		public static readonly ScreenOperations Instance = new();
		private SqlConnection? connection;

		private ScreenOperations() {
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
			if (connection is null)
				return false;

			try {
				connection.Open();
				var command = new SqlCommand($"SELECT 1 FROM {ScreensConstants.TABLE_NAME};", connection);

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

		private class ActiveScreenController {
			public static readonly ActiveScreenController Instance = new();

			public int? GetActiveScreenId(string bankName) {
				try {
					if (ScreenOperations.Instance.connection is null)
						return null;

					int? ret = -1;

					string query = $"SELECT {ScreensConstants.SCREEN_ID} FROM {ScreensConstants.TABLE_NAME} WHERE {ScreensConstants.BANK_NAME} = @bankName AND {ScreensConstants.IS_ACTIVE} = 1;";
					var command = new SqlCommand(query, ScreenOperations.Instance.connection);
					command.Parameters.AddWithValue("@bankName", bankName);

					ScreenOperations.Instance.connection.Open();

					var reader = command.ExecuteReader();

					if (reader.Read())
						ret = reader.GetInt32(0);

					return ret;
				}
				catch (SqlException ex) {
					ExceptionHelper.HandleSqlException(ex, "Screen ID");
				}
				catch (Exception ex) {
					ExceptionHelper.HandleGeneralException(ex);
				}
				finally {
					ScreenOperations.Instance.connection?.Close();
				}

				return null;
			}

			public bool DeactivateScreen(string bankName, int screenId) {
				return SetIsActive(bankName, screenId, false);
			}

			public bool ActivateScreen(string bankName, int screenId) {
				int? currentlyActiveScreenId = GetActiveScreenId(bankName);

				if (currentlyActiveScreenId is null)
					return false;

				if (currentlyActiveScreenId != -1 && currentlyActiveScreenId != screenId) {
					DeactivateScreen(bankName, (int) currentlyActiveScreenId);
				}

				return SetIsActive(bankName, screenId, true);
			}

			private bool SetIsActive(string bankName, int screenId, bool active) {
				try {
					if (ScreenOperations.Instance.connection is null)
						return false;

					string query = $"UPDATE {ScreensConstants.TABLE_NAME} SET {ScreensConstants.IS_ACTIVE} = @isActive WHERE {ScreensConstants.BANK_NAME} = @bankName AND {ScreensConstants.SCREEN_ID} = @screenId";
					var command = new SqlCommand(query, ScreenOperations.Instance.connection);
					command.Parameters.AddWithValue("@isActive", active ? 1 : 0);
					command.Parameters.AddWithValue("@bankName", bankName);
					command.Parameters.AddWithValue("@screenId", screenId);

					ScreenOperations.Instance.connection.Open();

					return command.ExecuteNonQuery() == 1;
				}
				catch (SqlException ex) {
					ExceptionHelper.HandleSqlException(ex, "Screen ID");
				}
				catch (Exception ex) {
					ExceptionHelper.HandleGeneralException(ex);
				}
				finally {
					ScreenOperations.Instance.connection?.Close();
				}

				return false;
			}
		}

		/// <summary>
		/// Adds a screen to the database.
		/// </summary>
		/// <param name="screen">The screen to be added to the database</param>
		/// <returns>The ID of the screen that was added. If the operation fails, <c>null</c> is returned.</returns>
		public int? AddScreen(TicketingScreen screen) {
			try {
				if (connection is null)
					return null;

				if (screen.IsActive) {
					int? activeScreenId = GetActiveScreenId(screen.BankName);
					if (activeScreenId is null)
						return null;
					if (activeScreenId != -1)
						DeactivateScreen(screen.BankName, (int) activeScreenId);
				}

				string query = $"INSERT INTO {ScreensConstants.TABLE_NAME} ({ScreensConstants.BANK_NAME}, {ScreensConstants.IS_ACTIVE}, {ScreensConstants.SCREEN_TITLE}) VALUES (@bankName, @isActive, @screenTitle); SELECT CAST(IDENT_CURRENT('{ScreensConstants.TABLE_NAME}') AS int);";
				var command = new SqlCommand(query, connection);
				command.Parameters.AddWithValue("@bankName", screen.BankName);
				command.Parameters.AddWithValue("@isActive", screen.IsActive);
				command.Parameters.AddWithValue("@screenTitle", screen.ScreenTitle);

				connection.Open();

				return (int) command.ExecuteScalar();
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
		/// Gets the ID of the active screen.
		/// </summary>
		/// <param name="bankName">The name of the bank.</param>
		/// <returns>The ID of the active screen. If there is no active screen, <c>-1</c> is returned. If the operation fails, <c>null</c> is returned.</returns>
		public int? GetActiveScreenId(string bankName) {
			if (connection is null)
				return null;

			return ActiveScreenController.Instance.GetActiveScreenId(bankName);
		}

		/// <summary>
		/// Deactivates the specifed screen.
		/// </summary>
		/// <param name="bankName">The name of the bank which owns the screen.</param>
		/// <param name="screenId">The ID of the screen.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public bool DeactivateScreen(string bankName, int screenId) {
			if (connection is null)
				return false;

			return ActiveScreenController.Instance.DeactivateScreen(bankName, screenId);
		}

		/// <summary>
		/// Activates the specified screen.
		/// </summary>
		/// <param name="bankName">The name of the bank which owns the screen.</param>
		/// <param name="screenId">The ID of the screen.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public bool ActivateScreen(string bankName, int screenId) {
			if (connection is null)
				return false;

			return ActiveScreenController.Instance.ActivateScreen(bankName, screenId);
		}

		/// <summary>
		/// Deletes the screen with the given ID from the database.
		/// </summary>
		/// <param name="bankName">The name of the bank that owns the screen.</param>
		/// <param name="screenId">The ID of the screen to be deleted</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public bool DeleteScreen(string bankName, int screenId) {
			try {
				if (connection is null)
					return false;

				string query = $"DELETE FROM {ScreensConstants.TABLE_NAME} WHERE {ScreensConstants.BANK_NAME} = @bankName AND {ScreensConstants.SCREEN_ID} = @screenId;";
				var command = new SqlCommand(query, connection);
				command.Parameters.AddWithValue("@bankName", bankName);
				command.Parameters.AddWithValue("@screenId", screenId);

				connection.Open();

				return command.ExecuteNonQuery() == 1;
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex, "screen ID");
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
		/// <param name="bankName">The name of the bank that owns the screens.</param>
		/// <param name="screenIds">The IDs of the screen to be deleted</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public bool DeleteScreens(string bankName, List<int> screenIds) {
			try {
				if (connection is null)
					return false;

				var query = new StringBuilder($"DELETE FROM {ScreensConstants.TABLE_NAME} WHERE {ScreensConstants.BANK_NAME} = @bankName AND {ScreensConstants.SCREEN_ID} IN (");
				var command = new SqlCommand() { Connection = connection };
				command.Parameters.AddWithValue("@bankName", bankName);

				int i = 0;
				foreach (var screenId in screenIds) {
					query.Append("@screenId").Append(i).Append(',');
					command.Parameters.AddWithValue("@screenId" + i, screenId);

					++i;
				}

				query.Length--;
				query.Append(");");
				command.CommandText = query.ToString();

				connection.Open();
				return command.ExecuteNonQuery() == screenIds.Count;
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex, "screen ID");
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
		/// Updates the screen with the given ID to <c>newScreen</c>.
		/// </summary>
		/// <param name="bankName">The name of the bank that owns the screen.</param>
		/// <param name="screenId">The ID of the screen.</param>
		/// <param name="newScreen">A <c>TicketingScreen</c> object containing the updated screen information.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public bool UpdateScreen(string bankName, int screenId, TicketingScreen newScreen) {
			try {
				if (connection is null)
					return false;

				int? activeScreenId = GetActiveScreenId(bankName);
				if (activeScreenId is not null && activeScreenId != screenId) {
					DeactivateScreen(bankName, (int) activeScreenId);
				}

				string query = $"UPDATE {ScreensConstants.TABLE_NAME} SET {ScreensConstants.SCREEN_TITLE} = @screenTitle, {ScreensConstants.IS_ACTIVE} = @isActive WHERE {ScreensConstants.SCREEN_ID} = @screenId AND {ScreensConstants.BANK_NAME} = @bankName";
				var command = new SqlCommand(query, connection);
				command.Parameters.AddWithValue("@screenTitle", newScreen.ScreenTitle);
				command.Parameters.AddWithValue("@isActive", newScreen.IsActive);
				command.Parameters.AddWithValue("@screenId", screenId);
				command.Parameters.AddWithValue("@bankName", bankName);

				connection.Open();

				return command.ExecuteNonQuery() == 1;
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex, "screen ID");
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
		/// Checks if the screen with the given bank name and ID exists.
		/// </summary>
		/// <param name="bankName">The name of the bank that owns the screen.</param>
		/// <param name="screenId">The ID of the screen.</param>
		/// <returns><c>true</c> if a matching screen exists, and <c>false</c> if it does not. If the operation fails, <c>null</c> is returned.</returns>
		public bool? CheckIfScreenExists(string bankName, int screenId) {
			try {
				if (connection is null)
					return null;

				string query = $"SELECT COUNT({ScreensConstants.SCREEN_ID}) FROM {ScreensConstants.TABLE_NAME} WHERE {ScreensConstants.BANK_NAME} = @bankName AND {ScreensConstants.SCREEN_ID} = @screenId;";
				var command = new SqlCommand(query, connection);
				command.Parameters.AddWithValue("@bankName", bankName);
				command.Parameters.AddWithValue("@screenId", screenId);

				connection.Open();

				return (int) command.ExecuteScalar() == 1;
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
		/// Gets the screen with the given bank name and ID.
		/// </summary>
		/// <param name="bankName">The name of the bank that owns the screen.</param>
		/// <param name="screenId">The ID of the screen.</param>
		/// <returns>A <c>TicketingScreen</c> object representing the screen. If the screen does not exist, <EmptyScreen.Value cref="EmptyScreen.Value"/> is returned. If the operation fails, <c>null</c> is returned.</returns>
		public TicketingScreen? GetScreenById(string bankName, int screenId) {
			try {
				if (connection is null)
					return null;

				TicketingScreen? screen = EmptyScreen.Value;

				string query = $"SELECT {ScreensConstants.SCREEN_TITLE}, {ScreensConstants.IS_ACTIVE} FROM {ScreensConstants.TABLE_NAME} WHERE {ScreensConstants.BANK_NAME} = @bankName AND {ScreensConstants.SCREEN_ID} = @screenId;";
				var command = new SqlCommand(query, connection);
				command.Parameters.AddWithValue("@bankName", bankName);
				command.Parameters.AddWithValue("@screenId", screenId);

				connection.Open();

				var reader = command.ExecuteReader();

				if (reader.Read()) {
					string screenTitle = (string) reader[ScreensConstants.SCREEN_TITLE];
					bool isActive = (bool) reader[ScreensConstants.IS_ACTIVE];
					screen = new TicketingScreen(bankName, screenId, screenTitle, isActive);
				}

				return screen;
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

			return null; ;
		}

		/// <summary>
		/// Gets all the buttons associated with the specified screen.
		/// </summary>
		/// <param name="bankName">The name of the bank that owns the screen.</param>
		/// <param name="screenId">The ID of the screen.</param>
		/// <returns>A list of <c>TicketingButton</c> objects representing the buttons on the screen. If there are no buttons, an empty list is returned. If the operation fails, <c>null</c> is returned.</returns>
		public List<TicketingButton>? GetButtons(string bankName, int screenId) {
			try {
				if (connection is null)
					return null;

				var buttons = new List<TicketingButton>();

				string query = $"SELECT {ButtonsConstants.BUTTON_ID}, {ButtonsConstants.NAME_EN}, {ButtonsConstants.NAME_AR}, {ButtonsConstants.TYPE}, {ButtonsConstants.SERVICE}, {ButtonsConstants.MESSAGE_EN}, " +
					$"{ButtonsConstants.MESSAGE_AR} FROM {ButtonsConstants.TABLE_NAME} WHERE {ButtonsConstants.BANK_NAME} = @bankName AND {ButtonsConstants.SCREEN_ID} = @screenId";
				var command = new SqlCommand(query, connection);
				command.Parameters.AddWithValue("@bankName", bankName);
				command.Parameters.AddWithValue("@screenId", screenId);

				connection.Open();

				var reader = command.ExecuteReader();

				while (reader.Read()) {
					int buttonId = (int) reader[ButtonsConstants.BUTTON_ID];
					string nameEn = (string) reader[ButtonsConstants.NAME_EN];
					string nameAr = (string) reader[ButtonsConstants.NAME_AR];
					string type = (string) reader[ButtonsConstants.TYPE];

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

				return buttons;
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
