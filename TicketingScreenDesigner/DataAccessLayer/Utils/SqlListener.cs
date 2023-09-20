#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using System.Data;
using DataAccessLayer.Constants;
using ExceptionUtils;
using LogUtils;
using System.Data.SqlClient;

namespace DataAccessLayer.Utils {
	/// <summary>
	/// This class is used to listen for database changes that match a certain entity type, bank, and (optionally) screen.
	/// </summary>
	public class SqlListener {
		private readonly SqlConnection? connection;
		private SqlDependency dependency;
		private SqlCommand command;
		private readonly NotifiableEntityTypes entityType;
		private readonly string bankName;
		private readonly int screenId;

		/// <summary>
		/// The delegate that gets called when the dependency triggers a change.
		/// </summary>
		public DatabaseNotificationDelegate ClientDelegate { get; set; }

		public string CommandText { get; private set; }

		/// <summary>
		/// returns <c>true</c> if <c>SqlDependency</c> has been successfully started, and <c>false</c> otherwise.
		/// </summary>
		public bool Started { get; private set; }

		/// <summary>
		/// Creates an instance of <c>SqlListener</c>.
		/// </summary>
		/// <param name="entityType">The type of the entity to listen for (screens or buttons).</param>
		/// <param name="bankName">The name of the bank for which to listen.</param>
		/// <param name="screenId">The ID of the screen for which to listen. This is only used if <c>entityType</c> is set to Buttons.</param>
		public SqlListener(NotifiableEntityTypes entityType, string bankName, int screenId) {
			try {
				try {
					connection = DBUtils.CreateConnection();
				}
				catch (Exception ex) {
					ExceptionHelper.HandleGeneralException(ex);
					LogsHelper.Log("SqlListener - Failed to create connection.", DateTime.Now, EventSeverity.Error);
				}

				this.entityType = entityType;
				this.bankName = bankName;
				this.screenId = screenId;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private void CreateCommand() {
			if (entityType == NotifiableEntityTypes.Screens) {
				CommandText = $"SELECT {ScreensConstants.SCREEN_ID} FROM dbo.{ScreensConstants.TABLE_NAME} WHERE {ScreensConstants.BANK_NAME} = @bankName;";
			}
			else if (entityType == NotifiableEntityTypes.Buttons) {
				CommandText = $"SELECT {ButtonsConstants.BUTTON_ID} FROM dbo.{ButtonsConstants.TABLE_NAME} WHERE {ButtonsConstants.BANK_NAME} = @bankName AND {ButtonsConstants.SCREEN_ID} = @screenId;";
			}

			command = new SqlCommand(CommandText, connection);
			command.Parameters.Add("@bankName", SqlDbType.VarChar, BanksConstants.BANK_NAME_SIZE).Value = bankName;
			if (entityType == NotifiableEntityTypes.Buttons) {
				command.Parameters.Add("@screenId", SqlDbType.Int).Value = screenId;
			}
		}

		/// <summary>
		/// Starts the SqlDependency on the appropriate connection.
		/// </summary>
		/// <returns><c>true</c> if the dependency was started successfully or was already started, and <c>false</c> otherwise.</returns>
		public bool Start() {
			try {
				if (connection is null) {
					Started = false;
					return false;
				}

				if (Started) {
					return true;
				}

				bool startResult = SqlDependency.Start(connection.ConnectionString);
				Started = startResult;

				connection.Open();
				SubscribeToDependency();

				return startResult;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return false;
			}
		}

		/// <summary>
		/// Stops the SqlDependency on the appropriate connection.
		/// </summary>
		/// /// <returns><c>true</c> if the dependency was stopped successfully or was already stopped, and <c>false</c> otherwise.</returns>
		public bool Stop() {
			try {
				if (connection is null) {
					return false;
				}

				if (!Started) {
					return true;
				}

				connection.Close();
				bool stopResult = SqlDependency.Stop(connection.ConnectionString);
				Started = stopResult ? false : Started;
				return stopResult;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return false;
			}
		}

		private void OnDependencyChange(object? sender, SqlNotificationEventArgs e) {
			try {
				LogsHelper.Log("Event args info: " + Enum.GetName(e.Info), DateTime.Now, EventSeverity.Info);
				if (ClientDelegate is not null) {
					ClientDelegate(e.Info);
				}
				else {
					LogsHelper.Log("Client delegate is null", DateTime.Now, EventSeverity.Warning);
				}

				if (e.Info != SqlNotificationInfo.Invalid && e.Type != SqlNotificationType.Subscribe)
					SubscribeToDependency();
				else
					LogsHelper.Log("Unable to resubscribe to SQL dependency.", DateTime.Now, EventSeverity.Error);
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex, "Screen ID");
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private void SubscribeToDependency() {
			try {
				CreateCommand();
				dependency = new SqlDependency(command);
				dependency.OnChange += new OnChangeEventHandler(OnDependencyChange);
				command.ExecuteNonQuery();
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex, "Screen ID");
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}
	}
}
