#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using DataAccessLayer.DataClasses;
using DataAccessLayer.DBOperations;
using ExceptionUtils;
using LogUtils;

namespace BusinessLogicLayer.Controllers {
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

        /// <summary>
        /// Creates a <c>ScreenController</c> object.
        /// </summary>
        /// <param name="success"><c>true</c> if the connection with the database was established successfully, and <c>false</c> otherwise.</param>
        public ScreenController(out bool success) {
            try {
                if (ScreenOperations.VerifyConnection() && ButtonOperations.VerifyConnection()) {
                    success = true;
                }
                else {
                    LogsHelper.Log("Verification failed - ScreenController.", DateTime.Now, EventSeverity.Error);
                    success = false;
                }

                pendingIndex = -1;
                BankName = string.Empty;
                ScreenId = -1;
                ScreenTitle = string.Empty;
                pendingAdds = new List<TicketingButton>();
                pendingUpdates = new Dictionary<int, TicketingButton>();
                pendingDeletes = new HashSet<int>();
            }
            catch (Exception ex) {
                ExceptionHelper.HandleGeneralException(ex);
                success = false;
            }
        }

        /// <summary>
        /// Creates a <c>ScreenController</c> object with the given bank name.
        /// </summary>
        /// <param name="success"><c>true</c> if the connection with the database was established successfully, and <c>false</c> otherwise.</param>
        /// <param name="bankName">The name of the bank.</param>
        public ScreenController(out bool success, string bankName) : this(out success) {
            try {
                BankName = bankName;
            }
            catch (Exception ex) {
                ExceptionHelper.HandleGeneralException(ex);
                success = false;
            }
        }

        /// <summary>
        /// Creates a <c>ScreenController</c> object with the given bank name and screen information.
        /// </summary>
        /// <param name="success"><c>true</c> if the connection with the database was established successfully, and <c>false</c> otherwise.</param>
        /// <param name="bankName">The name of the bank.</param>
        /// <param name="screenId">The ID fo the screen.</param>
        /// <param name="screenTitle">The title of the screen.</param>
        public ScreenController(out bool success, string bankName, int screenId, string screenTitle) : this(out success, bankName) {
            try {
                ScreenId = screenId;
                ScreenTitle = screenTitle;
            }
            catch (Exception ex) {
                ExceptionHelper.HandleGeneralException(ex);
                success = false;
            }
        }

