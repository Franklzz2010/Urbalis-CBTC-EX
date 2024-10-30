using System;
using System.Reflection;

using BveTypes.ClassWrappers;
using AtsEx.Extensions.SignalPatch;
using AtsEx.PluginHost;
using AtsEx.PluginHost.Plugins;
using AtsEx.Extensions.PreTrainPatch;
using System.IO;
using System.Collections.Generic;
using AtsEx.PluginHost.MapStatements;
using AtsEx.Extensions.ContextMenuHacker;
using System.Linq;



namespace UrbalisAts.WCU {
    [PluginType(PluginType.MapPlugin)]
    public class PluginMain : AssemblyPluginBase {
        static PluginMain() {
            Config.Load(Path.Combine(Config.PluginDir, "TGMT_WCUConfig.txt"));
        }

        public double MovementAuthority { get; set; } = 0;
        public int OBCULevel { get; set; } = 0;
        public double SelfTrainLocation { get; set; } = 0;
        public int doorSide { get; set; } = 0;
        public int depTime { get; set; } = 0;
        public bool isPass { get; set; } = false;
        public int stopPos { get; set; } = 0;
        public static List<int> speedLimitList  = new List<int>();
        public static List<int> speedLimitPosList = new List<int>();

        private List<SignalPatch> SignalPatch = new List<SignalPatch>();
        private Train Train;
        private PreTrainPatch PreTrainPatch;
        private SectionManager sectionManager;
        private Station Station;

        public static bool PluginReady;

        public PluginMain(PluginBuilder builder) : base(builder) {
            Plugins.AllPluginsLoaded += OnAllPluginsLoaded;
            BveHacker.ScenarioCreated += OnScenarioCreated;
        }

        private void OnAllPluginsLoaded(object sender, EventArgs e) {
            MovementAuthority = OBCULevel = 0;
            PluginReady = false;
        }


        private void OnScenarioCreated(ScenarioCreatedEventArgs e) {
            if (!e.Scenario.Trains.ContainsKey(Config.PretrainName)) {
                throw new BveFileLoadException(string.Format("找不到具有{0}密钥的其他列车。", Config.PretrainName), "TGMT-CBTC-EX_WCU");
            }

            Train = e.Scenario.Trains[Config.PretrainName];

            sectionManager = e.Scenario.SectionManager;
            PreTrainPatch = Extensions.GetExtension<IPreTrainPatchFactory>().Patch(nameof(PreTrainPatch), sectionManager, new PreTrainLocationConverter(Train, sectionManager));
            int pointer = 0;
            while (pointer < sectionManager.Sections.Count - 1) {
                SignalPatch.Add(Extensions.GetExtension<ISignalPatchFactory>().Patch(nameof(SignalPatch), sectionManager.Sections[pointer] as Section,
                    source => (sectionManager.Sections[pointer].Location >= SelfTrainLocation && sectionManager.Sections[pointer].Location >= Config.TGMTTerrtoryStart
                    && sectionManager.Sections[pointer].Location < Config.TGMTTerrtoryEnd) ? (OBCULevel == 2) ? (int)Config.CTCSignalIndex : source : source));
                ++pointer;
            }


            PluginReady = false;
                
        }

        public override void Dispose() {
            for (int i = 0; i < SignalPatch.Count; ++i) SignalPatch[i]?.Dispose();
            PreTrainPatch?.Dispose();
            Plugins.AllPluginsLoaded -= OnAllPluginsLoaded;
            BveHacker.ScenarioCreated -= OnScenarioCreated;
        }

        private class PreTrainLocationConverter : IPreTrainLocationConverter {
            private readonly Train SourceTrain;
            private readonly SectionManager SectionManager;

            public PreTrainLocationConverter(Train sourceTrain, SectionManager sectionManager) {
                SourceTrain = sourceTrain;
                SectionManager = sectionManager;
            }

            public PreTrainLocation Convert(PreTrainLocation source)
                => SourceTrain.TrainInfo.TrackKey == Config.PretrainTrackkey ? PreTrainLocation.FromLocation(SourceTrain.Location, SectionManager) : source;
        }

        public override TickResult Tick(TimeSpan elapsed) {

            MovementAuthority = Train.Location;

            
            var nextSta = BveHacker.Scenario.Route.Stations[BveHacker.Scenario.Route.Stations.CurrentIndex + 1] as Station;
            if (nextSta.DoorSide != 0 && !nextSta.Pass || (nextSta.DoorSide == 0 && nextSta.Pass))
            {
                doorSide = nextSta.DoorSide;
                isPass = nextSta.Pass;
                if (isPass) depTime = 0;
                else depTime = nextSta.DepartureTimeMilliseconds;

                stopPos = Convert.ToInt32(nextSta.Location);
            }
                


            var trackInfo = BveHacker.Scenario.Route.SpeedLimits.Src;


            if (!PluginReady)
            {
                for (int i = 0; i < trackInfo.Count; i++)
                {
                    int speed;
                    var fieldType = trackInfo[i].GetType();
                    FieldInfo speedInfo = fieldType.GetField("a", BindingFlags.NonPublic | BindingFlags.Instance);
                    var speedvalue = speedInfo.GetValue(trackInfo[i]);


                    if (speedvalue * 3.6 > 95) speed = 95;
                    else speed = Convert.ToInt32(speedvalue * 3.6);


                    FieldInfo[] locInfo = fieldType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                    var locValue = locInfo[1].GetValue(trackInfo[i]);

                    int distance = Convert.ToInt32(locValue);

                    if (speedLimitList.Count == 0 || speed != speedLimitList.Last())
                    {
                        speedLimitList.Add(speed);
                        speedLimitPosList.Add(distance);

                    }
                }

                PluginReady = true;
            }


            return new MapPluginTickResult();
        }


    }
}
