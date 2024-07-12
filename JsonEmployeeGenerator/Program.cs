using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JsonEmployeeGenerator
{
    class Program
    {
        static void Main(string[] args)
        {

            string[] roles = new string[] { "Junior Developer", "Semi Senior Developer", "Senior Developer", "Principal", "Team Leader" };
            string[] teams = new string[] { "Platform", "Sales", "Billing", "Mirage" };


            var generator = new Random();
            var all_lines_in_file = File.ReadAllLines("employees.txt").ToArray();
            var employees = new List<JsonEmployee>();
            for (int i = 0; i < all_lines_in_file.Length; i++)
            {
                JsonEmployee e = new JsonEmployee();
                e.Id = i;
                e.Name = all_lines_in_file[i].Split('\t')[0];
                e.SurName = all_lines_in_file[i].Split('\t')[1];
                e.Email = all_lines_in_file[i].Split('\t')[2];
                e.Age = generator.Next(18, 66);
                if (i < 11)
                {
                    e.Role = "Manager";
                    e.Teams = new List<string>();
                }
                else
                {
                    e.ManagerId = generator.Next(11);
                    e.Role = roles[generator.Next(4)];
                    int count = generator.Next(1, 4);
                    var employeeTeams = new List<string>();
                    for (int j = 0; j < count; ++j)
                    {
                        employeeTeams.Add(teams[generator.Next(4)]);
                    }
                    e.Teams = employeeTeams;
                }

                employees.Add(e);
            }
            var jsonFile = File.CreateText("employees.json");
            jsonFile.WriteLine("[");

            for (int i = 0; i < employees.Count; ++i)
            {
                var jsonEmployee = employees[i];
                string str =
                    "{{\"Id\":{7},\"ManagerId\":{0},\"Age\":{1},\"Teams\":[{2}],\"Role\":\"{3}\",\"Email\":\"{4}\",\"SurName\":\"{5}\",\"Name\":\"{6}\"}}";
                if (i != employees.Count - 1)
                    str += ",";
                var formattedEmployeed = string.Format(str,
                    jsonEmployee.ManagerId.HasValue ? jsonEmployee.ManagerId.ToString() : "null",
                    jsonEmployee.Age,
                    string.Join(",", jsonEmployee.Teams.Select(x => "\"" + x + "\"")),
                    jsonEmployee.Role,
                    jsonEmployee.Email,
                    jsonEmployee.SurName,
                    jsonEmployee.Name,
                    jsonEmployee.Id);
                jsonFile.WriteLine(formattedEmployeed);
            }
            jsonFile.WriteLine("]");
            jsonFile.Flush();
        }

    }

    internal class JsonEmployee
    {
        public string Name { get; set; }
        public string SurName { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public string Role { get; set; }
        public int? ManagerId { get; set; }
        public List<string> Teams { get; set; }
        public int Id { get; set; }
    }
}
