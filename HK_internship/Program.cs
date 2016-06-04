using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HK_internship
{
    class Program
    {
        // <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string[] arr2 = {"http://10.0.2.2:9000/", Environment.GetEnvironmentVariable("base")};
            Console.WriteLine(arr2[1]);
            Proxy.SimpleListenerExample(arr2);
        }
    }
}
