using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SD
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
            //Application.Run(new LoginForm());
        }
    }

    static class Mode
    {
        public static bool mode { get; set; }
    }

    static class Constants
    {
        public static string UvnsDigiPass { get; set; }
        public static string DigiSshPass { get; set; }
        public static string DigiSshLogin { get; set; }
        public static string SalepointPass { get; set; }
        public static string SalepointLogin { get; set; }
        public static string SrvSmenaRootPass { get; set; }
        public static string SrvSmenaRootLogin { get; set; }
        public static string SrvSpdaemonPass { get; set; }
        public static string SrvSpdaemonLogin { get; set; }
        public static string DigitpricePass { get; set; }
        public static string DigitpriceLogin { get; set; }
        public static string BdLogin { get; set; }
        public static string BdPass { get; set; }
        public static bool Mode { get; set; }
        public static string UserLogin { get; set; }
        public static string UserPass { get; set; }
        public static string UserStatus { get; set; }
        public static string SqlFile { get; set; }

    }
}
