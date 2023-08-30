using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;

namespace TicketingScreenDesigner {
	public static class DBUtils {
		public static SqlConnection CreateConnection() {
			string connectionString = "server=OMAR-ASEER;database=TSD;integrated security=sspi";
			return new SqlConnection(connectionString);
		}
	}
}
