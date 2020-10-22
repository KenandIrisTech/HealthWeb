using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Syncfusion.EJ2.Charts;

namespace HealthWeb.Controllers
{
    public class MemberController : Controller
    {
        // GET: Member
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult VitalSigns()
        {

            GetVitalSignsData();
            return View();
        }

        public ActionResult Evaluation(string id)
        {
            id = id ?? "1";
            ViewBag.PageId = id;
            switch (id)
            {
                case "1":
                    GetVitalSignsData();
                    break;
                case "2":
                    GetTargetData();
                    break;
                case "3":
                    GetPolarRadarAreaData();
                    break;
            }

            return View();
        }

        private void GetVitalSignsData()
        {
            ViewBag.TemperatureList = new List<object>()
            {
                new {Area="大安診所", MeasurementTime = new DateTime(2020,02,18,14,20,0), Value=36.5 },
                new {Area="不明", MeasurementTime = new DateTime(2020,02,20,9,20,0), Value=36.7 },
            };

            ViewBag.BodyPressureList = new List<object>()
            {
                new {Area="大安診所", MeasurementTime = new DateTime(2020,02,18,14,20,0), Pulse=62, LP=90, HP=140 },
                new {Area="不明", MeasurementTime = new DateTime(2020,02,20,9,20,0), Pulse=65, LP=99, HP=150 },
                new {Area="不明", MeasurementTime = new DateTime(2020,02,20,9,20,0), Pulse=65, LP=99, HP=150 },
                new {Area="健康動中心", MeasurementTime = new DateTime(2020,03,2,9,20,0), Pulse=65, LP=99, HP=150 },
                new {Area="健康動中心", MeasurementTime = new DateTime(2020,3,6,9,20,0), Pulse=65, LP=99, HP=150 },
                new {Area="健康動中心", MeasurementTime = new DateTime(2020,4,15,9,20,0), Pulse=65, LP=99, HP=150 },
            };

            ViewBag.BodySugarList = new List<object>()
            {
                new {Area="健康動中心", MeasurementTime = new DateTime(2020,02,18,11,20,0), MeasurementType="飯前", BS=62},
                new {Area="健康動中心", MeasurementTime = new DateTime(2020,02,18,13,50,0), MeasurementType="飯後", BS=102},

                new {Area="大安診所", MeasurementTime = new DateTime(2020,02,23,6,40,0), MeasurementType="飯前", BS=97},
                new {Area="大安診所", MeasurementTime = new DateTime(2020,02,23,7,30,0), MeasurementType="飯後", BS=122},

                new {Area="老人活動中心", MeasurementTime = new DateTime(2020,02,18,14,20,0), MeasurementType="飯前", BS=98},
                new {Area="老人活動中心", MeasurementTime = new DateTime(2020,02,18,14,20,0), MeasurementType="飯後", BS=162},

                new {Area="老人活動中心", MeasurementTime = new DateTime(2020,02,18,14,20,0), MeasurementType="飯前", BS=104},
                new {Area="老人活動中心", MeasurementTime = new DateTime(2020,02,18,14,20,0), MeasurementType="飯後", BS=107},

            };

            ViewBag.SportList = new List<object>()
            {
                new {Area="健康動中心", MeasurementTime = new DateTime(2020,02,18,11,20,0), TotalDuration=769, CAL = 42, DIST = 3.2, AvgHeart=10},
                new {Area="健康動中心", MeasurementTime = new DateTime(2020,02,18,11,20,0), TotalDuration=756, CAL = 196, DIST = 1.8, AvgHeart=12},
                new {Area="健康動中心", MeasurementTime = new DateTime(2020,02,18,11,20,0), TotalDuration=442, CAL = 122, DIST = 2.3, AvgHeart=15},
                new {Area="健康動中心", MeasurementTime = new DateTime(2020,02,18,11,20,0), TotalDuration=202, CAL = 16, DIST = 0.8, AvgHeart=13},

            };
        }
        // GET: PolarRadarArea

        private void GetPolarRadarAreaData()
        {
            List<PolarAreaChartData> chartData = new List<PolarAreaChartData>
            {
                new PolarAreaChartData { x= "生理", y= 4 },
                new PolarAreaChartData { x= "心理", y= 3.0 },
                new PolarAreaChartData { x= "運動", y= 3.8 },
                new PolarAreaChartData { x= "社會互動", y= 3.4 },
            };
            ViewBag.dataSource = chartData;
            List<PolarAreaChartData> chartData1 = new List<PolarAreaChartData>
            {
                new PolarAreaChartData { x= "生理", y= 2.6 },
                new PolarAreaChartData { x= "心理", y= 2.8 },
                new PolarAreaChartData { x= "運動", y= 2.6 },
                new PolarAreaChartData { x= "社會互動", y= 3 },
            };
            ViewBag.dataSource1 = chartData1;
            List<PolarAreaChartData> chartData2 = new List<PolarAreaChartData>
            {
                new PolarAreaChartData { x= "生理", y= 2.8 },
                new PolarAreaChartData { x= "心理", y= 2.5 },
                new PolarAreaChartData { x= "運動", y= 2.8 },
                new PolarAreaChartData { x= "社會互動", y= 3.2 },
            };
            ViewBag.dataSource2 = chartData2;
            ViewBag.data = new List<object>()
            {
                new { text="極座標餅圖", value="Polar"},
                new { text="雷達圖", value="Radar"},
            };
                //new string[] { "Polar", "Radar" };
        }

        private void GetTargetData()
        {
            ViewBag.StepsTarget = new TargetManagement{ Target = 10000, Current= 1201 };
            ViewBag.WeightsTarget = new TargetManagement{ Target = 90, Current= 98 };
            ViewBag.SleepingTarget = new TargetManagement{ Target = 8, Current= 6.2 };
        }
    }

    public class PolarAreaChartData
    {
        public string x;
        public double y;
    }

    public class TargetManagement
    {
        public double Target { get; set; }
        public double Current { get; set; }
        public double Ratio
        {
            get { return Math.Round(Current / Target * 100,2); }
        }

        
    }
}