using System;
using System.Linq;
using System.ServiceModel;

namespace IdentityService
{
    class Host
    {
        static void Main(string[] args)
        {
            Console.Title = "Service";

            var host = new ServiceHost(typeof(ClaimsService));
            host.Open();

            host.Description.Endpoints.ToList().ForEach(ep => Console.WriteLine(ep.Address));

            Console.WriteLine();
            Console.WriteLine("service ready....");
            Console.ReadLine();
            host.Close();
        }
    }
}
