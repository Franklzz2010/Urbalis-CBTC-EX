﻿using System;
using System.Reflection;

using BveTypes.ClassWrappers;
using AtsEx.Extensions.SignalPatch;
using AtsEx.PluginHost;
using AtsEx.PluginHost.Plugins;
using AtsEx.Extensions.PreTrainPatch;
using System.IO;
using System.Collections.Generic;
using AtsEx.PluginHost.MapStatements;



namespace TGMTAts.WCU {
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

        private List<SignalPatch> SignalPatch = new List<SignalPatch>();
        private Train Train;
        private PreTrainPatch PreTrainPatch;
        private SectionManager sectionManager;
        private Station Station;


        public PluginMain(PluginBuilder builder) : base(builder) {
            Plugins.AllPluginsLoaded += OnAllPluginsLoaded;
            BveHacker.ScenarioCreated += OnScenarioCreated;
        }

        private void OnAllPluginsLoaded(object sender, EventArgs e) {
            MovementAuthority = OBCULevel = 0;
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
            doorSide = nextSta.DoorSide;
            depTime = nextSta.DepertureTimeMilliseconds;
            isPass = nextSta.Pass;
            stopPos = Convert.ToInt32(nextSta.Location);



            return new MapPluginTickResult();
        }


    }
}
