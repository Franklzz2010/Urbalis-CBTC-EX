using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Windows.Forms;

namespace UrbalisAts.OBCU {
    public partial class DebugWindow : Form {

        public DebugWindow() {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void timer1_Tick(object sender, EventArgs e) {
            if (!UrbalisAts.pluginReady) return;
            var sb = new StringBuilder();
            sb.AppendLine("UrbalisAts Beta for BVE5/6 1.2 by zbx1425 2021-7-12 https://www.zbx1425.cn");
            sb.AppendFormat("车位     : {0}\n", D(UrbalisAts.location));
            if (UrbalisAts.nextLimit == null) {
                sb.AppendLine("下一限速 : 无");
            } else {
                sb.AppendFormat("下一限速 : {0,6} {1,-4}\n", D(UrbalisAts.nextLimit.Location), D(UrbalisAts.nextLimit.Limit));
            }
            sb.AppendLine();
            sb.AppendFormat("授权终点 : {0,6}\n", D(UrbalisAts.movementEndpoint.Location));
            var pretrain = PreTrainManager.GetEndpoint();
            sb.AppendFormat("前车信息 : {0,6} {1,-4}\n", D(pretrain.Location), D(pretrain.Limit));
            sb.AppendFormat("车站     : {0,6} {1} {2}\n",
                D(MapStationManager.NextStation.StopPosition),
                MapStationManager.Arrived ? "站内停止" : "",
                MapStationManager.Arrived ? "已到达" : ""
            );
            sb.AppendFormat("线路限速 : {0,6} {1,-4} -> [{2,6} {3,-4}] -> {4,6} {5,-4}\n",
                D(UrbalisAts.trackLimit.last.Location), D(UrbalisAts.trackLimit.last.Limit),
                D(UrbalisAts.trackLimit.current.Location), D(UrbalisAts.trackLimit.current.Limit),
                D(UrbalisAts.trackLimit.next.Location), D(UrbalisAts.trackLimit.next.Limit)
            );
            foreach (var limit in UrbalisAts.trackLimit.trackLimits) {
                sb.AppendFormat("{0,6}: {1,-2} ;", D(limit.Location), D(limit.Limit));
            }
            sb.AppendLine();
            sb.AppendFormat("ATO 级位 : {0}\n", Ato.outputNotch);
            sb.AppendLine();

            foreach (var msg in UrbalisAts.debugMessages.Skip(Math.Max(0, UrbalisAts.debugMessages.Count() - 8))) {
                sb.AppendLine(msg);
            }
            //sb.AppendFormat("Signal  : {0,6} {1,-4}\n", D(UrbalisAts.signalLimit.Location), D(UrbalisAts.signalLimit.Limit));
            //sb.AppendFormat(" -Raw5  : {0,3}", UrbalisAts.signalLimit.updateCount);
            /*foreach (var sd in UrbalisAts.signalLimit.sgdata.Take(5)) {
                sb.AppendFormat("{0,6}: {1} ", D(sd.Distance), D(sd.Aspect));
            }*/
            sb.AppendLine();
            label1.Text = sb.ToString();
        }

        private string D(double d) {
            if (d <= -Config.LessInf) return "-Inf";
            if (d >= Config.LessInf) return "Inf";
            return ((int)d).ToString();
        }

        private string D(int d) {
            if (d <= -Config.LessInf) return "-Inf";
            if (d >= Config.LessInf) return "Inf";
            return d.ToString();
        }
    }
}
