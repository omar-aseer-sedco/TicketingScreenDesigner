using DataAccessLayer;
using DataAccessLayer.DataClasses;
using ExceptionUtils;
using LogUtils;

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
			try {
				if (BankOperations.Instance.VerifyConnection()) {
					success = true;
				}
				else {
					LogsHelper.Log("Verification failed.", DateTime.Now, EventSeverity.Error);
					success = false;
				}
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				success = false;
			}
		}

		/// <summary>
		/// Gets the screens for the specified bank if it exists. If it does not exist, creates the bank then returns the screens.
		/// </summary>
		/// <param name="bank">The bank to create/get the screens for.</param>
		/// <returns>A list of <c>TicketingScreen</c> objects representing the screens owned by the bank. If the operation fails, <c>null</c> is returned.</returns>
		public List<TicketingScreen>? GetOrCreateBank(Bank bank) {
			try {
				bool? bankExists = BankOperations.Instance.CheckIfBankExists(bank.BankName);

				if (bankExists is null) {
					return null;
				}

				if (!(bool) bankExists) {
					BankOperations.Instance.AddBank(bank);
				}
			
				return BankOperations.Instance.GetScreens(bank.BankName);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return default;
			}
		}

		/// <summary>
		/// Checks if a bank with the given name exists.
		/// </summary>
		/// <param name="bankName">The name of the bank.</param>
		/// <returns><c>true</c> if a matching bank exists, and <c>false</c> if it does not. If the operation fails, <c>null</c> is returned.</returns>
		public bool? CheckIfBankExists(string bankName) {
			try {
				return BankOperations.Instance.CheckIfBankExists(bankName);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return default;
			}
		}

		/// <summary>
		/// Adds a bank to the database.
		/// </summary>
		/// <param name="bank">The bank to be added to the database.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public bool AddBank(Bank bank) {
			try {
				return BankOperations.Instance.AddBank(bank);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return default;
			}
		}

		/// <summary>
		/// Gets the screens of the specified bank.
		/// </summary>
		/// <param name="bankName">The name of the bank.</param>
		/// <returns>A list of <c>TicketingScreen</c> objects representing the screens owned by the bank. If the bank does not exist, an empty list is returned. If the operation fails, <c>null</c> is returned.</returns>
		public List<TicketingScreen>? GetScreens(string bankName) {
			try {
				return BankOperations.Instance.GetScreens(bankName);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return default;
			}
		}

		/// <summary>
		/// Verifies the password for the specified bank.
		/// </summary>
		/// <param name="bankName">The name of the bank.</param>
		/// <param name="password">The plaintext password to be verified</param>
		/// <returns><c>true</c> if the bank exists and the password matches the one stored in the database, and <c>false</c> if it does not. If the operation fails, <c>null</c> is returned.</returns>
		public bool? VerifyPassword(string bankName, string password) {
			try {
				string? actualPassword = BankOperations.Instance.GetPassword(bankName);

				return actualPassword is null ? null : actualPassword != string.Empty && actualPassword == password;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return default;
			}
		}
	}
}
