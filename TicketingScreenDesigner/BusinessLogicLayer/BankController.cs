using DataAccessLayer;
using DataAccessLayer.DataClasses;

namespace BusinessLogicLayer {
	public static class BankController {
		public static List<TicketingScreen> GetOrCreateBank(Bank bank) {
			if (!BankOperations.CheckIfBankExists(bank.BankName)) {
				BankOperations.AddBank(bank);
			}
			
			return BankOperations.GetScreens(bank.BankName);
		}

		public static bool CheckIfBankExists(string bankName) {
			return BankOperations.CheckIfBankExists(bankName);
		}

		public static void AddBank(Bank bank) {
			BankOperations.AddBank(bank);
		}

		public static List<TicketingScreen> GetScreens(string bankName) {
			return BankOperations.GetScreens(bankName);
		}

		public static bool VerifyPassword(string bankName, string password) {
			string actualPassword = BankOperations.GetPassword(bankName);

			return actualPassword == password && actualPassword != string.Empty;
		}
	}
}
