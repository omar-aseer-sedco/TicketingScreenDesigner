using DataAccessLayer;
using DataAccessLayer.DataClasses;

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
			if (ScreenOperations.Instance.VerifyConnection() && BankOperations.Instance.VerifyConnection()) {
				success = true;
			}
			else {
				success = false;
			}

			BankName = string.Empty;
		}

		/// <summary>
		/// Creates a <c>BankController</c> object with the specifed bank name.
		/// </summary>
		/// <param name="success"><c>true</c> if the connection with the database was established successfully, and <c>false</c> otherwise.</param>
		/// <param name="bankName">The name of the bank.</param>
		public BankController(out bool success, string bankName) : this(out success) {
			BankName = bankName;
		}

		/// <summary>
		/// Adds a screen to the database.
		/// </summary>
		/// <param name="screen">The screen to be added to the database.</param>
		/// <returns>The ID of the screen. If the operation fails, <c>null</c> is returned.</returns>
		public int? AddScreen(TicketingScreen screen) {
			return ScreenOperations.Instance.AddScreen(screen);
		}

		/// <summary>
		/// Updates the screen with the given ID to newScreen.
		/// </summary>
		/// <param name="screenId">The ID of the screen to be updated</param>
		/// <param name="newScreen">A <c>TicketingScreen</c> object representing the updated screen.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public bool UpdateScreen(int screenId, TicketingScreen newScreen) {
			return ScreenOperations.Instance.UpdateScreen(BankName, screenId, newScreen);
		}

		/// <summary>
		/// Delete the screen with the specified ID.
		/// </summary>
		/// <param name="screenId">The ID of the screen to be deleted.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public bool DeleteScreen(int screenId) {
			return ScreenOperations.Instance.DeleteScreen(BankName, screenId);
		}

		/// <summary>
		/// Delete multiple screens.
		/// </summary>
		/// <param name="screenIds">A list containing the IDs of the screens to be deleted.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public bool DeleteScreens(List<int> screenIds) {
			return ScreenOperations.Instance.DeleteScreens(BankName, screenIds);
		}

		/// <summary>
		/// Gets the ID of the active screen.
		/// </summary>
		/// <returns>The ID of the active screen. If no screens are active, <c>-1</c> is returned. If the operation fails, <c>null</c> is returned.</returns>
		public int? GetActiveScreenId() {
			return ScreenOperations.Instance.GetActiveScreenId(BankName);
		}

		/// <summary>
		/// Checks if the screen with the given ID exists.
		/// </summary>
		/// <param name="screenId">The ID of the screen.</param>
		/// <returns>Returns <c>true</c> if a matching screen exists, and <c>false</c> if it does not. If the operation fails, <c>null</c> is returned.</returns>
		public bool? CheckIfScreenExists(int screenId) {
			return ScreenOperations.Instance.CheckIfScreenExists(BankName, screenId);
		}

		/// <summary>
		/// Gets all the screens for the bank.
		/// </summary>
		/// <returns>A list of <c>TicketingScreen</c> objects representing the screens of the bank. If the bank does not exist, an empty list is returned. If the operation fails, <c>null</c> is returned.</returns>
		public List<TicketingScreen>? GetScreens() {
			return BankOperations.Instance.GetScreens(BankName);
		}

		/// <summary>
		/// Sets the screen with the given ID as the active screen. If a different screen is already active, it is deactivated.
		/// </summary>
		/// <param name="screenId">The ID of the screen.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public bool ActivateScreen(int screenId) {
			return ScreenOperations.Instance.ActivateScreen(BankName, screenId);
		}

		/// <summary>
		/// Sets the screen with the given ID to inactive.
		/// </summary>
		/// <param name="screenId">The ID of the screen.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public bool DeactivateScreen(int screenId) {
			return ScreenOperations.Instance.DeactivateScreen(BankName, screenId);
		}

		/// <summary>
		/// Gets the buttons for the screen with the given ID.
		/// </summary>
		/// <param name="screenId">The ID of the screen</param>
		/// <returns>A list of <c>TicketingButton</c> items representing the buttons of the screen. If the screen does not have buttons, an empty list is returned. If the operation fails, <c>null</c> is returned.</returns>
		public List<TicketingButton>? GetButtons(int screenId) {
			return ScreenOperations.Instance.GetButtons(BankName, screenId);
		}
	}
}
