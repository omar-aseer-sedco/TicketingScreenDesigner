#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using DataAccessLayer;
using DataAccessLayer.DataClasses;
using ExceptionUtils;
using LogUtils;

namespace BusinessLogicLayer {
	/// <summary>
	/// Contains methods for managing a bank.
	/// </summary>
	public class BankController {
		public string BankName { get; set; }

		/// <summary>
		/// Creates a <c>BankController</c> object.
		/// </summary>
		/// <param name="success"><c>true</c> if the connection with the database was established successfully, and <c>false</c> otherwise.</param>
		public BankController(out bool success) {
			try {
				if (ScreenOperations.Instance.VerifyConnection() && BankOperations.Instance.VerifyConnection()) {
					success = true;
				}
				else {
					LogsHelper.Log("Verification failed - BankController.", DateTime.Now, EventSeverity.Error);
					success = false;
				}

				BankName = string.Empty;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				success = false;
			}
		}

		/// <summary>
		/// Creates a <c>BankController</c> object with the specifed bank name.
		/// </summary>
		/// <param name="success"><c>true</c> if the connection with the database was established successfully, and <c>false</c> otherwise.</param>
		/// <param name="bankName">The name of the bank.</param>
		public BankController(out bool success, string bankName) : this(out success) {
			try {
				BankName = bankName;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				success = false;
			}
		}

		/// <summary>
		/// Adds a screen to the database.
		/// </summary>
		/// <param name="screen">The screen to be added to the database.</param>
		/// <returns>The ID of the screen. If the operation fails, <c>null</c> is returned.</returns>
		public int? AddScreen(TicketingScreen screen) {
			try {
				return ScreenOperations.Instance.AddScreen(screen);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return default;
			}
		}

		/// <summary>
		/// Updates the screen with the given ID to newScreen.
		/// </summary>
		/// <param name="screenId">The ID of the screen to be updated</param>
		/// <param name="newScreen">A <c>TicketingScreen</c> object representing the updated screen.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public bool UpdateScreen(int screenId, TicketingScreen newScreen) {
			try {
				return ScreenOperations.Instance.UpdateScreen(BankName, screenId, newScreen);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return default;
			}
		}

		/// <summary>
		/// Delete the screen with the specified ID.
		/// </summary>
		/// <param name="screenId">The ID of the screen to be deleted.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public bool DeleteScreen(int screenId) {
			try {
				return ScreenOperations.Instance.DeleteScreen(BankName, screenId);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return default;
			}
		}

		/// <summary>
		/// Delete multiple screens.
		/// </summary>
		/// <param name="screenIds">A list containing the IDs of the screens to be deleted.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public bool DeleteScreens(List<int> screenIds) {
			try {
				return ScreenOperations.Instance.DeleteScreens(BankName, screenIds);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return default;
			}
		}

		/// <summary>
		/// Gets the ID of the active screen.
		/// </summary>
		/// <returns>The ID of the active screen. If no screens are active, <c>-1</c> is returned. If the operation fails, <c>null</c> is returned.</returns>
		public int? GetActiveScreenId() {
			try {
				return ScreenOperations.Instance.GetActiveScreenId(BankName);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return default;
			}
		}

		/// <summary>
		/// Checks if the screen with the given ID exists.
		/// </summary>
		/// <param name="screenId">The ID of the screen.</param>
		/// <returns>Returns <c>true</c> if a matching screen exists, and <c>false</c> if it does not. If the operation fails, <c>null</c> is returned.</returns>
		public bool? CheckIfScreenExists(int screenId) {
			try {
				return ScreenOperations.Instance.CheckIfScreenExists(BankName, screenId);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return default;
			}
		}

		/// <summary>
		/// Gets all the screens for the bank.
		/// </summary>
		/// <returns>A list of <c>TicketingScreen</c> objects representing the screens of the bank. If the bank does not exist, an empty list is returned. If the operation fails, <c>null</c> is returned.</returns>
		public List<TicketingScreen>? GetScreens() {
			try {
				return BankOperations.Instance.GetScreens(BankName);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return default;
			}
		}

		/// <summary>
		/// Sets the screen with the given ID as the active screen. If a different screen is already active, it is deactivated.
		/// </summary>
		/// <param name="screenId">The ID of the screen.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public bool ActivateScreen(int screenId) {
			try {
				return ScreenOperations.Instance.ActivateScreen(BankName, screenId);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return default;
			}
		}

		/// <summary>
		/// Sets the screen with the given ID to inactive.
		/// </summary>
		/// <param name="screenId">The ID of the screen.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public bool DeactivateScreen(int screenId) {
			try {
				return ScreenOperations.Instance.DeactivateScreen(BankName, screenId);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return default;
			}
		}

		/// <summary>
		/// Gets the buttons for the screen with the given ID.
		/// </summary>
		/// <param name="screenId">The ID of the screen</param>
		/// <returns>A list of <c>TicketingButton</c> items representing the buttons of the screen. If the screen does not have buttons, an empty list is returned. If the operation fails, <c>null</c> is returned.</returns>
		public List<TicketingButton>? GetButtons(int screenId) {
			try {
				return ScreenOperations.Instance.GetButtons(BankName, screenId);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return default;
			}
		}
	}
}
