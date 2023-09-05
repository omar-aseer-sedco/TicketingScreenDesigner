using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Text.Json;

namespace TicketingScreenDesigner {
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
			catch (FileNotFoundException ex) {
				ExceptionHelper.ShowErrorMessageBox("The configuration file was not found. Was it accidentally moved, renamed, or deleted?");
				LogsHelper.Log(new LogEvent(ex.Message, DateTime.Now, EventSeverity.Error, ex.Source, ex.StackTrace));
				throw;
			}
			catch (ArgumentException ex) {
				ExceptionHelper.ShowErrorMessageBox("Failed to connect to database. Please try again.");
				LogsHelper.Log(new LogEvent(ex.Message, DateTime.Now, EventSeverity.Error, ex.Source, ex.StackTrace));
				throw;
			}
			catch (Exception ex) {
				if (ex is SerializationException || ex is NullReferenceException) {
					ExceptionHelper.HandleConfigReadError(ex);
					throw;
				}

				ExceptionHelper.HandleGeneralException(ex);
				throw;
			}
		}
	}
}
