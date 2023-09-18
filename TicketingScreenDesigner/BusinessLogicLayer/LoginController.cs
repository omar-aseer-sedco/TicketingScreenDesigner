﻿using DataAccessLayer;
using DataAccessLayer.DataClasses;
using ExceptionUtils;
using LogUtils;

namespace BusinessLogicLayer {
	/// <summary>
	/// Contains methods for managing login and registration.
	/// </summary>
	public static class LoginController {
		private static bool initialized = false;

		/// <summary>
		/// Initializes the controller and verifies the database connection.
		/// </summary>
		/// <returns><c>true</c> if the database connection is verified successfully, and <c>false</c> otherwise.</returns>
		public static bool Initialize() {
			try {
				if (initialized || BankOperations.VerifyConnection()) {
					initialized = true;
					return initialized;
				}
				else {
					LogsHelper.Log("Verification failed - LoginController.", DateTime.Now, EventSeverity.Error);
					initialized = false;
					return initialized;
				}
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				initialized = false;
				return initialized;
			}
		}

		/// <summary>
		/// Checks if a bank with the given name exists.
		/// </summary>
		/// <param name="bankName">The name of the bank.</param>
		/// <returns><c>true</c> if a matching bank exists, and <c>false</c> if it does not. If the operation fails, <c>null</c> is returned.</returns>
		public static bool? CheckIfBankExists(string bankName) {
			try {
				if (!Initialize())
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
				if (!Initialize())
					return default;

				return BankOperations.AddBank(bank);
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
		public static List<TicketingScreen>? GetScreens(string bankName) {
			try {
				if (!Initialize())
					return default;

				return BankOperations.GetScreens(bankName);
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
				if (!Initialize())
					return default;

				string? actualPassword = BankOperations.GetPassword(bankName);

				return actualPassword is null ? null : actualPassword != string.Empty && actualPassword == password;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return default;
			}
		}
	}
}
