using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketingScreenDesigner
{
	public static class dbUtils
	{
		public static SqlConnection CreateConnection()
		{
			string connectionString = "server=(local);database=TSD;integrated security=sspi";
			return new SqlConnection(connectionString);
		}
	}
}
