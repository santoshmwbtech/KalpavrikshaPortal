using Repository.Interfaces;
using Repository.Services;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

namespace JBNAdminPortal
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<IUserRepository, UserRepository>();
            container.RegisterType<IDashboardRepository, DashboardRepository>();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}