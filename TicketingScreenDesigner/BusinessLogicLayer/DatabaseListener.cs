using DataAccessLayer.Constants;
using DataAccessLayer.Utils;
using ExceptionUtils;

namespace BusinessLogicLayer {
	public abstract class DatabaseListener {
		protected abstract SqlListener SqlListener { get; set; }

		/// <summary>
		/// Starts the listener.
		/// </summary>
		/// <returns><c>true</c> if the listener was started successfully or was already started, and <c>false</c> otherwise.</returns>
		public bool Start() {
			try {
				return SqlListener.Start();
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
				return SqlListener.Stop();
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
				SqlListener.ClientDelegate += clientDelegate;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}
	}
}
