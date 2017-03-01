using Ninject;
using System;

namespace ServiceClient
{
    class Program
    {
        static void Main(string[] args)
        {
            IKernel kernel = new StandardKernel(new ClientNinjectModule());

            Console.WriteLine(Providers.MyModuleProvider.GetMyModule().GetModel(2345));
            Console.ReadLine();
        }
    }
}
