namespace DataAccessLayer.DataClasses {
    public class BankService {
        public string BankName { get; set; } = string.Empty;
        public int BankServiceId { get; set; } = 0;
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public bool Active { get; set; } = false;
        public int MaxDailyTickets { get; set; } = 0;
        public int MinServiceTime { get; set; } = 45;
        public int MaxServiceTime { get; set; } = 300;
    }
}
