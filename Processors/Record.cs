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

        public static Record? FromArray(string[] cols, Dictionary<string, int> indexes)
        {
            string SafeGet(string key) => indexes.ContainsKey(key) && indexes[key] < cols.Length ? cols[indexes[key]] : "";
            return new Record
            {
                Name = SafeGet("Nome"),
                Email = SafeGet("Email"),
                Contact = SafeGet("Contacto"),
                MaritalStatus = SafeGet("Estado Civil"),
                Salary = SafeGet("Salário") != "" ? SafeGet("Salário") : SafeGet("Salário Líquido")
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
                Salary = SafeGet("Salário") != "" ? SafeGet("Salário") : SafeGet("Salário Líquido")
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
                Salary = Get("Salário") != "" ? Get("Salário") : Get("Salário Líquido")
            };
        }
    }
}
