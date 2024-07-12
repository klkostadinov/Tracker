using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc5;
using WebApplication.Repositories;

namespace WebApplication
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            
            // register all your components with the container here
            // it is NOT necessary to register your controllers
            
            container.RegisterType<IEmployeeService, EmployeeService>();
            
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}