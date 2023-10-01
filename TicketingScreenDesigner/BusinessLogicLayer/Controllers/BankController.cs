using DataAccessLayer.DataClasses;
using DataAccessLayer.DBOperations;
using DataAccessLayer.Constants;
using ExceptionUtils;
using LogUtils;
using System.Data.SqlClient;

namespace BusinessLogicLayer.Controllers {
	/// <summary>
	/// Contains methods for managing a bank.
	/// </summary>
	public static class BankController {
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
				var screenVerificationStatus = ScreenOperations.VerifyConnection();
				if (bankVerificationStatus == InitializationStatus.SUCCESS && screenVerificationStatus == InitializationStatus.SUCCESS) {
					initialized = true;
					return InitializationStatus.SUCCESS;
				}
				else {
					LogsHelper.Log("Verification failed - BankController.", DateTime.Now, EventSeverity.Error);
					initialized = false;
					return bankVerificationStatus == InitializationStatus.SUCCESS ? screenVerificationStatus : bankVerificationStatus;
				}
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
		/// Adds a screen to the database.
		/// </summary>
		/// <param name="screen">The screen to be added to the database.</param>
		/// <returns>The ID of the screen. If the operation fails, <c>null</c> is returned.</returns>
		public static int? AddScreen(TicketingScreen screen) {
			try {
				if (Initialize() != InitializationStatus.SUCCESS)
					return default;

				return ScreenOperations.AddScreen(screen);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return default;
			}
		}

		/// <summary>
		/// Updates the screen with the given ID to newScreen.
		/// </summary>
		/// <param name="bankName">The name of the bank.</param>
		/// <param name="screenId">The ID of the screen to be updated</param>
		/// <param name="newScreen">A <c>TicketingScreen</c> object representing the updated screen.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public static bool UpdateScreen(string bankName, int screenId, TicketingScreen newScreen) {
			try {
				if (Initialize() != InitializationStatus.SUCCESS)
					return default;

				return ScreenOperations.UpdateScreen(bankName, screenId, newScreen);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return default;
			}
		}

		/// <summary>
		/// Delete multiple screens.
		/// </summary>
		/// <param name="bankName">The name of the bank.</param>
		/// <param name="screenIds">A list containing the IDs of the screens to be deleted.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public static bool DeleteScreens(string bankName, List<int> screenIds) {
			try {
				if (Initialize() != InitializationStatus.SUCCESS)
					return default;

				return ScreenOperations.DeleteScreens(bankName, screenIds);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return default;
			}
		}

		/// <summary>
		/// Gets the ID of the active screen.
		/// </summary>
		/// <param name="bankName">The name of the bank.</param>
		/// <returns>The ID of the active screen. If no screens are active, <c>-1</c> is returned. If the operation fails, <c>null</c> is returned.</returns>
		public static int? GetActiveScreenId(string bankName) {
			try {
				if (Initialize() != InitializationStatus.SUCCESS)
					return default;

				return ScreenOperations.GetActiveScreenId(bankName);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return default;
			}
		}

		/// <summary>
		/// Checks if the screen with the given ID exists.
		/// </summary>
		/// <param name="bankName">The name of the bank.</param>
		/// <param name="screenId">The ID of the screen.</param>
		/// <returns>Returns <c>true</c> if a matching screen exists, and <c>false</c> if it does not. If the operation fails, <c>null</c> is returned.</returns>
		public static bool? CheckIfScreenExists(string bankName, int screenId) {
			try {
				if (Initialize() != InitializationStatus.SUCCESS)
					return default;

				return ScreenOperations.CheckIfScreenExists(bankName, screenId);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return default;
			}
		}

		/// <summary>
		/// Asynchronously gets all the screens for the bank.
		/// </summary>
		/// <param name="bankName">The name of the bank.</param>
		/// <returns>A list of <c>TicketingScreen</c> objects representing the screens of the bank. If the bank does not exist, an empty list is returned. If the operation fails, <c>null</c> is returned.</returns>
		public static async Task<List<TicketingScreen>?> GetScreensAsync(string bankName) {
			try {
				if (Initialize() != InitializationStatus.SUCCESS)
					return default;

				return await BankOperations.GetScreensAsync(bankName);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return default;
			}
		}

		/// <summary>
		/// Sets the screen with the given ID as the active screen. If a different screen is already active, it is deactivated.
		/// </summary>
		/// <param name="bankName">The name of the bank.</param>
		/// <param name="screenId">The ID of the screen.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public static bool ActivateScreen(string bankName, int screenId) {
			try {
				if (Initialize() != InitializationStatus.SUCCESS)
					return default;

				return ScreenOperations.SetIsActive(bankName, screenId, true);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return default;
			}
		}

		/// <summary>
		/// Sets the screen with the given ID to inactive.
		/// </summary>
		/// <param name="bankName">The name of the bank.</param>
		/// <param name="screenId">The ID of the screen.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public static bool DeactivateScreen(string bankName, int screenId) {
			try {
				if (Initialize() != InitializationStatus.SUCCESS)
					return default;

				return ScreenOperations.SetIsActive(bankName, screenId, false);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return default;
			}
		}

		/// <summary>
		/// Gets the buttons for the screen with the given ID.
		/// </summary>
		/// <param name="bankName">The name of the bank.</param>
		/// <param name="screenId">The ID of the screen</param>
		/// <returns>A list of <c>TicketingButton</c> items representing the buttons of the screen. If the screen does not have buttons, an empty list is returned. If the operation fails, <c>null</c> is returned.</returns>
		public static async Task<List<TicketingButton>?> GetButtons(string bankName, int screenId) {
			try {
				if (Initialize() != InitializationStatus.SUCCESS)
					return default;

				return await ScreenOperations.GetButtonsAsync(bankName, screenId);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return default;
			}
		}
	}
}
