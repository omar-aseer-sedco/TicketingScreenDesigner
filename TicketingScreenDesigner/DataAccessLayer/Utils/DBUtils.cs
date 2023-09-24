using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Text.Json;
using LogUtils;
using ExceptionUtils;

namespace DataAccessLayer.Utils {
	public static class DBUtils {
		private static DBConfig? config = null;

		public delegate void ReaderDelegate(SqlDataReader reader);

		/// <summary>
		/// Creates an <c>SqlConnection</c> object. The connection string is obtained from a config file.
		/// </summary>
		/// <returns>The SqlConnection.</returns>
		public static SqlConnection CreateConnection() {
			try {
				if (config is null) {
					string configFilePath = Path.Join(Directory.GetCurrentDirectory(), "config", "DB_config.txt");

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

				string connectionString = $"server={config.Server}; database={config.Database}; MultipleActiveResultSets=True;";
				if (config.IntegratedSecurity is not null && config.IntegratedSecurity != string.Empty) {
					connectionString += $" integrated security={config.IntegratedSecurity};";
				}
				if (config.UserId is not null && config.UserId != string.Empty && config.Password is not null) {
					connectionString += $" User ID={config.UserId}; Password={config.Password};";
				}

				return new SqlConnection(connectionString);
			}
			catch (ArgumentException ex) {
				LogsHelper.Log(new LogEvent(ex.Message, DateTime.Now, EventSeverity.Error, ex.Source, ex.StackTrace));
				throw;
			}
			catch (Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException) {
				LogsHelper.Log(new LogEvent(ex.Message, DateTime.Now, EventSeverity.Error, ex.Source, ex.StackTrace));
				throw;
			}
			catch (Exception ex) when (ex is SerializationException || ex is NullReferenceException || ex is JsonException) {
				LogsHelper.Log(new LogEvent(ex.Message, DateTime.Now, EventSeverity.Error, ex.Source, ex.StackTrace));
				throw;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				throw;
			}
		}

		/// <summary>
		/// Creates a connection and verifies that no errors occur opening it.
		/// </summary>
		/// <returns><c>true</c> if the connection can be established correctly, and <c>false</c> otherwise.</returns>
		public static bool VerifyConnection() {
			try {
				using var connection = CreateConnection();
				connection.Open();

				return true;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return false;
			}
		}

		/// <summary>
		/// Calls <c>ExecuteNonQuery</c> using the given command.
		/// </summary>
		/// <param name="connection">The connection</param>
		/// <param name="command">The command to execute.</param>
		/// <returns>The number of rows affected by the operation.</returns>
		public static int ExecuteNonQuery(SqlCommand command) {
			try {
				using var connection = CreateConnection();
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
		public static void ExecuteReader(SqlCommand command, ReaderDelegate readerDelegate) {
			try {
				using var connection = CreateConnection();
				command.Connection = connection;
				connection.Open();

				using (var reader = command.ExecuteReader()) {
					readerDelegate(reader);
				}
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		/// <summary>
		/// Calls <c>ExecuteReaderAsync</c> using the given command. The delegate is then executed on the reader.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <param name="readerDelegate">A <c>ReaderDelegate</c> that gets executed on the reader.</param>
		/// <returns>A task representing the asynchronous operation.</returns>
		public static async Task ExecuteReaderAsync(SqlCommand command, ReaderDelegate readerDelegate) {
			try {
				using var connection = CreateConnection();
				command.Connection = connection;
				await connection.OpenAsync();

				using var reader = await command.ExecuteReaderAsync();
				readerDelegate(reader);
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		/// <summary>
		/// Calls <c>ExecuteScalar</c> using the given command.
		/// </summary>
		/// <param name="command">The command to execute.</param>
		/// <returns>The result of <c>command.ExecuteScalar</c> call.</returns>
		public static object ExecuteScalar(SqlCommand command) {
			try {
				using var connection = CreateConnection();
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

			return -1;
		}
	}
}
