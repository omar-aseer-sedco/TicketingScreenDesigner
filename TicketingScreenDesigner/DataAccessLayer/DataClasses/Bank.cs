namespace DataAccessLayer.DataClasses {
	public class Bank {
		public string BankName { get; set; }
		public string Password { get; set; }

		public Bank() {
			BankName = string.Empty;
			Password = string.Empty;
		}

		public Bank(string bankName, string password) {
			BankName = bankName;
			Password = password;
		}
	}
}
