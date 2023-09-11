using DataAccessLayer;
using DataAccessLayer.DataClasses;
using ExceptionUtils;

namespace BusinessLogicLayer {
	/// <summary>
	/// Contains methods for managing a screen.
	/// </summary>
	public class ScreenController {
		private int pendingIndex;

		private readonly List<TicketingButton> pendingAdds;
		private readonly Dictionary<int, TicketingButton> pendingUpdates;
		private readonly HashSet<int> pendingDeletes;

		public string BankName { get; set; }
		public int ScreenId { get; set; }
		public string ScreenTitle { get; set; }

		public ScreenController() {
			pendingIndex = -1;
			BankName = string.Empty;
			ScreenId = -1;
			ScreenTitle = string.Empty;
			pendingAdds = new List<TicketingButton>();
			pendingUpdates = new Dictionary<int, TicketingButton>();
			pendingDeletes = new HashSet<int>();
		}

		public ScreenController(string bankName) : this() {
			BankName = bankName;
		}

		public ScreenController(string bankName, int screenId, string screenTitle) : this(bankName) {
			ScreenId = screenId;
			ScreenTitle = screenTitle;
		}

		/// <summary>
		/// Gets all the buttons that are commited to the database.
		/// </summary>
		/// <returns>A list of <c>TicketingButton</c> items representing the buttons in the database. If there are none, an empty list is returned. If the operation fails, <c>null</c> is returned.</returns>
		public List<TicketingButton>? GetCommittedButtons() {
			return ScreenOperations.GetButtons(BankName, ScreenId);
		}

		/// <summary>
		/// Gets all the buttons of the screen, including pending buttons.
		/// </summary>
		/// <returns>A list of <c>TicketingButton</c> items representing the buttons. If the operation fails, <c>null</c> is returned.</returns>
		public List<TicketingButton>? GetAllButtons() {
			var buttons = ScreenOperations.GetButtons(BankName, ScreenId);
			if (buttons is null)
				return null;

			foreach (var button in buttons.ToList()) {
				if (pendingDeletes.Contains(button.ButtonId) || pendingUpdates.ContainsKey(button.ButtonId)) {
					buttons.Remove(button);
				}
			}

			buttons.AddRange(pendingAdds);
			buttons.AddRange(pendingUpdates.Values.ToList());

			return buttons;
		}

