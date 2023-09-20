using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Text.Json;
using LogUtils;
using ExceptionUtils;

namespace DataAccessLayer.Utils {
	public static class DBUtils {
		private static DBConfig? config = null;

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

				string connectionString = $"server={config.Server}; database={config.Database};";
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
	}
}
