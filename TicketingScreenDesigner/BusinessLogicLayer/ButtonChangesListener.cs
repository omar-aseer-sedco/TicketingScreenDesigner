using DataAccessLayer.Utils;
using ExceptionUtils;

namespace BusinessLogicLayer {
	public class ButtonChangesListener : DatabaseListener {
		protected override SqlListener SqlListener { get; set; }

		public ButtonChangesListener(string bankName, int screenId) {
			SqlListener = new SqlButtonListener(bankName, screenId);
		}
	}
}
