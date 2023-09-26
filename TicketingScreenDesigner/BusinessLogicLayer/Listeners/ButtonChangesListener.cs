using DataAccessLayer.Listeners;

namespace BusinessLogicLayer.Listeners {
	public class ButtonChangesListener : DatabaseListener {
		protected override SqlListener SqlListener { get; set; }

		public ButtonChangesListener(string bankName, int screenId) {
			SqlListener = new SqlButtonListener(bankName, screenId);
		}
	}
}
