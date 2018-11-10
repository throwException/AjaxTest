using Nancy;
using Nancy.Hosting.Self;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NancyAjaxTest
{
    public static class MainClass
    {
        public static void Main(string[] args)
        {
            var uri = "http://localhost:8888";
            Console.WriteLine("Starting Nancy on " + uri);

            // initialize an instance of NancyHost
            var host = new NancyHost(new Uri(uri));
            host.Start();  // start hosting

            while (true)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
