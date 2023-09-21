using DataAccessLayer.Utils;
using ExceptionUtils;

namespace BusinessLogicLayer {
	/// <summary>
	/// This class is used to listen for database changes that match a certain entity type, bank, and (optionally) screen.
	/// </summary>
	public class ScreenChangesListener : DatabaseListener {
		protected override SqlListener SqlListener { get; set; }

		/// <summary>
		/// Creates an instance of <c>DatabaseListner</c>.
		/// </summary>
		/// <param name="entityType">The type of the entity to listen for (screens or buttons).</param>
		/// <param name="bankName">The name of the bank for which to listen.</param>
		/// <param name="screenId">The ID of the screen for which to listen. This is only used if <c>entityType</c> is set to Buttons.</param>
		public ScreenChangesListener(string bankName) : base() {
			SqlListener = new SqlScreenListener(bankName);
		}
	}
}