        /// <summary>
        /// Asynchronously gets all the buttons of the screen, including pending buttons.
        /// </summary>
        /// <returns>A list of <c>TicketingButton</c> items representing the buttons. If the operation fails, <c>null</c> is returned.</returns>
        public async Task<List<TicketingButton>?> GetAllButtonsAsync() {
            try {
                var buttons = await ScreenOperations.GetButtonsAsync(BankName, ScreenId);
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
            catch (Exception ex) {
                ExceptionHelper.HandleGeneralException(ex);
                return default;
            }
        }

        /// <summary>
        /// Adds a button to the screen. The operation is not committed to the database until <CommitChanges cref="CommitPendingChanges()"/> is called.
        /// </summary>
        /// <param name="button">The button to be added.</param>
        public void AddButtonCancellable(TicketingButton button) {
            try {
                if (button.ButtonId == 0) {
                    button.ButtonId = GetNextPendingIndex();
                }
                pendingAdds.Add(button);
            }
            catch (Exception ex) {
                ExceptionHelper.HandleGeneralException(ex);
            }
        }

        /// <summary>
        /// Updates the button with the given ID. The operation is not commited to the database until <CommitChanges cref="CommitPendingChanges()"/> is called.
        /// </summary>
        /// <param name="buttonId">The ID of the button to be edited. The ID should not be 0.</param>
        /// <param name="newButton">A <c>TicketingButton</c> object containing the updated button information.</param>
        public void UpdateButtonCancellable(int buttonId, TicketingButton newButton) {
            try {
                if (buttonId == 0)
                    throw new ArgumentOutOfRangeException(nameof(buttonId), "Button ID should not be 0. It has to be either negative (for an uncommited button) or positive (for a committed button).");

                bool isPendingList = false;
                for (int i = 0; i < pendingAdds.Count; ++i) {
                    if (pendingAdds[i].ButtonId == buttonId) {
                        pendingAdds[i] = newButton;
                        isPendingList = true;
                        break;
                    }
                }

                if (!isPendingList && pendingUpdates.ContainsKey(buttonId)) {
                    pendingUpdates[buttonId] = newButton;
                    isPendingList = true;
                }

                if (!isPendingList) {
                    pendingUpdates.Add(buttonId, newButton);
                }
            }
            catch (Exception ex) {
                ExceptionHelper.HandleGeneralException(ex);
            }
        }

        /// <summary>
        /// Deletes the buttons with the specified IDs. The delete operation is not committed to the database until <CommitChanges cref="CommitPendingChanges()"/> is called.
        /// </summary>
        /// <param name="buttonIds">A list containing the IDs of the button to be deleted.</param>
        public void DeleteButtonsCancellable(List<int> buttonIds) {
            try {
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
            catch (Exception ex) {
                ExceptionHelper.HandleGeneralException(ex);
            }
        }

        /// <summary>
        /// Commits all cancellable changes (adds, deletes, and updates) to the database.
        /// </summary>
        /// <returns><c>true</c> if the operation succeeds, and <c>false</c> if it fails.</returns>
        public bool CommitPendingChanges(out List<int>? failedIds) {
            try {
                failedIds = new List<int>();

                var success = ButtonOperations.AtomicCommit(BankName, ScreenId, pendingAdds, pendingUpdates, pendingDeletes.ToList());

                if (success is null)
                    return default;

                bool ret = true;
                foreach (var i in success) {
                    if (!i.Value) {
                        failedIds.Add(i.Key);
                        ret = false;
                    }
                }

                return ret;
            }
            catch (Exception ex) {
                ExceptionHelper.HandleGeneralException(ex);
                failedIds = default;
                return default;
            }
        }

        /// <summary>
        /// Cancels all cancellable changes (adds, deletes, and updates).
        /// </summary>
        public void CancelPendingChanges() {
            try {
                pendingAdds.Clear();
                pendingUpdates.Clear();
                pendingDeletes.Clear();
            }
            catch (Exception ex) {
                ExceptionHelper.HandleGeneralException(ex);
            }
        }

        /// <summary>
        /// Checks if the button with the given ID exists in the database or in the pending list.
        /// </summary>
        /// <param name="buttonId">The ID of the button.</param>
        /// <returns><c>true</c> if a matching button exists in the database, and <c>false</c> if it does not. If the operation fails, <c>null</c> is returned.</returns>
        public bool? CheckIfButtonExists(int buttonId) {
            try {
                foreach (var button in pendingAdds) {
                    if (button.ButtonId == buttonId) {
                        return true;
                    }
                }

                bool? existsInDatabase = ButtonOperations.CheckIfButtonExists(BankName, ScreenId, buttonId);

                if (existsInDatabase is null) return null;

                return (bool) existsInDatabase || pendingUpdates.ContainsKey(buttonId);
            }
            catch (Exception ex) {
                ExceptionHelper.HandleGeneralException(ex);
                return default;
            }
        }

        /// <summary>
        /// Returns the number of pending changes (adds, updates, and deletes).
        /// </summary>
        /// <returns>The number of pending changes.</returns>
        public int GetPendingChangeCount() {
            try {
                return pendingAdds.Count + pendingUpdates.Count + pendingDeletes.Count;
            }
            catch (Exception ex) {
                ExceptionHelper.HandleGeneralException(ex);
                return -1;
            }
        }

        /// <summary>
        /// Returns a temporary index for a button that is still not committed to the database.
        /// </summary>
        /// <returns>The temporary index. It is always a negative number.</returns>
        public int GetNextPendingIndex() {
            --pendingIndex;
            return pendingIndex;
        }
    }
}
