using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace NetRemotingClient
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            bool bCreatedNew;
            Mutex m = new Mutex(false, Application.ProductName, out bCreatedNew);
            if (bCreatedNew)
            {
                Application.Run(new Client());
            }           
        }
    }
}
