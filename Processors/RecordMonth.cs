namespace NBK_RPA_CS
{
    public class RecordMonth
    {
        public string Periodo { get; set; } = "";
        public string VencimentosBrutos { get; set; } = "";
        public string Bonus { get; set; } = "";
        public string Seguros { get; set; } = "";
        public string SalarioLiquido { get; set; } = "";
        public string Outros { get; set; } = "";
        public string PaymentMethod { get; set; } = "";
        public string ReceiptReference { get; set; } = "";
        public string ManagerSignature { get; set; } = "";
        public DateTime Date { get; set; }
    }
}
