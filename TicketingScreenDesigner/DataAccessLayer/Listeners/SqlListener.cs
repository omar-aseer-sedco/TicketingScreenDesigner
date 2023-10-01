#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using DataAccessLayer.Constants;
using DataAccessLayer.Utils;
using ExceptionUtils;
using LogUtils;
using System.Data.SqlClient;

namespace DataAccessLayer.Listeners {
	/// <summary>
	/// This class is used to listen for database changes that match a certain entity type, bank, and (optionally) screen.
	/// </summary>
	public abstract class SqlListener {
		private SqlDependency dependency;
		protected readonly SqlConnection? connection;

		/// <summary>
		/// The delegate that gets called when the dependency triggers a change.
		/// </summary>
		public DatabaseNotificationDelegate ClientDelegate { get; set; }
		/// <summary>
		/// A SQL query representing the command that is used to subscribe to the dependency.
		/// </summary>
		public string CommandText { get; protected set; }
		/// <summary>
		/// returns <c>true</c> if <c>SqlDependency</c> has been successfully started, and <c>false</c> otherwise.
		/// </summary>
		public bool Started { get; private set; }

		/// <summary>
		/// Creates an instance of <c>SqlListener</c>.
		/// </summary>
		public SqlListener() {
			try {
				connection = DBUtils.CreateConnection(out InitializationStatus status);

				if (status != InitializationStatus.SUCCESS || connection is null) {
					throw new Exception("Failed to create connection. Error: " + Enum.GetName(status));
				}
			}
			catch (Exception ex) {
				LogsHelper.Log("SqlListener - Failed to create connection.", DateTime.Now, EventSeverity.Error);
				ExceptionHelper.HandleGeneralException(ex);
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

		/// <summary>
		/// Creates the command that is used to subscribe to the dependency.
		/// </summary>
		/// <returns>An instance of <c>SqlCommand</c>.</returns>
		protected abstract SqlCommand CreateCommand();

		private void OnDependencyChange(object? sender, SqlNotificationEventArgs e) {
			try {
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
				ExceptionHelper.HandleSqlException(ex);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private void SubscribeToDependency() {
			try {
				SqlCommand command = CreateCommand();
				dependency = new SqlDependency(command);
				dependency.OnChange += new OnChangeEventHandler(OnDependencyChange);
				command.ExecuteNonQuery();
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}
	}
}
