using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using System.Reflection;
using AtsEx.PluginHost.Plugins;
using Zbx1425.DXDynamicTexture;
using AtsEx.PluginHost;
using AtsEx.PluginHost.Input.Native;
using SlimDX.DirectInput;
using System.Security.Cryptography;
using System.Text;
using UrbalisAts.OBCU.UserInterface;
using AtsEx.Extensions.ConductorPatch;
using BveTypes.ClassWrappers;

namespace UrbalisAts.OBCU {
    public partial class UrbalisAts : AssemblyPluginBase {

        public static int[] panel_ = new int[256];
        public static bool doorOpen;
        public static AtsEx.PluginHost.Native.VehicleSpec vehicleSpec;
        public static double location = -114514;

        public static int nextStationNumber;

        public static List<string> debugMessages = new List<string>();

        //Urbalis只有AM-BM和AM-CBTC两个预选模式
        // 0: RM; 1: CM-ITC; 2: CM-CBTC; 3: AM-BM; 4: AM-CBTC; 5: XAM
        public static int selectedMode = 4;
        // 0: RM; 1: CM; 2: AM; 3: XAM
        public static int driveMode = 1;
        // 0: IXL; 1: ITC; 2: CTC
        public static int signalMode = 2;
        // 1: MM; 2: AM; 3: AA
        public static int doorMode = 1;
        // 0: 没有CTC,ITC; 1: 没有CTC; 2: 正常
        public static int deviceCapability = 2;

        // 暂时的预选速度，-1表示没有在预选
        public static int selectingMode = -1;
        public static double selectModeStartTime = 0;

        //首次启动需将HMI上msg显示的时间校准
        public static bool initTimeMode = true;

        public static int ebState = 0;
        public static bool releaseSpeed = false;
        public static int ackMessage = 0;

        public static bool releaseSpeedInop = false;

        public static int TrainNumber = 0;
        public static int DestinationNumber = 0;

        public static int VBCount = 0;
        public static int FBCount = 0;

        public static double reverseStartLocation = Config.LessInf;

        public static TrackLimit trackLimit = new TrackLimit();

        public static Form debugWindow;
        public static bool pluginReady = false;

        public static HarmonyLib.Harmony harmony;

        public static TextureHandle hTDTTex;
        public static TouchTextureHandle hHMITex;
        public static TextureHandle hHMI2Tex;

        public static Msg msg1 = new Msg();
        public static Msg msg2 = new Msg();
        public static Msg msg3 = new Msg();

        public static Conductor conductor = new Conductor();


        static UrbalisAts() {
            Config.Load(Path.Combine(Config.PluginDir, "TGMTConfig.txt"));
            //AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        const string ExpectedHash = "9758E6EA853B042ED49582081371764F43BC8E4DC7955C2B6D949015B984C8E2";

        private void Load() {
            if (Config.Debug) {
                new Thread(() => {
                    debugWindow = new DebugWindow();
                    Application.Run(debugWindow);
                }).Start();
            }
            try {
                //TextureManager.Initialize();
                TGMTPainter.Initialize();
                hHMITex = TouchManager.Register(Config.HMIImageSuffix, 1024, 1024);
                TouchManager.EnableEvent(MouseButtons.Left, TouchManager.EventType.Down);
                hHMITex.SetClickableArea(0, 0, 800, 600);
                hHMITex.MouseDown += HMITouch.OnHMITexMouseDown;
                hTDTTex = TextureManager.Register(Config.TDTImageSuffix, 256, 256);
                hHMI2Tex = TextureManager.Register(Config.HMI2ImageSuffix, 1024, 1024);
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }

            TGMTPainter.voltage = 1350;
        }


        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args) {
            if (args.Name.Contains("Harmony")) {
                return Assembly.LoadFile(Config.HarmonyPath);
            }
            return null;
        }

        public static void FixIncompatibleModes() {
            if (selectedMode == 0) signalMode = 0; // 预选了IXL
            if (selectedMode == 1 && signalMode > 1) signalMode = 1; // 预选了ITC
            if (selectedMode == 3 && signalMode > 1) signalMode = 1; // 预选了ITC

            if (deviceCapability == 0) signalMode = 0; // 没有TGMT设备
            if (deviceCapability == 1 && signalMode > 1) signalMode = 1; // 没有无线电信号

            if (signalMode > 0 && driveMode == 0) driveMode = 1; // 有信号就至少是SM
            if (signalMode == 0 && driveMode > 0) driveMode = 0; // 没信号就得是RM
        }

        public static int ConvertTime(int human) {
            var hrs = human / 10000;
            var min = human / 100 % 100;
            var sec = human % 100;
            return hrs * 3600 + min * 60 + sec;
        }

        public static void SetSignal(int signal) {

        }
        public override void Dispose() {
            if (debugWindow != null) debugWindow.Close();
            TGMTPainter.Dispose();
            hHMITex.Dispose();
            hTDTTex.Dispose();
            hHMI2Tex.Dispose();

            Native.NativeKeys.AtsKeys[NativeAtsKeyName.A1].Pressed -= OnA1Pressed;
            Native.NativeKeys.AtsKeys[NativeAtsKeyName.B1].Pressed -= OnB1Pressed;
            Native.NativeKeys.AtsKeys[NativeAtsKeyName.B2].Pressed -= OnB2Pressed;
            Native.NativeKeys.AtsKeys[NativeAtsKeyName.K].Pressed -= OnKPressed;
            Native.NativeKeys.AtsKeys[NativeAtsKeyName.L].Pressed -= OnLPressed;
            Native.NativeKeys.AtsKeys[NativeAtsKeyName.A1].Released -= OnA1Up;
            Native.NativeKeys.AtsKeys[NativeAtsKeyName.B1].Released -= OnB1Up;
            Native.NativeKeys.AtsKeys[NativeAtsKeyName.K].Released -= OnKUp;
            Native.NativeKeys.AtsKeys[NativeAtsKeyName.L].Released -= OnLUp;

            Native.BeaconPassed -= SetBeaconData;
            Native.DoorClosed -= DoorClose;
            Native.DoorOpened -= DoorOpen;
            Native.Started -= Initialize;

            Plugins.AllPluginsLoaded -= OnAllPluginsLoaded;
            //TextureManager.Dispose();
        }

        public static void Log(string msg) {
            time /= 1000;
            var hrs = time / 3600 % 60;
            var min = time / 60 % 60;
            var sec = time % 60;
            debugMessages.Add(string.Format("{0:D2}:{1:D2}:{2:D2} {3}", Convert.ToInt32(hrs), Convert.ToInt32(min), Convert.ToInt32(sec), msg));
        }
    }
}