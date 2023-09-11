using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Text.Json;
using LogUtils;
using ExceptionUtils;

namespace DataAccessLayer {
	public static class DBUtils {
		public static SqlConnection CreateConnection() {
			try {
				string configFilePath = Path.Join(Directory.GetCurrentDirectory(), "config", "DB_config.txt");

				DBConfig? config;

				using (StreamReader reader = new StreamReader(configFilePath)) {
					string jsonString = reader.ReadToEnd();
					config = JsonSerializer.Deserialize<DBConfig>(jsonString);
				}

				if (config is null)
					throw new NullReferenceException();

				string connectionString = $"server={config.Server};database={config.Database};integrated security={config.IntegratedSecurity}";
				return new SqlConnection(connectionString);
			}
			catch (ArgumentException ex) {
				LogsHelper.Log(new LogEvent(ex.Message, DateTime.Now, EventSeverity.Error, ex.Source, ex.StackTrace));
				throw;
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex);
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
