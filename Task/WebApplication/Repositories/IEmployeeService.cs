using PagedList;
using System;
using System.Collections.Generic;
using WebApplication.Models;

namespace WebApplication.Repositories
{
    public interface IEmployeeService : IDisposable
    {
        IPagedList<EmployeesArrival> GetEmployees(int? id, string sortorder, DateTime? subscriptionDate, int? page);
        void SaveEmployees(List<SimulationData> employees);
    }
}
