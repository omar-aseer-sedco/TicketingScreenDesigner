namespace DataAccessLayer.DataClasses {
	public class Bank {
		public string BankName { get; set; }

		public Bank() {
			BankName = string.Empty;
		}

		public Bank(string bankName) {
			BankName = bankName;
		}
	}
}
