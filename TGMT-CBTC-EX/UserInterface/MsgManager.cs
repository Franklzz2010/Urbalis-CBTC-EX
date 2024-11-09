using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtsEx.PluginHost.Native;

namespace UrbalisAts.OBCU
{
    public static class MsgManager
    {
        public class Msg
        { 
            public int MsgID { get; set; }
            public string MsgTime { get; set; }
        }

        public static Msg msg1, msg2, msg3;
        public static int page, maxpage;
        public static bool canUp, canDown;

        private static List<Msg> MsgList = new List<Msg>();

        public static void Initialize(VehicleState state)
        {
            MsgList.Clear();
            page = 0;

            SetMsg(12, state, true);
            SetMsg(13, state, true);
            SetMsg(1, state);

        }

        public static void SetMsg(int msgnumber, VehicleState state)
        {

            Msg msg = new Msg();
            msg.MsgTime = TimeFormatter.MiliSecondToShortString(state.Time.TotalMilliseconds);
            msg.MsgID = msgnumber;

            MsgList.Add(msg);
            UpdateMsg();
        }
        private static void SetMsg(int msgnumber, VehicleState state, bool isInit)
        {
            if (isInit)
            {
                Msg msg = new Msg();
                msg.MsgTime = TimeFormatter.MiliSecondToShortString(state.Time.TotalMilliseconds);
                msg.MsgID = msgnumber;

                MsgList.Add(msg);
            }
        }


        public static void UpdateMsg()
        {
            int maxmsgindex = MsgList.Count - 1;
            maxpage = maxmsgindex - 2;
            if (page < maxpage) canUp = true;
            else canUp = false;
            if (page > 0) canDown = true;
            else canDown = false;
            if (maxmsgindex - page - 2 >= 0)
            {
                msg1 = MsgList[maxmsgindex - page];
                msg2 = MsgList[maxmsgindex - page - 1];
                msg3 = MsgList[maxmsgindex - page - 2];

            }
            else
            {
                msg1 = MsgList[2];
                msg2 = MsgList[1];
                msg3 = MsgList[0];
            }
        }
    }
}
