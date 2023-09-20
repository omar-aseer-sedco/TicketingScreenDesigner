#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using DataAccessLayer.Constants;
using DataAccessLayer.Utils;
using ExceptionUtils;

namespace BusinessLogicLayer {
	/// <summary>
	/// This class is used to listen for database changes that match a certain entity type, bank, and (optionally) screen.
	/// </summary>
	public class DatabaseListener {
		private readonly SqlListener sqlListener;

		/// <summary>
		/// Creates an instance of <c>DatabaseListner</c>.
		/// </summary>
		/// <param name="entityType">The type of the entity to listen for (screens or buttons).</param>
		/// <param name="bankName">The name of the bank for which to listen.</param>
		/// <param name="screenId">The ID of the screen for which to listen. This is only used if <c>entityType</c> is set to Buttons.</param>
		public DatabaseListener(NotifiableEntityTypes entityType, string bankName, int screenId) {
			try {
				sqlListener = new SqlListener(entityType, bankName, screenId);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}
		
		/// <summary>
		/// Starts the listener.
		/// </summary>
		/// <returns><c>true</c> if the listener was started successfully or was already started, and <c>false</c> otherwise.</returns>
		public bool Start() {
			try {
				return sqlListener.Start();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return false;
			}
		}

		/// <summary>
		/// Stops the listener.
		/// </summary>
		/// <returns><c>true</c> if the listener was stopped successfully or was already stopped, and <c>false</c> otherwise.</returns>
		public bool Stop() {
			try {
				return sqlListener.Stop();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return false;
			}
		}
		
		/// <summary>
		/// Subscribe to the listener's delegate. This delegate gets called when a database event that matches the the listener's parameters occurs.
		/// </summary>
		/// <param name="clientDelegate">The delegate to subscribe to the listener's delegate.</param>
		public void SubscribeToDelegate(DatabaseNotificationDelegate clientDelegate) {
			try {
				sqlListener.ClientDelegate += clientDelegate;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}
	}
}
