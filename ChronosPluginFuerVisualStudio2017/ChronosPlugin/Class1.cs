using AxelSemrau.Chronos.Plugin;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using IWin32Window = System.Windows.Forms.IWin32Window;

namespace ChronosPlugin
{
    public class Class1 : INeedToRunSampleLists
    {
        string pathFolder = @"c:\Users\Public\Folder For Chronos\ExchangeFolder\";

        string IWorkWithSampleLists.ButtonCaption => "Start Listening Mode ";

        System.Drawing.Icon IWorkWithSampleLists.ButtonIcon => Icon.ExtractAssociatedIcon(@"C:\Users\Public\Plugin For Chronos\ChronosPluginFuerVisualStudio2017\icon.ico");

        

        void IWorkWithSampleLists.DoYourJob()
        {
            // Use the process' main window as owner, so that our blocking
            // window can not be hidden behind the main window.
            //writeToFile("C:\\Users\\Public\\Folder For Chronos\\TestVorher.txt", "test vorher");
            clearExchangeFolder();
            watchFolder();
            
            
            
            //startLooking();
            //writeToFile("C:\\Users\\Public\\Folder For Chronos\\TestNachher.txt", "test nachher");
           
        }

        //delete every file within the exchange folder directly after starting the plugin
        //therefore the plugin must be started before the the experiment editor!
        private void clearExchangeFolder()
        {
            string[] files = new string[] { };
            files = Directory.GetFiles(pathFolder, "*.txt");
            foreach (string file in files)
            {
                File.Delete(file);
            }
        }

        private void processSampleList(string samplelist)
        {
            try
            {
                RunSampleListEventArgs arg = new AxelSemrau.Chronos.Plugin.RunSampleListEventArgs()
                {
                    SampleListFile = samplelist,
                    ExtendLastPlanner = true,
                    StartAndWaitForEnd = true,
                    SwitchToSchedulesView = false
                };
                System.Threading.Thread.Sleep(100);
                var ex = RunSampleList(this, arg);
                if (ex != null)
                {
                    writeToFile(@"c:\Users\Public\Folder For Chronos\ErrorMessageSamplelist.txt", "Error: " + ex.Message);
                }
                System.Threading.Thread.Sleep(100);
            }
            catch (Exception e)
            {
                writeToFile("C:\\Users\\Public\\Folder For Chronos\\ErrorMessage.txt", e.Message);

            }
        }

        private void watchFolder()
        {
            writeToFile("C:\\Users\\Public\\Folder For Chronos\\watchFolder.txt", "watch folder");
            int counter = 0;
            while (true)
            {
                string[] files = new string[] { };
                writeToFile("Durchlauf: " + counter.ToString());
                //Wait 2 seconds between each check
                System.Threading.Thread.Sleep(100);
                try
                {
                    files = Directory.GetFiles(pathFolder, "*.txt");
                    if (files.Length == 1 && files[0].Contains("pathToSampleList")) 
                    {
                        //split samplelist into lines and execute each sample list
                        string sampleList = System.IO.File.ReadAllText(files[0]);
                        string[] sampleLists = sampleList.Split('\n');
                        for (int j=0;j < sampleLists.Length; j++)
                        {
                            writeToFile("C:\\Users\\Public\\Folder For Chronos\\test.txt",sampleLists[j]);
                        }
                        for (int j=0; j < sampleLists.Length; j++)
                        {
                            writeToFile(sampleLists[j]);
                            processSampleList(sampleLists[j]);
                        }
                        //processSampleList(sampleList);
                        System.Threading.Thread.Sleep(100);
                        File.Delete(files[0]);
                        writeToFile(pathFolder + "SampleListProcessed.txt", "done");
                    }
                    
                }
                catch (System.ArgumentException ae)
                {
                    writeToFile(pathFolder + "SampleListProcessed.txt", "error");
                }
                
                counter += 1;
            }
        }

        public void writeToFile(string text)
        {
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"C:\Users\Public\Folder For Chronos\Testoutut.txt", true))
            {
                file.WriteLine(text);
            }
        }

        public void writeToFile(string fname, string text)
        {
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(fname, true))
            {
                file.WriteLine(text);
            }
        }

     
        public event AxelSemrau.Chronos.Plugin.RunSampleListHandler RunSampleList;

        private AxelSemrau.Chronos.Plugin.ISampleListAccessor mSampleList;
  
        public AxelSemrau.Chronos.Plugin.ISampleListAccessor SampleList
        {
            set { mSampleList = value; }
        }
    }
}
