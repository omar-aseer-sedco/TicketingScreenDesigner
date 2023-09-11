using DataAccessLayer;
using DataAccessLayer.DataClasses;

namespace BusinessLogicLayer {
	/// <summary>
	/// Contains methods for managing login and registration.
	/// </summary>
	public class LoginController {
		/// <summary>
		/// Creates a <c>LoginController</c> object.
		/// </summary>
		/// <param name="success"><c>true</c> if the connection with the database was established successfully, and <c>false</c> otherwise.</param>
		public LoginController(out bool success) {
			if (BankOperations.Instance.VerifyConnection()) {
				success = true;
			}
			else {
				success = false;
			}
		}

		/// <summary>
		/// Gets the screens for the specified bank if it exists. If it does not exist, creates the bank then returns the screens.
		/// </summary>
		/// <param name="bank">The bank to create/get the screens for.</param>
		/// <returns>A list of <c>TicketingScreen</c> objects representing the screens owned by the bank. If the operation fails, <c>null</c> is returned.</returns>
		public List<TicketingScreen>? GetOrCreateBank(Bank bank) {
			bool? bankExists = BankOperations.Instance.CheckIfBankExists(bank.BankName);

			if (bankExists is null) {
				return null;
			}

			if (!(bool) bankExists) {
				BankOperations.Instance.AddBank(bank);
			}
			
			return BankOperations.Instance.GetScreens(bank.BankName);
		}

		/// <summary>
		/// Checks if a bank with the given name exists.
		/// </summary>
		/// <param name="bankName">The name of the bank.</param>
		/// <returns><c>true</c> if a matching bank exists, and <c>false</c> if it does not. If the operation fails, <c>null</c> is returned.</returns>
		public bool? CheckIfBankExists(string bankName) {
			return BankOperations.Instance.CheckIfBankExists(bankName);
		}

		/// <summary>
		/// Adds a bank to the database.
		/// </summary>
		/// <param name="bank">The bank to be added to the database.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public bool AddBank(Bank bank) {
			return BankOperations.Instance.AddBank(bank);
		}

		/// <summary>
		/// Gets the screens of the specified bank.
		/// </summary>
		/// <param name="bankName">The name of the bank.</param>
		/// <returns>A list of <c>TicketingScreen</c> objects representing the screens owned by the bank. If the bank does not exist, an empty list is returned. If the operation fails, <c>null</c> is returned.</returns>
		public List<TicketingScreen>? GetScreens(string bankName) {
			return BankOperations.Instance.GetScreens(bankName);
		}

		/// <summary>
		/// Verifies the password for the specified bank.
		/// </summary>
		/// <param name="bankName">The name of the bank.</param>
		/// <param name="password">The plaintext password to be verified</param>
		/// <returns><c>true</c> if the bank exists and the password matches the one stored in the database, and <c>false</c> if it does not. If the operation fails, <c>null</c> is returned.</returns>
		public bool? VerifyPassword(string bankName, string password) {
			string? actualPassword = BankOperations.Instance.GetPassword(bankName);

            return actualPassword is null ? null : actualPassword != string.Empty && actualPassword == password;
		}
	}
}
