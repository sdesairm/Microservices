using System;
using Ninject;
using Microsoft.Owin.Hosting;
using Owin;
using System.Net;

namespace ServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            IKernel kernel = new StandardKernel(new CommonComponents.CommonNinjectModule());
            using (WebApp.Start<Startup>(@"http://+:5435"))
            {
                Console.ReadLine();
            }
        }
    }
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var listener = (HttpListener)app.Properties["System.Net.HttpListener"];
            listener.AuthenticationSchemes = AuthenticationSchemes.IntegratedWindowsAuthentication;
            app.UseNancy();
        }
    }
}
