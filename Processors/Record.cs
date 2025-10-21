using System.Collections.Generic;
using OfficeOpenXml;

namespace NBK_RPA_CS
{
    public class Record
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Contact { get; set; } = "";
        public string MaritalStatus { get; set; } = "";
        public string Salary { get; set; } = "";
        public decimal NetSalary { get; set; } = 0m; 
        public string Periodo { get; set; } = "";
        public string VencimentosBrutos { get; set; } = "";
        public string Bonus { get; set; } = "";
        public string Seguros { get; set; } = "";
        public string Outros { get; set; } = "";
        public string PaymentMethod { get; set; } = "";
        public string ReceiptReference { get; set; } = "";
        public string ManagerSignature { get; set; } = "";
        public DateTime Date { get; set; } = DateTime.MinValue;
    

        public static Record? FromArray(string[] cols, Dictionary<string, int> indexes)
        {
            string SafeGet(string key) => indexes.ContainsKey(key) && indexes[key] < cols.Length ? cols[indexes[key]] : "";
            return new Record
            {
                Name = SafeGet("Nome"),
                Email = SafeGet("Email"),
                Contact = SafeGet("Contacto"),
                MaritalStatus = SafeGet("Estado Civil"),
                Salary = SafeGet("Salário") != "" ? SafeGet("Salário") : SafeGet("Salário Líquido"),
                Periodo = SafeGet("Período") != "" ? SafeGet("Período") : SafeGet("Período")
            };
        }

        public static Record? FromExcel(ExcelWorksheet ws, int row, Dictionary<string, int> indexes)
        {
            string SafeGet(string key) => indexes.ContainsKey(key) ? ws.Cells[row, indexes[key]].Text : "";
            return new Record
            {
                Name = SafeGet("Nome"),
                Email = SafeGet("Email"),
                Contact = SafeGet("Contacto"),
                MaritalStatus = SafeGet("Estado Civil"),
                Salary = SafeGet("Salário") != "" ? SafeGet("Salário") : SafeGet("Salário Líquido"),
                Periodo = SafeGet("Período") != "" ? SafeGet("Período") : SafeGet("Período"),
            };
        }

        public static Record? FromJson(Dictionary<string, object> data)
        {
            string Get(string k) => data.ContainsKey(k) ? data[k]?.ToString() ?? "" : "";
            return new Record
            {
                Name = Get("Nome"),
                Email = Get("Email"),
                Contact = Get("Contacto"),
                MaritalStatus = Get("Estado Civil"),
                Salary = Get("Salário") != "" ? Get("Salário") : Get("Salário Líquido"),
                Periodo = Get("Período") != "" ? Get("Período") : Get("Período"),
            };
        }
        public static Record FromTxt(string nome, string email, string contacto, string estadoCivil, decimal netSalary)
        {
            return new Record
            {
                Name = nome,
                Email = email,
                Contact = contacto,
                MaritalStatus = estadoCivil,
                Salary = netSalary.ToString("N2"),
                NetSalary = netSalary
            };
        }

        public RecordTxt ToRecordTxt(string periodo = "", string vencimentosBrutos = "", string bonus = "",
                             string seguros = "", string outros = "", string paymentMethod = "",
                             string receiptReference = "", string managerSignature = "", DateTime? date = null)
        {
            return new RecordTxt
            {
                Name = this.Name,
                Email = this.Email,
                Contact = this.Contact,
                MaritalStatus = this.MaritalStatus,
                NetSalary = this.NetSalary.ToString("N2"),
                Periodo = this.Periodo,
                VencimentosBrutos = this.VencimentosBrutos,
                Bonus = this.Bonus,
                Seguros = this.Seguros,
                Outros = this.Outros,
                PaymentMethod = this.PaymentMethod,
                ReceiptReference = this.ReceiptReference,
                ManagerSignature = managerSignature,
                Date = date ?? DateTime.Now
            };
        }

        

    }
}