		/// <summary>
		/// Adds a button to the screen. The operation is not committed to the database until <CommitChanges cref="CommitPendingChanges()"/> is called.
		/// </summary>
		/// <param name="button">The button to be added.</param>
		public void AddButtonCancellable(TicketingButton button) {
			try {
				if (button.ButtonId == 0) {
					button.ButtonId = pendingIndex;
					--pendingIndex;
				}
				pendingAdds.Add(button);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		/// <summary>
		/// Adds buttons to the screen. The operation is not committed to the database until <CommitChanges cref="CommitPendingChanges()"/> is called.
		/// </summary>
		/// <param name="buttons">A list of <c>TicketingButton</c> objects representing the buttons to be added.</param>
		public void AddButtonsCancellable(List<TicketingButton> buttons) {
			try {
				foreach (var button in buttons) {
					if (button.ButtonId == 0) {
						button.ButtonId = pendingIndex;
						--pendingIndex;
					}
				}
				pendingAdds.AddRange(buttons);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		/// <summary>
		/// Adds a button to the screen and immediately commits the change to the database.
		/// </summary>
		/// <param name="button">The button to be added.</param>
		/// <returns>The ID of the button that was added. If the operation fails, <c>null</c> is returned.</returns>
		public int? AddButtonAndCommit(TicketingButton button) {
			return ButtonOperations.AddButton(button);
		}

		/// <summary>
		/// Adds multiple buttons to the screen and immedaitely commits the changes to the database.
		/// </summary>
		/// <param name="buttons">A list of buttons to be added.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public bool AddButtonsAndCommit(List<TicketingButton> buttons) {
			return ButtonOperations.AddButtons(buttons);
		}

		/// <summary>
		/// Updates the button with the given ID. The operation is not commited to the database until <CommitChanges cref="CommitPendingChanges()"/> is called.
		/// </summary>
		/// <param name="buttonId">The ID of the button to be edited.</param>
		/// <param name="newButton">A <c>TicketingButton</c> object containing the updated button information.</param>
		public void UpdateButtonCancellable(int buttonId, TicketingButton newButton) {
			UpdateButtonsCancellable(new Dictionary<int, TicketingButton> {{ buttonId, newButton }});
		}

		/// <summary>
		/// Updates the buttons with the given buttons IDs. The operation is not commited to the database until <CommitChanges cref="CommitPendingChanges()"/> is called.
		/// </summary>
		/// <param name="buttons">A dictionary where the keys are button IDs and the corresponding values are the updated buttons.</param>
		public void UpdateButtonsCancellable(Dictionary<int, TicketingButton> buttons) {
			try {
				foreach (var button in buttons) {
					bool isPendingList = false;
					for (int i = 0; i < pendingAdds.Count; ++i) {
						if (pendingAdds[i].ButtonId == button.Key) {
							pendingAdds[i] = button.Value;
							isPendingList = true;
							break;
						}
					}
					if (!isPendingList && pendingUpdates.ContainsKey(button.Key)) {
						pendingUpdates[button.Key] = button.Value;
						isPendingList = true;
					}

					if (!isPendingList) {
						pendingUpdates.Add(button.Key != 0 ? button.Key : pendingIndex, button.Value);
						--pendingIndex;
					}
				}
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		/// <summary>
		/// Updates the button with the given ID and immediately commits the change to the database.
		/// </summary>
		/// <param name="buttonId">The ID of the button to be edited.</param>
		/// <param name="newButton">A <c>TicketingButton</c> object containing the updated button information.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public bool UpdateButtonAndCommit(int buttonId, TicketingButton newButton) {
			return ButtonOperations.UpdateButton(BankName, ScreenId, buttonId, newButton);
		}

		/// <summary>
		/// Updates the buttons with the given IDs and immediately commits the changes to the database.
		/// </summary>
		/// <param name="buttons">A dictionary where the keys are button IDs and the corresponding values are the updated buttons.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public bool UpdateButtonsAndCommit(Dictionary<int, TicketingButton> buttons) {
			return ButtonOperations.UpdateButtons(BankName, ScreenId, buttons);
		}

		/// <summary>
		/// Deletes the button with the specified ID. The delete operation is not committed to the database until <CommitChanges cref="CommitPendingChanges()"/> is called.
		/// </summary>
		/// <param name="buttonId">The ID of the button to be deleted.</param>
		public void DeleteButtonCancellable(int buttonId) {
			DeleteButtonsCancellable(new List<int> { buttonId });
		}

		/// <summary>
		/// Deletes the buttons with the specified IDs. The delete operation is not committed to the database until <CommitChanges cref="CommitPendingChanges()"/> is called.
		/// </summary>
		/// <param name="buttonIds">A list containing the IDs of the button to be deleted.</param>
		public void DeleteButtonsCancellable(List<int> buttonIds) {
			foreach (int buttonId in buttonIds) {
				bool isPendingList = false;
				foreach (var pendingButton in pendingAdds) {
					if (pendingButton.ButtonId == buttonId) {
						pendingAdds.Remove(pendingButton);
						isPendingList = true;
						break;
					}
				}
				if (pendingUpdates.ContainsKey(buttonId)) {
					pendingUpdates.Remove(buttonId);
					isPendingList = true;
				}

				if (!isPendingList) {
					pendingDeletes.Add(buttonId);
				}
			}
		}

		/// <summary>
		/// Deletes the button with the specifed ID and immediately commits the change to the database.
		/// </summary>
		/// <param name="buttonId">The ID of the button to be deleted.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public bool DeleteButtonAndCommit(int buttonId) {
			return ButtonOperations.DeleteButton(BankName, ScreenId, buttonId);
		}

		/// <summary>
		/// Deletes the buttons with specified IDs and immediately commits the changes to the database.
		/// </summary>
		/// <param name="buttonIds">A list containing the IDs of the button to be deleted.</param>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public bool DeleteButtonsAndCommit(List<int> buttonIds) {
			return ButtonOperations.DeleteButtons(BankName, ScreenId, buttonIds);
		}

		/// <summary>
		/// Commits all cancellable changes (adds, deletes, and updates) to the database.
		/// </summary>
		/// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
		public bool CommitPendingChanges() {
			return CommitAdds() && CommitUpdates() && CommitDeletes();
		}

		/// <summary>
		/// Cancels all cancellable changes (adds, deletes, and updates).
		/// </summary>
		public void CancelPendingChanges() {
			pendingAdds.Clear();
			pendingUpdates.Clear();
			pendingDeletes.Clear();
		}

		/// <summary>
		/// Checks if the button with the given ID exists in the database or in the pending list.
		/// </summary>
		/// <param name="buttonId">The ID of the button.</param>
		/// <returns><c>true</c> if a matching button exists in the database, and <c>false</c> if it does not. If the operation fails, <c>null</c> is returned.</returns>
		public bool? CheckIfButtonExists(int buttonId) {
			foreach (var button in pendingAdds) {
				if (button.ButtonId == buttonId) {
					return true;
				}
			}

			bool? existsInDatabase = ButtonOperations.CheckIfButtonExists(BankName, ScreenId, buttonId);

			if (existsInDatabase is null) return null;

			return (bool) existsInDatabase || pendingUpdates.ContainsKey(buttonId);
		}

		/// <summary>
		/// Returns the number of pending changes (adds, updates, and deletes).
		/// </summary>
		/// <returns>The number of pending changes.</returns>
		public int GetPendingChangeCount() {
			return pendingAdds.Count + pendingUpdates.Count + pendingDeletes.Count;
		}

		private bool CommitAdds() {
			if (pendingAdds.Count == 0)
				return true;

			return ButtonOperations.AddButtons(ScreenId, pendingAdds);
		}

		private bool CommitUpdates() {
			if (pendingUpdates.Count == 0)
				return true;

			return ButtonOperations.UpdateButtons(BankName, ScreenId, pendingUpdates);
		}

		private bool CommitDeletes() {
			if (pendingDeletes.Count == 0)
				return true;

			return ButtonOperations.DeleteButtons(BankName, ScreenId, pendingDeletes.ToList());
		}
	}
}
