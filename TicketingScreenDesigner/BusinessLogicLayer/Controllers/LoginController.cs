using DataAccessLayer.DataClasses;
using DataAccessLayer.DBOperations;
using DataAccessLayer.Constants;
using ExceptionUtils;
using LogUtils;
using System.Data.SqlClient;

namespace BusinessLogicLayer.Controllers {
	/// <summary>
	/// Contains methods for managing login and registration.
	/// </summary>
	public static class LoginController {
		private static bool initialized = false;

		/// <summary>
		/// Initializes the controller and verifies the database connection.
		/// </summary>
		/// <returns><c>true</c> if the database connection is verified successfully, and <c>false</c> otherwise.</returns>
		public static InitializationStatus Initialize() {
			try {
				if (initialized) {
					return InitializationStatus.SUCCESS;
				}

				var bankVerificationStatus = BankOperations.VerifyConnection();

				if (bankVerificationStatus == InitializationStatus.SUCCESS) {
					initialized = true;
				}
				else {
					LogsHelper.Log("Verification failed - Error: " + Enum.GetName(bankVerificationStatus), DateTime.Now, EventSeverity.Error);
					initialized = false;
				}

				return bankVerificationStatus;
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex);
				return InitializationStatus.FAILED_TO_CONNECT;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				initialized = false;
				return InitializationStatus.UNDEFINED_ERROR;
			}
		}

		/// <summary>
		/// Checks if a bank with the given name exists.
		/// </summary>
		/// <param name="bankName">The name of the bank.</param>
		/// <returns><c>true</c> if a matching bank exists, and <c>false</c> if it does not. If the operation fails, <c>null</c> is returned.</returns>
		public static bool? CheckIfBankExists(string bankName) {
			try {
				if (Initialize() != InitializationStatus.SUCCESS)
					return default;

				return BankOperations.CheckIfBankExists(bankName);
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
		public static bool AddBank(Bank bank) {
			try {
				if (Initialize() != InitializationStatus.SUCCESS)
					return default;

				return BankOperations.AddBank(bank);
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
		public static bool? VerifyPassword(string bankName, string password) {
			try {
				if (Initialize() != InitializationStatus.SUCCESS)
					return default;

				string? actualPassword = BankOperations.GetPassword(bankName);

				return actualPassword is null ? null : actualPassword != string.Empty && actualPassword == password;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return default;
			}
		}

		/// <summary>
		/// Changes the password for the given bank.
		/// </summary>
		/// <param name="bankName">The name of the bank.</param>
		/// <param name="oldPassword">The original password.</param>
		/// <param name="newPassword">The new password.</param>
		/// <returns><c>true</c> if the password is changed successfully, <c>false</c> if the given old password is incorrect, and <c>null</c> if the operation fails.</returns>
		public static bool? ChangePassword(string bankName, string oldPassword, string newPassword) {
			try {
				if (Initialize() != InitializationStatus.SUCCESS)
					return null;

				newPassword = newPassword.Trim();
				if (newPassword == string.Empty)
					return null;

				bool? verification = VerifyPassword(bankName, oldPassword);
				if (verification is null || verification == false)
					return verification;

				bool success = BankOperations.ChangePassword(bankName, newPassword);
				return success ? success : null;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return null;
			}
		}
	}
}
