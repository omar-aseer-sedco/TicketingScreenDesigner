namespace DataAccessLayer {
	public class DBConfig {
		public string Server { get; set; }
		public string Database { get; set; }
		public string IntegratedSecurity { get; set; }
		public string UserId { get; set; }
		public string Password { get; set; }

		public DBConfig() {
			Server = string.Empty;
			Database = string.Empty;
			IntegratedSecurity = string.Empty;
			UserId = string.Empty;
			Password = string.Empty;
		}

		public DBConfig(string server, string database, string integratedSecurity, string userId, string password) {
			Server = server;
			Database = database;
			IntegratedSecurity = integratedSecurity;
			UserId = userId;
			Password = password;
		}

		public void TrimData() {
			Server = Server.Trim();
			Database = Database.Trim();
			IntegratedSecurity = IntegratedSecurity.Trim();
			UserId = UserId.Trim();
			Password = Password.Trim();
		}
	}
}
