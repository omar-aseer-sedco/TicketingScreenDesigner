using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Text.Json;
using LogUtils;
using ExceptionUtils;
using DataAccessLayer.Constants;

namespace DataAccessLayer.Utils {
	/// <summary>
	/// Contains methods for managing SQL connections and executing SQL commands.
	/// </summary>
	public static class DBUtils {
		private static DBConfig? config = null;

		public delegate void ReaderDelegate(SqlDataReader reader);

		/// <summary>
		/// Creates an <c>SqlConnection</c> object. The connection data is obtained from a config file.
		/// </summary>
		/// <returns>The SqlConnection.</returns>
		public static SqlConnection? CreateConnection(out InitializationStatus result) {
			try {
				if (config is null) {
					string configFilePath = Path.Join(Directory.GetCurrentDirectory(), "config", "DB_config.json");

					using (var reader = new StreamReader(configFilePath)) {
						var options = new JsonSerializerOptions() {
							AllowTrailingCommas = true,
						};

						string jsonString = reader.ReadToEnd();
						config = JsonSerializer.Deserialize<DBConfig>(jsonString, options);
					}

					if (config is null)
						throw new NullReferenceException();
				}

				config.TrimData();

				bool authenticated = false;
				string connectionString = $"server={config.Server}; database={config.Database};";
				if (config.IntegratedSecurity != string.Empty) {
					connectionString += $" integrated security={config.IntegratedSecurity};";
					authenticated = true;
				}
				else if (config.UserId != string.Empty && config.Password != string.Empty) {
					connectionString += $" User ID={config.UserId}; Password={config.Password};";
					authenticated = true;
				}

				if (config.Server == string.Empty || config.Database == string.Empty || !authenticated) {
					throw new Exception("Configuration information missing.");
				}

				var connection = new SqlConnection(connectionString);

				result = InitializationStatus.SUCCESS;
				return connection;
			}
			catch (Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException) {
				LogsHelper.Log(new LogEvent(ex.Message, DateTime.Now, EventSeverity.Error, ex.Source, ex.StackTrace));
				result = InitializationStatus.FILE_NOT_FOUND;
				return default;
			}
			catch (Exception ex) when (ex is SerializationException || ex is NullReferenceException || ex is JsonException || ex is ArgumentException) {
				LogsHelper.Log(new LogEvent(ex.Message, DateTime.Now, EventSeverity.Error, ex.Source, ex.StackTrace));
				result = InitializationStatus.FILE_CORRUPTED;
				return default;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				result = InitializationStatus.FAILED_TO_CONNECT;
				return default;
			}
		}

		/// <summary>
		/// Creates a connection and verifies that no errors occur opening it.
		/// </summary>
		/// <returns>A value from the <InitializationStatus cref="InitializationStatus"/> enum.</returns>
		public static InitializationStatus VerifyConnection() {
			try {
				using var connection = CreateConnection(out InitializationStatus status);
				connection?.Open();

				return status;
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
		/// Calls <c>ExecuteNonQuery</c> using the given command.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <returns>The number of rows affected by the operation.</returns>
		public static int ExecuteNonQuery(SqlCommand command) {
			try {
				using var connection = CreateConnection(out InitializationStatus status);

				if (status != InitializationStatus.SUCCESS || connection is null) {
					return -1;
				}

				command.Connection = connection;
				connection.Open();

				return command.ExecuteNonQuery();
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}

			return -1;
		}

		/// <summary>
		/// Calls <c>ExecuteReader</c> using the given command. The delegate is then executed on the reader.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <param name="readerDelegate">A <c>ReaderDelegate</c> that gets executed on the reader.</param>
		public static bool ExecuteReader(SqlCommand command, ReaderDelegate readerDelegate) {
			try {
				using var connection = CreateConnection(out InitializationStatus status);

				if (status != InitializationStatus.SUCCESS || connection is null) {
					return false;
				}

				command.Connection = connection;
				connection.Open();

				using var reader = command.ExecuteReader();
				readerDelegate(reader);

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

		/// <summary>
		/// Calls <c>ExecuteReaderAsync</c> using the given command. The delegate is then executed on the reader.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <param name="readerDelegate">A <c>ReaderDelegate</c> that gets executed on the reader.</param>
		/// <returns>A task representing the asynchronous operation.</returns>
		public static async Task<bool> ExecuteReaderAsync(SqlCommand command, ReaderDelegate readerDelegate) {
			try {
				using var connection = CreateConnection(out InitializationStatus status);

				if (status != InitializationStatus.SUCCESS || connection is null) {
					return false;
				}

				command.Connection = connection;
				await connection.OpenAsync();

				using var reader = await command.ExecuteReaderAsync();
				readerDelegate(reader);

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

		/// <summary>
		/// Calls <c>ExecuteScalar</c> using the given command.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <returns>The result of <c>command.ExecuteScalar</c> call.</returns>
		public static object ExecuteScalar(SqlCommand command) {
			try {
				using var connection = CreateConnection(out InitializationStatus status);

				if (status != InitializationStatus.SUCCESS || connection is null) {
					return false;
				}

				command.Connection = connection;
				connection.Open();

				return command.ExecuteScalar();
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
