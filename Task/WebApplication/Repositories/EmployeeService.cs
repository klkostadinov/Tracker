using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using PagedList;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Web;
using WebApplication.Models;
using WebApplication;
using System;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;

namespace WebApplication.Repositories
{
    public class EmployeeService : IEmployeeService
    {
        [Dependency]
        public EmployeesDbContext Context { get; set; }

        public IPagedList<EmployeesArrival> GetEmployees(int? id, string sortorder, DateTime? subscriptionDate, int? page)
        {
            IQueryable<EmployeesArrival> employees = null;

            if (id != null && subscriptionDate != null)
            {
                employees = Context.EmployeesArrival.Where(s => s.EmployeeId == id && DbFunctions.TruncateTime(s.ArrivalTime) == ((DateTime)subscriptionDate).Date);
            }
            else if (id != null)
            {
                employees = Context.EmployeesArrival.Where(s => s.EmployeeId == id);
            }
            else if (subscriptionDate != null)
            {
                employees = Context.EmployeesArrival.Where(s => DbFunctions.TruncateTime(s.ArrivalTime) == ((DateTime)subscriptionDate).Date);
            }
            else
            {
                employees = Context.EmployeesArrival;
            }

            switch (sortorder)
            {
                case "id_desc":
                    employees = employees.OrderByDescending(s => s.EmployeeId);
                    break;
                case "date_asc":
                    employees = employees.OrderBy(s => s.ArrivalTime);
                    break;
                case "date_desc":
                    employees = employees.OrderByDescending(s => s.ArrivalTime);
                    break;
                default:
                    employees = employees.OrderBy(s => s.EmployeeId);
                    break;
            }

            int pageSize = 20;
            int pageNumber = (page ?? 1);

            return employees.ToPagedList(pageNumber, pageSize);
        }

        public void SaveEmployees(List<SimulationData> employees)
        {
            foreach (var employee in employees)
            {
                Context.EmployeesArrival.Add(new EmployeesArrival() { Id = Guid.NewGuid(), EmployeeId = employee.EmployeeId, ArrivalTime = employee.When });
            }

            Context.SaveChanges();
        }

        public void Dispose()
        {
            if (Context != null)
            {
                Context.Dispose();
            }
        }
    }
}