using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EmployeeTimeReport
{
    public class Employee
    {
        public string Name { get; set; }
        public double TotalTime { get; set; }
    }

    class Program
    {
        private const string ApiUrl = "https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries?code=vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==";
        private const string HtmlFilePath = "index.html";

        static async Task Main(string[] args)
        {
            try
            {
                var employees = await FetchEmployeeDataAsync();
                var htmlContent = GenerateHtmlContent(employees);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static async Task<List<Employee>> FetchEmployeeDataAsync()
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(ApiUrl);
            var employees = JsonConvert.DeserializeObject<List<Employee>>(response);
            return employees;
        }

        private static string GenerateHtmlContent(List<Employee> employees)
        {
            employees = employees.OrderByDescending(e => e.TotalTime).ToList();
            var rows = employees.Select(employee =>
            {
                var rowClass = employee.TotalTime < 100 ? "low-hours" : "";
                return $@"
            <tr class='{rowClass}'>
                <td>{employee.Name}</td>
                <td>{employee.TotalTime}</td>
            </tr>";
            });

            return string.Join(Environment.NewLine, rows);
        }

    }
}
