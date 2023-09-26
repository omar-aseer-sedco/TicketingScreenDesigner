using DataAccessLayer.Constants;
using System.Data.SqlClient;
using System.Data;
using ExceptionUtils;

namespace DataAccessLayer.Listeners {
	public class SqlScreenListener : SqlListener {
		private readonly string bankName;

		public SqlScreenListener(string bankName) : base() {
			try {
				this.bankName = bankName;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				this.bankName = string.Empty;
			}
		}

		protected override SqlCommand CreateCommand() {
			try {
				CommandText = $"SELECT {ScreensConstants.SCREEN_TITLE}, {ScreensConstants.IS_ACTIVE} FROM dbo.{ScreensConstants.TABLE_NAME} WHERE {ScreensConstants.BANK_NAME} = @bankName;";

				SqlCommand command = new SqlCommand(CommandText, connection);
				command.Parameters.Add("@bankName", SqlDbType.VarChar, BanksConstants.BANK_NAME_SIZE).Value = bankName;

				return command;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				return new SqlCommand();
			}
		}
	}
}
