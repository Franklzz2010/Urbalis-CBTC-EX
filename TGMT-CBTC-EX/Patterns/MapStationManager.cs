using BveTypes.ClassWrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGMTAts.OBCU
{
    public static class MapStationManager
    {
        public class Station
        {
            public int StopPosition = (int)Config.LessInf;
            public int RouteOpenTime = 0;
            public int DepartureTime = 0;
            public int DoorOpenType = 0;
            public int StationNumber = 0;
            //增加功能 跳停
            public bool Pass = false;
            //增加/更改开门类型 3为左右同时开启 4为先释放左门 5为先释放右门
            public bool OpenLeftDoors { get { return DoorOpenType == 1 || DoorOpenType == 3 || DoorOpenType == 4 || DoorOpenType == 5; } }
            public bool OpenRightDoors { get { return DoorOpenType == 2 || DoorOpenType == 3 || DoorOpenType == 4 || DoorOpenType == 5; } }
        }


        public static Station NextStation = new Station();

        public static bool Stopped;

        public static bool Arrived;

        public static void Update(AtsEx.PluginHost.Native.VehicleState state, bool doorState)
        {
            switch (TGMTAts.mapPlugin.doorSide)
            {
                case 1:
                    NextStation.DoorOpenType = 2;
                    break;
                case -1:
                    NextStation.DoorOpenType = 1;
                    break;
            }

            NextStation.RouteOpenTime = NextStation.DepartureTime = TGMTAts.mapPlugin.depTime;

            NextStation.Pass = TGMTAts.mapPlugin.isPass;

            NextStation.StopPosition = TGMTAts.mapPlugin.stopPos;


            if (state.Speed == 0 && state.Location > NextStation.StopPosition - Config.StationStartDistance)
            {
                if (!Stopped) TGMTAts.Log("已在站内停稳");
                if (Stopped == false)
                {
                    TGMTAts.panel_[202] = TimeFormatter.MiliSecondToInt(state.Time.TotalMilliseconds);
                }
                Stopped = true;
            }
            if (doorState)
            {
                if (!Arrived) TGMTAts.Log("已开门");
                Arrived = true;
            }
            if (state.Location > NextStation.StopPosition + Config.StationEndDistance)
            {
                NextStation = new Station();
                Stopped = false;
                Arrived = false;
                TGMTAts.Log("已出站");
            }

        }

        public static SpeedLimit RecommendCurve()
        {
            if (TGMTAts.signalMode > 1 && NextStation.Pass)
            {
                return SpeedLimit.inf;
            }
            else
            {
                if (NextStation.StopPosition >= (int)Config.LessInf)
                {
                    return SpeedLimit.inf;
                }
                else if (Arrived)
                {
                    return SpeedLimit.inf;
                }
                else if (Stopped)
                {
                    return new SpeedLimit(0, 0);
                }
                else
                {
                    return new SpeedLimit(0, NextStation.StopPosition);
                }
            }
        }

        public static SpeedLimit CTCEndpoint()
        {
            if (TGMTAts.time > NextStation.RouteOpenTime)
            {
                return SpeedLimit.inf;
            }
            else
            {
                return new SpeedLimit(0, NextStation.StopPosition + Config.StationMotionEndpoint);
            }
        }
    }
}
