using JBNClassLibrary;
using JBNWebAPI.Logger;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;

namespace AdService
{
    public partial class AdsService : ServiceBase
    {
        public static Helper helper = new Helper();
        Timer timer = new Timer();
        public AdsService()
        {
            InitializeComponent();
            helper.SetTimer.Interval = 5000;
            helper.SetTimer.AutoReset = true;
            helper.SetTimer.Enabled = true;
            helper.SetTimer.Elapsed += new System.Timers.ElapsedEventHandler(SetTimer_Elapsed);
        }

        protected override void OnStart(string[] args)
        {
            helper.SetTimer.Start();
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
        }

        protected override void OnStop()
        {
            helper.SetTimer.Enabled = false;
            helper.SetTimer.Stop();
        }

        private void SetTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            RunAdService();
        }
        private void OnElapsedTime(object sender, ElapsedEventArgs e)
        {
            RunAdService();
        }
        private void RunAdService()
        {
            DLAdvertisements dLAdvertisements = new DLAdvertisements();
            dLAdvertisements.SendNotifications();
        }
    }
}
