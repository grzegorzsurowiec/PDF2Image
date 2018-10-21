using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace PDF2Image
{
    /// <summary>
    /// Logika interakcji dla klasy App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            this.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorFolder = string.Format("errors{0}", Path.DirectorySeparatorChar);

                if (!Directory.Exists(errorFolder)) Directory.CreateDirectory(errorFolder);

                string errorFile = string.Format("{0}{1}{2}.log",
                    errorFolder,
                    Path.DirectorySeparatorChar,
                    DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss")
                    );

                System.IO.StreamWriter file = new System.IO.StreamWriter(errorFile, true, System.Text.Encoding.ASCII);
                file.WriteLine(e.Exception.ToString());
                file.Close();
                MessageBox.Show("Program wykonał nieprawidłową operację.\nProszę zgłosić problem.");
            }
            catch
            {
                MessageBox.Show(e.Exception.ToString());
            }
            e.Handled = true;
        }
    }

    
}
