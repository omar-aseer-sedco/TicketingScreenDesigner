﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketingScreenDesigner {
	public class DBConfig {
		public string Server { get; set; }
		public string Database { get; set; }
		public string IntegratedSecurity { get; set; }

		public DBConfig() {
			Server = "(local)";
			Database = "TSD";
			IntegratedSecurity = "sspi";
		}

		public DBConfig(string server, string database, string integratedSecurity) {
			Server = server;
			Database = database;
			IntegratedSecurity = integratedSecurity;
		}
	}
}