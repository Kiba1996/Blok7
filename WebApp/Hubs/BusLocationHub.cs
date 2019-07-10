using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;
using WebApp.Models;

namespace WebApp.Hubs
{
    [HubName("notificationsBus")]
    public class BusLocationHub : Hub
    {
        private static IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<BusLocationHub>();

      

        private static List<Station> stations = new List<Station>();

        private static Timer timer = new Timer();
        private static int cnt = 0;

        public BusLocationHub()
        {
        }

        public void TimeServerUpdates()
        {
            // if (!timer.Enabled)
            //  {
            if (timer.Interval != 4000)
            {
                timer.Interval = 4000;
              // timer.Start();
                timer.Elapsed += OnTimedEvent;
               
              }
            timer.Enabled = true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            #if DEBUG
                (source as Timer).Enabled = false;
#endif
            if (stations != null)
            {
                if (cnt >= stations.Count)
                {
                    cnt = 0;
                }
                double[] niz = { stations[cnt].Latitude, stations[cnt].Longitude };
                Clients.All.setRealTime(niz);
                cnt++;
            }
            else
            {
                double[] nizz = { 0, 0 };
                //Clients.All.SendAsync("setRealTime", nizz);
                //Clients.All.setRealTime(nizz);
            }
            #if DEBUG
                (source as Timer).Enabled = true;
            #endif
        
       // GetTime();
        }

        public void GetTime()
        {
            if (stations.Count > 0)
            {
                if (cnt >= stations.Count)
                {
                    cnt = 0;
                }
                double[] niz = { stations[cnt].Latitude, stations[cnt].Longitude };
               // Clients.All.setRealTime(niz);
                cnt++;
            }
        }

        public void StopTimeServerUpdates()
        {
            timer.Stop();
            stations = null;
        }

        public void AddStations(List<Station> stationsBM)
        {
            stations = new List<Station>();
            stations = stationsBM;
        }
    }
}