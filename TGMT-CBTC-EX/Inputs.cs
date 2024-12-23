using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using AtsEx.PluginHost.Plugins;
using SlimDX.XInput;

namespace TGMTAts.OBCU{
    public partial class TGMTAts : AssemblyPluginBase {

        private static bool a1Down, b1Down;

        private void OnA1Pressed(object sender, EventArgs e) {
            a1Down = true;
            if (a1Down && b1Down && Ato.IsAvailable()) {
                driveMode = 2;
            }
        }


        private void OnB1Pressed(object sender, EventArgs e) {
            b1Down = true;
            if (a1Down && b1Down && Ato.IsAvailable()) { 
                driveMode = 2;
            }
        }
        private void OnB2Pressed(object sender, EventArgs e) {
            switch (ackMessage) {
                case 2:
                    // 释放速度
                    if (!releaseSpeed) Log("释放速度");
                    releaseSpeed = true;
                    break;
                case 6:
                    // 切换到RM
                    ebState = 0;
                    signalMode = 0;
                    FixIncompatibleModes();
                    break;
            }
        }
        private void OnLPressed(object sender, EventArgs e) 
        {
            selectModeStartTime = time;
        }

        private void OnKPressed(object sender, EventArgs e)
        {
            selectModeStartTime = time;
        }
        private void OnA1Up(object sender, EventArgs e) => a1Down = false;
        private void OnB1Up(object sender, EventArgs e) => b1Down = false;
        private void OnKUp(object sender, EventArgs e)
        {
            if (time - selectModeStartTime > 1000)
            {
                TGMTAts.panel_[50] = 1;
            }
        }
        private void OnLUp(object sender, EventArgs e)
        {
            if (time - selectModeStartTime > 1000)
            {
                signalMode = 0;
                FixIncompatibleModes();
            }
        }

        private void SetBeaconData(AtsEx.PluginHost.Native.BeaconPassedEventArgs e) {
            switch (e.Type) {
                case -16777214:
                    trackLimit.SetBeacon(e);
                    break;
                case 96811:
                    deviceCapability = e.Optional;
                    FixIncompatibleModes();
                    break;
                case 96812:
                    doorMode = e.Optional;
                    break;
                case 96813:
                    signalMode = e.Optional / 10 % 10;
                    selectedMode = e.Optional / 100 % 10;
                    driveMode = 1;
                    FixIncompatibleModes();
                    if(signalMode != 0&& driveMode != 0) {
                        VBCount = FBCount = 1;
                    }
                    break;
                case 96810:
                    trackLimit.SetBeacon(e);
                    break;
                case 96824:
                    TGMTAts.panel_[200] = e.Optional;
                    TGMTAts.Log("下一车站站号：" + TGMTAts.panel_[200].ToString());
                    break;
                case 96825:
                    DestinationNumber = e.Optional;
                    break;
                case 96826:
                    TrainNumber = e.Optional;
                    break;
                case 96828:
                case 96829:
                    PreTrainManager.SetBeacon(e);
                    break;
                case 96801:
                case 96802:
                    // TGMT 主
                    // TGMT 填充
                    if (signalMode == 0) signalMode = 1;
                    FixIncompatibleModes();
                    if (signalMode == 1) {
                        if (e.SignalIndex > 0) {
                            Log("移动授权延伸到 " + e.Optional);
                            // 延伸移动授权终点
                            movementEndpoint = new SpeedLimit(0, e.Optional);
                            releaseSpeed = false;
                        } else {
                            Log("红灯 移动授权终点是 " + location + e.Distance);
                            movementEndpoint = new SpeedLimit(0, location + e.Distance);
                        }
                    }
                    break;
                case 96803:
                    // TGMT 定位
                    signalMode = 2;
                    FixIncompatibleModes();
                    break;
                /*case 96890:
                    TGMTAts.panel_[201] = e.Optional;
                    Log(Convert.ToString(panel_[201]));
                    break;*/
            }
        }

        private void Initialize(AtsEx.PluginHost.Native.StartedEventArgs e) {
            driveMode = 1;
            FixIncompatibleModes();

            movementEndpoint = SpeedLimit.inf;
            nextLimit = null;
            selectingMode = -1;
            selectModeStartTime = 0;
            pluginReady = false;
            reverseStartLocation = Config.LessInf;
            releaseSpeed = false;
            ebState = 0;
            ackMessage = 0;

            DestinationNumber = TrainNumber = 0;

            Ato.ResetCache();
            PreTrainManager.ResetCache();
        }

        public static double time;
        public static double doorOpenTime, doorCloseTime;
        
        
        private void DoorOpen(AtsEx.PluginHost.Native.DoorEventArgs e) {
            doorOpen = true;
            doorOpenTime = time;
        }
        
        
        private void DoorClose(AtsEx.PluginHost.Native.DoorEventArgs e) {
            doorOpen = false;
            doorCloseTime = time;
        }

        
        private void HornBlow(int type){

		}
		
	}
}