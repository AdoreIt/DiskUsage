using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using System.Management;
//using System.Management.Instrumentation;
//using System.Collections.Specialized;
using System.Threading;
using System.Diagnostics;
using System.Windows;

namespace DiskUsage
{
    public partial class DiskUsage : Form
    {

        NotifyIcon diskUsageIcon;
        Icon lightUsageIcon;
        Icon mediumUsageIcon;
        Icon hardUsageIcon;
        Icon hundredPercentUsageIcon;

        Thread diskUsageChekerThread;

        public DiskUsage()
        {

            lightUsageIcon = new Icon("light.ico");
            mediumUsageIcon = new Icon("medium.ico");
            hardUsageIcon = new Icon("full.ico");
            hundredPercentUsageIcon = new Icon("hundredpercent.ico");

            diskUsageIcon = new NotifyIcon();
            diskUsageIcon.Icon = lightUsageIcon;
            diskUsageIcon.Visible = true;

            MenuItem closeItem = new MenuItem("Close");
            MenuItem taskBarItem = new MenuItem("Open Task Meneger");

            ContextMenu contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add(taskBarItem);
            contextMenu.MenuItems.Add(closeItem);            
            diskUsageIcon.ContextMenu = contextMenu;


            InitializeComponent();
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;


            closeItem.Click += CloseItem_Click;
            taskBarItem.Click += TaskBarItem_Click;

            diskUsageChekerThread = new Thread(new ThreadStart(DiskUsageCheker));
            diskUsageChekerThread.Start();

        }

        private void TaskBarItem_Click(object sender, EventArgs e)
        {
            Process.Start("taskmgr");
        }

        private void CloseItem_Click(object sender, EventArgs e)
        {
            diskUsageChekerThread.Abort();
            diskUsageIcon.Dispose();
            this.Close();
        }

        private void DiskUsageCheker()
        {
            PerformanceCounter diskUsageCounter = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total");
            int percents = 0;
            try
            {
                while (true)
                {
                    percents = Convert.ToInt32(diskUsageCounter.NextValue());


                    if (percents < 30)
                        diskUsageIcon.Icon = lightUsageIcon;
                    else if (percents > 30 && percents < 70)
                        diskUsageIcon.Icon = mediumUsageIcon;
                    else if (percents > 70 && percents < 97)
                        diskUsageIcon.Icon = hardUsageIcon;
                    else
                        diskUsageIcon.Icon = hundredPercentUsageIcon;
                    Thread.Sleep(1500);

                }
            }
            catch(ThreadAbortException)
            {
                diskUsageCounter.Dispose();
            }
        }


        //private void DiskUsageCheker
    }
}
