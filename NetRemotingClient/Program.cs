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
            string strFullPath = Application.ExecutablePath;
            string strFileName = System.IO.Path.GetFileName(strFullPath);
            Mutex m = new Mutex(false, strFileName, out bCreatedNew);
            if (bCreatedNew)
            {
                Application.Run(new Client());
            }           
        }
    }
}
