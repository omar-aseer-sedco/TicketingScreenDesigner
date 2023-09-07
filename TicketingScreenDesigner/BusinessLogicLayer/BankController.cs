using DataAccessLayer;
using DataAccessLayer.DataClasses;

namespace BusinessLogicLayer {
	public static class BankController {
		public static List<TicketingScreen> AccessBank(Bank bank) {
			if (!BankOperations.CheckIfBankExists(bank.BankName)) {
				BankOperations.AddBank(bank);
			}
			
			return BankOperations.GetScreens(bank.BankName);
		}
	}
}
