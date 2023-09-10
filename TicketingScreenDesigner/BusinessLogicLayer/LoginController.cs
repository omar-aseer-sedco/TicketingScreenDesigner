using DataAccessLayer;
using DataAccessLayer.DataClasses;

namespace BusinessLogicLayer {
	/// <summary>
	/// Contains methods for managing login and registration.
	/// </summary>
	public static class LoginController {
		/// <summary>
		/// Gets the screens for the specified bank if it exists. If it does not exist, creates the bank then returns the screens.
		/// </summary>
		/// <param name="bank">The bank to create/get the screens for.</param>
		/// <returns>A list of <c>TicketingScreen</c> objects representing the screens owned by the bank.</returns>
		public static List<TicketingScreen> GetOrCreateBank(Bank bank) {
			if (!BankOperations.CheckIfBankExists(bank.BankName)) {
				BankOperations.AddBank(bank);
			}
			
			return BankOperations.GetScreens(bank.BankName);
		}

		/// <summary>
		/// Checks if a bank with the given name exists.
		/// </summary>
		/// <param name="bankName">The name of the bank.</param>
		/// <returns><c>true</c> if a matching bank exists, and <c>false</c> otherwise.</returns>
		public static bool CheckIfBankExists(string bankName) {
			return BankOperations.CheckIfBankExists(bankName);
		}

		/// <summary>
		/// Adds a bank to the database.
		/// </summary>
		/// <param name="bank">The bank to be added to the database.</param>
		public static void AddBank(Bank bank) {
			BankOperations.AddBank(bank);
		}

		/// <summary>
		/// Gets the screens of the specified bank.
		/// </summary>
		/// <param name="bankName">The name of the bank.</param>
		/// <returns>A list of <c>TicketingScreen</c> objects representing the screens owned by the bank. If the bank does not exist, an empty list is returned.</returns>
		public static List<TicketingScreen> GetScreens(string bankName) {
			return BankOperations.GetScreens(bankName);
		}

		/// <summary>
		/// Verifies the password for the specified bank.
		/// </summary>
		/// <param name="bankName">The name of the bank.</param>
		/// <param name="password">The plaintext password to be verified</param>
		/// <returns><c>true</c> if the bank exists and the password matches the one stored in the database, and <c>false</c> otherwise.</returns>
		public static bool VerifyPassword(string bankName, string password) {
			string actualPassword = BankOperations.GetPassword(bankName);

			return actualPassword != string.Empty && actualPassword == password;
		}
	}
}
