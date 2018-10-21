using Ghostscript.NET;
using Ghostscript.NET.Processor;
using Ghostscript.NET.Rasterizer;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace PDF2Image
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string sourcefolder = "";
        string destfolder = "";
    
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GC.Collect();

            if (sourcefolder == "" || destfolder == "")
            {
                MessageBox.Show("Ustal folder źródłowy i docelowy.\nSet source and destination folder");
            }
            else
            {
                this.IsEnabled = false;
                this.Height += 20;

                string[] files = Directory.GetFiles(this.sourcefolder, "*.pdf");

                int couter = 0;
                foreach (string file in files)
                {
                    progress.Value = couter * 100 / files.Length;
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
                    GhostscriptVersionInfo gvi = new GhostscriptVersionInfo("gsdll32.dll");

                    using (GhostscriptProcessor proc = new GhostscriptProcessor(gvi))
                    {
                        using (GhostscriptRasterizer rasterizer = new GhostscriptRasterizer())
                        {
                            using (FileStream stream = File.Open(file, FileMode.Open))
                            {
                                rasterizer.Open(stream, gvi, true);

                                if (rasterizer.PageCount == 1)
                                {
                                    Image img = rasterizer.GetPage((int)this.cbDPI.SelectedItem, (int)this.cbDPI.SelectedItem, 1);

                                    img.Save(string.Concat(this.destfolder, Path.DirectorySeparatorChar, Path.GetFileNameWithoutExtension(file), ".", this.cbFormat.Text.ToLower()),
                                        (ImageFormat)this.cbFormat.SelectedItem);
                                    img.Dispose();
                                }
                                else
                                {
                                    for (var pageNumber = 1; pageNumber <= rasterizer.PageCount; pageNumber++)
                                    {
                                        Image img = rasterizer.GetPage((int)this.cbDPI.SelectedItem, (int)this.cbDPI.SelectedItem, pageNumber);
                                        img.Save(string.Concat(this.destfolder, Path.DirectorySeparatorChar, Path.GetFileNameWithoutExtension(file), "_", pageNumber, ".", this.cbFormat.Text.ToLower()),
                                                (ImageFormat)this.cbFormat.SelectedItem);
                                        img.Dispose();
                                    }
                                }

                            }
                        }
                        GC.Collect();
                    }

                    couter++;
                }

                this.Height -= 20;
                this.IsEnabled = true;

                MessageBox.Show("Konwersja zakończona.\nConversion completed");
                
            }
        }

        private void BSourceFolder_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                this.sourcefolder = dialog.SelectedPath;
                this.lbSourceFolder.Content = this.AdaptStringL(this.sourcefolder);
                this.lbSourceFolder.ToolTip = this.sourcefolder;
            }
        }

        private void BDestFolder_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                this.destfolder = dialog.SelectedPath;
                this.lbDestFolder.Content = this.AdaptStringL(this.destfolder);
                this.lbDestFolder.ToolTip = this.destfolder;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.cbFormat.Items.Add(ImageFormat.Bmp);
            this.cbFormat.Items.Add(ImageFormat.Jpeg);
            this.cbFormat.Items.Add(ImageFormat.Png);
            this.cbFormat.SelectedIndex = 2;

            this.cbDPI.Items.Add(150);
            this.cbDPI.Items.Add(300);
            this.cbDPI.Items.Add(600);
            this.cbDPI.Items.Add(1200);
            this.cbDPI.SelectedIndex = 2;

        }

        private string AdaptStringL(string str)
        {
            if (str.Length > 20) return str.Substring(0, 20)+"...";
            else return str;
        }

    }
}
