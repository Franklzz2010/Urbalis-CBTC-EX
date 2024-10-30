using SlimDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Zbx1425.DXDynamicTexture;
using System.Collections;
using System.Runtime.InteropServices.ComTypes;
using BveTypes.ClassWrappers;
using AtsEx.PluginHost;
using AtsEx.PluginHost.Handles;
using UrbalisAts.OBCU.UserInterface;

namespace UrbalisAts.OBCU {

    public static class TGMTPainter {

        public static string INIPath = Convert.ToString(Path.Combine(Config.PluginDir, "StationList.ini"));

        public static GDIHelper hHMI, hTDT, hHMI2;

        public static void Initialize() {
            var imgDir = Config.ImageAssetPath;

            hHMI = new GDIHelper(1024, 1024);
            hTDT = new GDIHelper(256, 256);
            hHMI2 = new GDIHelper(1024, 1024);



            hmi = new Bitmap(Path.Combine(imgDir, "hmi.png"));
            ackcmd = new Bitmap(Path.Combine(imgDir, "msg.png"));
            atoctrl = new Bitmap(Path.Combine(imgDir, "atoctrl.png"));
            dormode = new Bitmap(Path.Combine(imgDir, "dormode.png"));
            dorrel = new Bitmap(Path.Combine(imgDir, "dorrel.png"));
            drvmode = new Bitmap(Path.Combine(imgDir, "drvmode.png"));
            emergency = new Bitmap(Path.Combine(imgDir, "emergency.png"));
            fault = new Bitmap(Path.Combine(imgDir, "fault.png"));
            selmode = new Bitmap(Path.Combine(imgDir, "selmode.png"));
            sigmode = new Bitmap(Path.Combine(imgDir, "sigmode.png"));
            special = new Bitmap(Path.Combine(imgDir, "special.png"));
            stopsig = new Bitmap(Path.Combine(imgDir, "stopsig.png"));
            departure = new Bitmap(Path.Combine(imgDir, "departure.png"));
            menu = new Bitmap(Path.Combine(imgDir, "menu.png"));
            hmitdt = new Bitmap(Path.Combine(imgDir, "hmi_tdt.png"));
            life = new Bitmap(Path.Combine(imgDir, "life.png"));
            distance = new Bitmap(Path.Combine(imgDir, "distance.png"));
            msg = new Bitmap(Path.Combine(imgDir, "msg_history.png"));
            rmpanel = new Bitmap(Path.Combine(imgDir, "rmpanel.png"));
            bmconfirm = new Bitmap(Path.Combine(imgDir, "bmconfirm.png"));
            menuext = new Bitmap(Path.Combine(imgDir, "menuext.png"));
            crewnumenter = new Bitmap(Path.Combine(imgDir, "crewnumenter.png"));

            drawFont = new System.Drawing.Font("思源黑体 CN Bold", 30);
            timeFont = new System.Drawing.Font("思源黑体 CN Bold", 15);
            distanceFont = new System.Drawing.Font("思源黑体 CN Bold", 16);
            hmi2Font = new System.Drawing.Font("思源黑体 CN Bold", 10);
            crewNumFont = new System.Drawing.Font("思源黑体 CN Bold", 15);

            num0 = new Bitmap(Path.Combine(imgDir, "num0.png"));
            numn0 = new Bitmap(Path.Combine(imgDir, "num-0.png"));
            colon = new Bitmap(Path.Combine(imgDir, "colon.png"));

            
            tdtbackoff = new Bitmap(Path.Combine(imgDir, "tdt_back_off.png"));
            tdtbackred = new Bitmap(Path.Combine(imgDir, "tdt_back_red.png"));
            tdtbackgreen = new Bitmap(Path.Combine(imgDir, "tdt_back_green.png"));
            tdtdigitsred = Image.FromFile(Path.Combine(imgDir, "tdt_digits_red.png"));
            tdtdigitsgreen = Image.FromFile(Path.Combine(imgDir, "tdt_digits_green.png"));
            //tdtdigitsred =new Bitmap(Path.Combine(imgDir, "tdt_digits_red.png"));
            //tdtdigitsgreen = new Bitmap(Path.Combine(imgDir, "tdt_digits_green.png"));

            hmi2 = new Bitmap(Path.Combine(imgDir, "hmi2.png"));
            dooropenleft = new Bitmap(Path.Combine(imgDir, "dorleft.png"));
            dooropenright = new Bitmap(Path.Combine(imgDir, "dorright.png"));
            trainkey = new Bitmap(Path.Combine(imgDir, "key.png"));
            traindir1 = new Bitmap(Path.Combine(imgDir, "dir1.png"));
            traindir2 = new Bitmap(Path.Combine(imgDir, "dir2.png"));
            hmi2Green = new Bitmap(Path.Combine(imgDir, "hmi2Green.png"));
            hmi2Yellow = new Bitmap(Path.Combine(imgDir, "hmi2Yellow.png"));
            hmi2Red = new Bitmap(Path.Combine(imgDir, "hmi2Red.png"));


        }

        public static void Dispose() {
            hHMI.Dispose();
            hTDT.Dispose();
            hHMI2.Dispose();
        }
        public static int counter = 0;


        public static GDIHelper PaintHMI(AtsEx.PluginHost.Native.VehicleState state) {
            hHMI.BeginGDI();
            hHMI.DrawImage(hmi, 0, 0);

            if (UrbalisAts.driveMode == 0)
            {
                hHMI.DrawImage(rmpanel, 138, 73);
            }

            if (UrbalisAts.panel_[102] != 0 && !UrbalisAts.isDoorClosed)
            {
                if (state.Speed == 0)
                {
                    UrbalisAts.panel_[105] = 1;
                }
                else
                {
                    UrbalisAts.panel_[105] = 0;
                }
            }
            else
            {
                UrbalisAts.panel_[105] = 0;
            }

            if (UrbalisAts.selectingMode == -1 && UrbalisAts.ackMessage == 0)
            {
                hHMI.DrawImage(msg, 115, 480, UrbalisAts.msg1.MsgID * 18, 18);
                hHMI.DrawImage(msg, 115, 501, UrbalisAts.msg2.MsgID * 18, 18);
                hHMI.DrawImage(msg, 115, 522, UrbalisAts.msg3.MsgID * 18, 18);
            }
            


            hHMI.DrawImage(menu, 455, 520, UrbalisAts.panel_[23] * 64, 64);
            hHMI.DrawImage(drvmode, 530, 133, UrbalisAts.panel_[24] * 64, 64);
            hHMI.DrawImage(sigmode, 655, 133, UrbalisAts.panel_[25] * 64, 64);
            hHMI.DrawImage(stopsig, 670, 200, UrbalisAts.panel_[26] * 64, 64);
            hHMI.DrawImage(dorrel, 530, 267, UrbalisAts.panel_[27] * 64, 64);
            hHMI.DrawImage(dormode, 530, 337, UrbalisAts.panel_[28] * 64, 64);
            hHMI.DrawImage(departure, 670, 267, UrbalisAts.panel_[32] * 64, 64);
            hHMI.DrawImage(emergency, 670, 337, UrbalisAts.panel_[29] * 64, 64);
            hHMI.DrawImage(fault, 530, 405, UrbalisAts.panel_[30] * 64, 64);
            hHMI.DrawImage(special, 670, 405, UrbalisAts.panel_[31] * 64, 64);

            if (UrbalisAts.selectingMode != -1 || UrbalisAts.ackMessage != 0)
            {
                hHMI.DrawImage(ackcmd, 55, 475, UrbalisAts.panel_[35] * 70, 70);
            }
                        
            hHMI.DrawImage(atoctrl, 32, 380, UrbalisAts.panel_[21] * 64, 64);
            hHMI.DrawImage(selmode, 130, 380, UrbalisAts.panel_[22] * 64, 64);
            hHMI.DrawImage(hmitdt, 530, 70, UrbalisAts.panel_[105] * 64, 64);
            hHMI.DrawImage(life, 15, 560, UrbalisAts.panel_[107] * 40, 40);

            if (UrbalisAts.panel_[109] == 1)
            {
                hHMI.DrawImage(distance, 10, 144);
            }

            hHMI.DrawImage(num0, 289, 212, D((int)Math.Abs(Math.Ceiling(state.Speed)), 0) * 18, 18);
            hHMI.DrawImage(numn0, 275, 212, D((int)Math.Abs(Math.Ceiling(state.Speed)), 1) * 18, 18);

            if (UrbalisAts.panel_[50] == 1) hHMI.DrawImage(bmconfirm, 555, 400);

            if (UrbalisAts.panel_[61] == 1) hHMI.DrawImage(crewnumenter, 525, 108);

            if (UrbalisAts.panel_[51] == 1) hHMI.DrawImage(menuext, 479, 9);

            hHMI.EndGDI();

            var stringC = new StringFormat();
            stringC.Alignment = StringAlignment.Center;

            //司机号
            if (UrbalisAts.panel_[51] == 0)
            {
                hHMI.Graphics.DrawString(Convert.ToString(UrbalisAts.panel_[63]).PadLeft(3, '0'), crewNumFont, new SolidBrush(Color.FromArgb(0, 0, 0)), 704, 30);
                if (UrbalisAts.panel_[61] == 1 && UrbalisAts.panel_[62] != 0) hHMI.Graphics.DrawString(HMITouch.crewNumberPreEnterStr, drawFont, new SolidBrush(Color.FromArgb(0, 0, 0)), 653, 171, stringC);

            }   


            if (UrbalisAts.selectingMode == -1 && UrbalisAts.ackMessage == 0)
            {
                hHMI.Graphics.DrawString(UrbalisAts.msg1.MsgTime, timeFont, new SolidBrush(Color.FromArgb(199, 199, 198)), 83, 479, stringC);
                hHMI.Graphics.DrawString(UrbalisAts.msg2.MsgTime, timeFont, new SolidBrush(Color.FromArgb(199, 199, 198)), 83, 499, stringC);
                hHMI.Graphics.DrawString(UrbalisAts.msg3.MsgTime, timeFont, new SolidBrush(Color.FromArgb(199, 199, 198)), 83, 519, stringC);
            }

            hHMI.Graphics.FillRectangle(overspeed[UrbalisAts.panel_[10]], new Rectangle(20, 18, 80, 78));
            if (UrbalisAts.panel_[36] != 0 && UrbalisAts.time % 500 < 250) {
                hHMI.Graphics.DrawRectangle(ackPen, new Rectangle(53, 473, 280, 70));
            }
            if (UrbalisAts.panel_[15] >= 0) {
                var tRecommend = ((double)UrbalisAts.panel_[15] / 400 * 288 - 144) / 180 * Math.PI;
                hHMI.Graphics.FillPolygon(Brushes.Yellow, new Point[] {
                    Poc(288, 221, 130, 0, tRecommend), Poc(288, 221, 150, -11, tRecommend), Poc(288, 221, 150, 11, tRecommend)
                });
            }
            if (UrbalisAts.panel_[16] >= 0) {
                var tLimit = ((double)UrbalisAts.panel_[16] / 400 * 288 - 144) / 180 * Math.PI;
                hHMI.Graphics.FillPolygon(Brushes.Red, new Point[] {
                    Poc(288, 221, 130, 0, tLimit), Poc(288, 221, 150, -11, tLimit), Poc(288, 221, 150, 11, tLimit)
                });
            }
            var tSpeed = ((double)UrbalisAts.panel_[1] / 400 * 288 - 144) / 180 * Math.PI;
            if (UrbalisAts.panel_[15] >= 0)
            {
                var tRecommend = ((double)UrbalisAts.panel_[15] / 400 * 288 - 144) / 180 * Math.PI;
                var tLimit = ((double)UrbalisAts.panel_[16] / 400 * 288 - 144) / 180 * Math.PI;
                if (UrbalisAts.panel_[29] == 2)
                {
                    hHMI.Graphics.DrawEllipse(circlePenRed, new Rectangle(255, 188, 66, 66));
                    hHMI.Graphics.DrawLine(needlePenRed, Poc(288, 221, 33, 0, tSpeed), Poc(288, 221, 72, 0, tSpeed));
                    hHMI.Graphics.FillPolygon(Brushes.Red, new Point[] {
                    Poc(288, 221, 80, -2, tSpeed), Poc(288, 221, 70, -5, tSpeed), Poc(288, 221, 70, 5, tSpeed), Poc(288, 221, 80, 2, tSpeed)
                    });
                    hHMI.Graphics.DrawLine(needleEndPenRed, Poc(288, 221, 75, 0, tSpeed), Poc(288, 221, 120, 0, tSpeed));
                }
                else
                {
                    if (tSpeed > tRecommend && tSpeed <= tLimit)
                    {
                        hHMI.Graphics.DrawEllipse(circlePenOrange, new Rectangle(255, 188, 66, 66));
                        hHMI.Graphics.DrawLine(needlePenOrange, Poc(288, 221, 33, 0, tSpeed), Poc(288, 221, 72, 0, tSpeed));
                        hHMI.Graphics.FillPolygon(Brushes.Orange, new Point[] {
                        Poc(288, 221, 80, -2, tSpeed), Poc(288, 221, 70, -5, tSpeed), Poc(288, 221, 70, 5, tSpeed), Poc(288, 221, 80, 2, tSpeed)
                        });
                        hHMI.Graphics.DrawLine(needleEndPenOrange, Poc(288, 221, 75, 0, tSpeed), Poc(288, 221, 120, 0, tSpeed));
                    }
                    else
                    {
                        hHMI.Graphics.DrawEllipse(circlePen, new Rectangle(255, 188, 66, 66));
                        hHMI.Graphics.DrawLine(needlePen, Poc(288, 221, 33, 0, tSpeed), Poc(288, 221, 72, 0, tSpeed));
                        hHMI.Graphics.FillPolygon(Brushes.White, new Point[] {
                        Poc(288, 221, 80, -2, tSpeed), Poc(288, 221, 70, -5, tSpeed), Poc(288, 221, 70, 5, tSpeed), Poc(288, 221, 80, 2, tSpeed)
                        });
                        hHMI.Graphics.DrawLine(needleEndPen, Poc(288, 221, 75, 0, tSpeed), Poc(288, 221, 120, 0, tSpeed));
                    }
                }
            }
            else
            {
                hHMI.Graphics.DrawEllipse(circlePen, new Rectangle(255, 188, 66, 66));
                hHMI.Graphics.DrawLine(needlePen, Poc(288, 221, 33, 0, tSpeed), Poc(288, 221, 72, 0, tSpeed));
                hHMI.Graphics.FillPolygon(Brushes.White, new Point[] {
                Poc(288, 221, 80, -2, tSpeed), Poc(288, 221, 70, -5, tSpeed), Poc(288, 221, 70, 5, tSpeed), Poc(288, 221, 80, 2, tSpeed)
                });
                hHMI.Graphics.DrawLine(needleEndPen, Poc(288, 221, 75, 0, tSpeed), Poc(288, 221, 120, 0, tSpeed));
            }

            //HMI上TDT显示的数字
            if (UrbalisAts.panel_[105] == 1 && UrbalisAts.panel_[51] == 0 && UrbalisAts.panel_[61] == 0)
            {
                hHMI.Graphics.DrawString(Convert.ToString(Math.Abs(UrbalisAts.panel_[106])), drawFont, new SolidBrush(Color.FromArgb(199, 199, 198)), 715, 82, stringC);
            }
            

            if (UrbalisAts.panel_[107] != 5)
            {
                if (counter == 0) {
                UrbalisAts.panel_[107]++;
                counter++;
            }
            else if (counter == 5)
                {
                counter = 0;
                }
                else
                {
                    counter++;
                }
            }
            else
            {
                if (counter == 0)
                {
                    UrbalisAts.panel_[107] = 0;
                    counter++;
                }
                else if (counter == 5)
                    {
                        counter = 0;
                    }
                else
                {
                    counter++;
                }
            }

            hHMI.Graphics.DrawString(TimeFormatter.MiliSecondToString(state.Time.TotalMilliseconds), timeFont, new SolidBrush(Color.FromArgb(199, 199, 198)), 225, 568);

            hHMI.Graphics.DrawString(System.DateTime.Now.ToShortDateString(), timeFont, new SolidBrush(Color.FromArgb(199, 199, 198)), 100, 568);


            if (UrbalisAts.panel_[109] == 1)
            {
                hHMI.Graphics.FillRectangle(targetColor[UrbalisAts.panel_[13] * 1 + UrbalisAts.panel_[14] * 2], new Rectangle(68, 354 - UrbalisAts.panel_[11], 20, UrbalisAts.panel_[11]));
                hHMI.Graphics.DrawString(Convert.ToString(UrbalisAts.panel_[17]), drawFont, new SolidBrush(Color.FromArgb(199, 199, 198)), 77, 104, stringC);
                hHMI.Graphics.DrawString(Convert.ToString(UrbalisAts.panel_[108]) + "m", distanceFont, new SolidBrush(Color.FromArgb(199, 199, 198)), 128, 333, stringC);
            }




            int depTimeDisplayX = 430;
            int depTimeDisplayY = 45;
            int destStationDisplayX = 250;
            int destStationDisplayY = 45;
            int nextStationDisplayX = 150;
            int nextStationDisplayY = 45;


            hHMI.Graphics.DrawString(UrbalisAts.mapPlugin.nextStaName, timeFont, new SolidBrush(Color.FromArgb(199, 199, 198)), nextStationDisplayX, nextStationDisplayY, stringC);
            hHMI.Graphics.DrawString(UrbalisAts.mapPlugin.destStaName, timeFont, new SolidBrush(Color.FromArgb(199, 199, 198)), destStationDisplayX, destStationDisplayY, stringC);

            if (UrbalisAts.panel_[105] == 1)
            {
                hHMI.Graphics.DrawString("发车时间", timeFont, new SolidBrush(Color.FromArgb(199, 199, 198)), depTimeDisplayX, 20, stringC);
                hHMI.Graphics.DrawString(TimeFormatter.MiliSecondToString(MapStationManager.NextStation.DepartureTime), timeFont, new SolidBrush(Color.FromArgb(199, 199, 198)), depTimeDisplayX, depTimeDisplayY, stringC);
            }

            if (UrbalisAts.panel_[51] == 0) hHMI.Graphics.DrawString("T" + UrbalisAts.TrainNumber, timeFont, new SolidBrush(Color.FromArgb(199, 199, 198)), 600, 20, stringC);

            hHMI.Graphics.DrawString("终点站", timeFont, new SolidBrush(Color.FromArgb(199, 199, 198)), destStationDisplayX, 20, stringC);
            hHMI.Graphics.DrawString("下一站", timeFont, new SolidBrush(Color.FromArgb(199, 199, 198)), nextStationDisplayX, 20, stringC);

            int nextStationDistance = MapStationManager.NextStation.StopPosition - Convert.ToInt32(state.Location);

            double dis = Math.Round(Convert.ToDouble(nextStationDistance), 1);
            string disF = dis.ToString("0");
            bool arrived = false;

            if (state.Speed == 0 && ( (Math.Abs(dis) <= Config.DoorEnableWindow) || MapStationManager.Stopped) )
            {
                arrived= true;
            }
            else if (MapStationManager.Arrived && UrbalisAts.panel_[106] == 0)
            {
                arrived = true;
            }
            else if (state.Location > MapStationManager.NextStation.StopPosition + Config.StationEndDistance)
            {
                arrived = true;
            }

            if (nextStationDistance < 1000 && nextStationDistance >= -5 && !MapStationManager.NextStation.Pass && !arrived)
            {
                hHMI.Graphics.DrawString("距下站", timeFont, new SolidBrush(Color.FromArgb(199, 199, 198)), 393, 391, stringC);
                hHMI.Graphics.DrawString(disF + "m", timeFont, new SolidBrush(Color.FromArgb(199, 199, 198)), 393, 416, stringC);
            }


            return hHMI;
        }


        static int last101 = 0, last102 = 0;
        public static GDIHelper PaintTDT(AtsEx.PluginHost.Native.VehicleState state) {
            if (UrbalisAts.panel_[101] == last101 && UrbalisAts.panel_[102] == last102) return null;
            hTDT.BeginGDI();
            Image digitImage;
            if (UrbalisAts.panel_[102] == -1) {
                hTDT.DrawImage(tdtbackred, 0, 0);
                digitImage = tdtdigitsred;
            } else if (UrbalisAts.panel_[102] == 1) {
                hTDT.DrawImage(tdtbackgreen, 0, 0);
                digitImage = tdtdigitsgreen;
            } else {
                hTDT.DrawImage(tdtbackoff, 0, 0);
                digitImage = null;
            }
            hTDT.EndGDI();
            if (digitImage != null)
            {
                for (int i = 0; i <= 2; i++)
                {
                    var xpos = 152 - 55 * i;
                    hTDT.Graphics.SetClip(new Rectangle(xpos, 67, 60, 120));
                    var di = D(UrbalisAts.panel_[101], i);
                    if (di == 10) di = 0;
                    hTDT.Graphics.DrawImageUnscaled(digitImage, xpos, 67 - 120 * di);
                }
                /*if (UrbalisAts.panel_[102] == -1)
                {

                }
                else if (UrbalisAts.panel_[102] == 1)
                {
                    hTDT.DrawImage(tdtdigitsgreen, 0, 0);
                }*/
            }

            last101 = UrbalisAts.panel_[101]; last102 = UrbalisAts.panel_[102];
            return hTDT;
        }

        public static int voltage;
        

        public static GDIHelper PaintHMI2(AtsEx.PluginHost.Native.VehicleState state, AtsEx.PluginHost.Handles.HandleSet handles)
        {
            var stringC = new StringFormat();
            stringC.Alignment = StringAlignment.Center;

            hHMI2.BeginGDI();
            hHMI2.DrawImage(hmi2, 0, 0);

            if (UrbalisAts.doorOpen)
            {
                switch (MapStationManager.NextStation.DoorOpenType)
                {
                    case 1:
                        hHMI2.DrawImage(dooropenleft, 121, 305); 
                        break;
                    case 2:
                        hHMI2.DrawImage(dooropenright, 118, 218);
                        break;
                }
            }

            string trainDirection = "?";
            string handlePos = "?";
            int dirpos = 0;
            string trainsigmode = "?";
            string traindrvmode = "?";
            string traindormode = "?";
            string trainselmode = "?";


            switch (handles.Reverser.Position)
            {
                case ReverserPosition.B:
                    trainDirection = "后";
                    dirpos = -1;
                    break;
                case ReverserPosition.N:
                    trainDirection = "0";
                    dirpos = 0;
                    break;
                case ReverserPosition.F:
                    trainDirection = "前";
                    dirpos = 1;
                    break;
            }

            if (handles.Power.Notch > 0)
            {
                switch (handles.Power.Notch)
                {
                    case 1:
                        handlePos = "P1";
                        break;
                    case 2:
                        handlePos = "P2";
                        break;
                    case 3:
                        handlePos = "P3";
                        break;
                    case 4:
                        handlePos = "P4";
                        break;
                }
            }
            else
            {
                switch (handles.Brake.Notch)
                {
                    case 0:
                        handlePos = "0";
                        break;
                    case 1:
                        handlePos = "B1";
                        break;
                    case 2:
                        handlePos = "B2";
                        break;
                    case 3:
                        handlePos = "B3";
                        break;
                    case 4:
                        handlePos = "B4";
                        break;
                    case 5:
                        handlePos = "B5";
                        break;
                    case 6:
                        handlePos = "B6";
                        break;
                    case 7:
                        handlePos = "B7";
                        break;
                    case 8:
                        handlePos = "EB";
                        break;
                }
            }

            switch (UrbalisAts.signalMode)
            {
                case 0:
                    trainsigmode = "IXL";
                    traindormode = "MM";
                    break;
                case 1:
                    trainsigmode = "ITC";
                    traindormode = "AA";
                    break;
                case 2:
                    trainsigmode = "CBTC";
                    traindormode = "AA";
                    break;
            }

            switch (UrbalisAts.driveMode)
            {
                case 0:
                    traindrvmode = "RM";
                    break;
                case 1:
                    traindrvmode = "CM";
                    break;
                case 2:
                    traindrvmode = "AM";
                    break;
            }

            string trainmode = trainsigmode + "-" + traindrvmode + "-" + traindormode;

            switch (UrbalisAts.selectedMode)
            {
                case 0:
                    trainselmode = "RM";
                    break;
                case 1:
                    trainselmode = "CM-ITC";
                    break;
                case 2:
                    trainselmode = "CM-CBTC";
                    break;
                case 3:
                    trainselmode = "AM-ITC";
                    break;
                case 4:
                    trainselmode = "AM-CBTC";
                    break;
            }

            switch (dirpos)
            {
                case -1:
                    hHMI2.DrawImage(trainkey, 119, 166);
                    hHMI2.DrawImage(traindir2, 709, 158);
                    break;
                case 0:
                    break;
                case 1:
                    hHMI2.DrawImage(trainkey, 119, 166);
                    hHMI2.DrawImage(traindir1, 59, 158);
                    break;
            }

            int[] cabX_ = new int[8];
            int[] listY_ = new int[16];

            cabX_[1] = 149;
            cabX_[2] = 236;
            cabX_[3] = 323;
            cabX_[4] = 410;
            cabX_[5] = 497;
            cabX_[6] = 584;

            listY_[1] = 371;
            listY_[2] = 396;
            listY_[3] = 421;
            listY_[4] = 446;
            listY_[5] = 471;
            listY_[6] = 496;
            listY_[7] = 521;
            listY_[8] = 546;



            //状态部分

            if (state.BcPressure > 70)
            {
                hHMI2.DrawImage(hmi2Yellow, cabX_[1], listY_[1]);
                hHMI2.DrawImage(hmi2Yellow, cabX_[2], listY_[1]);
                hHMI2.DrawImage(hmi2Yellow, cabX_[3], listY_[1]);
                hHMI2.DrawImage(hmi2Yellow, cabX_[4], listY_[1]);
                hHMI2.DrawImage(hmi2Yellow, cabX_[5], listY_[1]);
                hHMI2.DrawImage(hmi2Yellow, cabX_[6], listY_[1]);
            }
            else if (state.BcPressure > 0)
            {
                hHMI2.DrawImage(hmi2Green, cabX_[1], listY_[1]);
                hHMI2.DrawImage(hmi2Green, cabX_[2], listY_[1]);
                hHMI2.DrawImage(hmi2Green, cabX_[3], listY_[1]);
                hHMI2.DrawImage(hmi2Green, cabX_[4], listY_[1]);
                hHMI2.DrawImage(hmi2Green, cabX_[5], listY_[1]);
                hHMI2.DrawImage(hmi2Green, cabX_[6], listY_[1]);
            }



            if (state.Current < -50)
            {
                hHMI2.DrawImage(hmi2Yellow, cabX_[1], listY_[2]);
                hHMI2.DrawImage(hmi2Yellow, cabX_[3], listY_[2]);
                hHMI2.DrawImage(hmi2Yellow, cabX_[4], listY_[2]);
                hHMI2.DrawImage(hmi2Yellow, cabX_[6], listY_[2]);
            }
            else if (state.Current < 0)
            {
                hHMI2.DrawImage(hmi2Green, cabX_[1], listY_[2]);
                hHMI2.DrawImage(hmi2Green, cabX_[3], listY_[2]);
                hHMI2.DrawImage(hmi2Green, cabX_[4], listY_[2]);
                hHMI2.DrawImage(hmi2Green, cabX_[6], listY_[2]);
            }



            if (dirpos != 0)
            {

                hHMI2.DrawImage(hmi2Green, cabX_[1], listY_[3]);
                hHMI2.DrawImage(hmi2Green, cabX_[6], listY_[3]);

                hHMI2.DrawImage(hmi2Green, cabX_[2], listY_[5]);
                hHMI2.DrawImage(hmi2Green, cabX_[5], listY_[5]);

            }



            if (state.Current > 0)
            {
                hHMI2.DrawImage(hmi2Green, cabX_[1], listY_[4]);
                hHMI2.DrawImage(hmi2Green, cabX_[3], listY_[4]);
                if (state.Speed > 1) 
                {
                    hHMI2.DrawImage(hmi2Green, cabX_[4], listY_[4]);
                    hHMI2.DrawImage(hmi2Green, cabX_[6], listY_[4]);

                }

                if (state.Speed > 5)
                {
                    hHMI2.DrawImage(hmi2Green, cabX_[2], listY_[4]);
                    hHMI2.DrawImage(hmi2Green, cabX_[5], listY_[4]);
                }
            }



            if (UrbalisAts.doorOpen)
            {
                hHMI2.DrawImage(hmi2Red, cabX_[1], listY_[7]);
                hHMI2.DrawImage(hmi2Red, cabX_[2], listY_[7]);
                hHMI2.DrawImage(hmi2Red, cabX_[3], listY_[7]);
                hHMI2.DrawImage(hmi2Red, cabX_[4], listY_[7]);
                hHMI2.DrawImage(hmi2Red, cabX_[5], listY_[7]);
                hHMI2.DrawImage(hmi2Red, cabX_[6], listY_[7]);

            }
            else if (state.Speed == 0)
            {
                hHMI2.DrawImage(hmi2Green, cabX_[1], listY_[7]);
                hHMI2.DrawImage(hmi2Green, cabX_[2], listY_[7]);
                hHMI2.DrawImage(hmi2Green, cabX_[3], listY_[7]);
                hHMI2.DrawImage(hmi2Green, cabX_[4], listY_[7]);
                hHMI2.DrawImage(hmi2Green, cabX_[5], listY_[7]);
                hHMI2.DrawImage(hmi2Green, cabX_[6], listY_[7]);
            }



            hHMI2.EndGDI();


            double speed = Math.Round(Convert.ToDouble(state.Speed), 1);
            string speedF = speed.ToString("0.0");

            double ampere = Math.Round(Convert.ToDouble(state.Current), 1);
            string ampereF = ampere.ToString("0.0");

            string trainnumberStr = Convert.ToString(UrbalisAts.TrainNumber);

            string tgspeed = "?";
            string tgdistance = "?";

            if (UrbalisAts.panel_[109] == 1)
            {
                tgspeed = Convert.ToString(UrbalisAts.panel_[17]) + "km/h";
                tgdistance = Convert.ToString(UrbalisAts.panel_[108]) + "m";
            }

            //时间
            hHMI2.Graphics.DrawString(TimeFormatter.MiliSecondToString(state.Time.TotalMilliseconds), hmi2Font, new SolidBrush(Color.FromArgb(199, 199, 198)), 123, 20, stringC);
            //车号
            hHMI2.Graphics.DrawString("T" + trainnumberStr, hmi2Font, new SolidBrush(Color.FromArgb(199, 199, 198)), 123, 58, stringC);
            //模式
            hHMI2.Graphics.DrawString(trainmode, hmi2Font, new SolidBrush(Color.FromArgb(199, 199, 198)), 123, 96, stringC);

            //下一站
            hHMI2.Graphics.DrawString(UrbalisAts.mapPlugin.nextStaName, hmi2Font, new SolidBrush(Color.FromArgb(199, 199, 198)), 319, 20, stringC);
            //终点站
            hHMI2.Graphics.DrawString(UrbalisAts.mapPlugin.destStaName, hmi2Font, new SolidBrush(Color.FromArgb(199, 199, 198)), 319, 58, stringC);
            //预选模式
            hHMI2.Graphics.DrawString(trainselmode, hmi2Font, new SolidBrush(Color.FromArgb(199, 199, 198)), 319, 96, stringC);

            //手柄级位
            hHMI2.Graphics.DrawString(trainDirection + " " + handlePos, hmi2Font, new SolidBrush(Color.FromArgb(199, 199, 198)), 532, 20, stringC);
            //牵引电流
            hHMI2.Graphics.DrawString(ampereF + "A", hmi2Font, new SolidBrush(Color.FromArgb(199, 199, 198)), 532, 58, stringC);
            //下一限速
            hHMI2.Graphics.DrawString(tgspeed, hmi2Font, new SolidBrush(Color.FromArgb(199, 199, 198)), 532, 96, stringC);

            //网压
            hHMI2.Graphics.DrawString(Convert.ToString(TGMTPainter.voltage) + "V", hmi2Font, new SolidBrush(Color.FromArgb(199, 199, 198)), 713, 20, stringC);
            //速度
            hHMI2.Graphics.DrawString(Convert.ToString(speedF) + "km/h", hmi2Font, new SolidBrush(Color.FromArgb(199, 199, 198)), 713, 58, stringC);
            //距离
            hHMI2.Graphics.DrawString(tgdistance, hmi2Font, new SolidBrush(Color.FromArgb(199, 199, 198)), 713, 96, stringC);


            int cabinitposX = 192;
            int cabinitposY = 175;

            hHMI2.Graphics.DrawString(trainnumberStr + "01", hmi2Font, new SolidBrush(Color.FromArgb(199, 199, 198)), cabinitposX + (0 * 87), cabinitposY, stringC);
            hHMI2.Graphics.DrawString(trainnumberStr + "02", hmi2Font, new SolidBrush(Color.FromArgb(199, 199, 198)), cabinitposX + (1 * 87), cabinitposY, stringC);
            hHMI2.Graphics.DrawString(trainnumberStr + "03", hmi2Font, new SolidBrush(Color.FromArgb(199, 199, 198)), cabinitposX + (2 * 87), cabinitposY, stringC);
            hHMI2.Graphics.DrawString(trainnumberStr + "04", hmi2Font, new SolidBrush(Color.FromArgb(199, 199, 198)), cabinitposX + (3 * 87), cabinitposY, stringC);
            hHMI2.Graphics.DrawString(trainnumberStr + "05", hmi2Font, new SolidBrush(Color.FromArgb(199, 199, 198)), cabinitposX + (4 * 87), cabinitposY, stringC);
            hHMI2.Graphics.DrawString(trainnumberStr + "06", hmi2Font, new SolidBrush(Color.FromArgb(199, 199, 198)), cabinitposX + (5 * 87), cabinitposY, stringC);




            return hHMI2;
        }
        static int[] pow10 = new int[] { 1, 10, 100, 1000, 10000, 100000 };

        static int D(int src, int digit) {
            if (pow10[digit] > src) {
                return 10;
            } else if (digit == 0 && src == 0) {
                return 0;
            } else {
                return src / pow10[digit] % 10;
            }
        }

        static Point Poc(int cx, int cy, int dr, int dt, double theta) {
            return new Point(
                (int)(cx + dr * Math.Sin(theta) + dt * Math.Cos(theta)),
                (int)(cy - dr * Math.Cos(theta) + dt * Math.Sin(theta))
            );
        }


        static Pen needlePen = new Pen(Color.White, 10);
        static Pen needlePenOrange = new Pen(Color.Orange, 10);
        static Pen needlePenRed = new Pen(Color.Red, 10);
        static Pen needleEndPen = new Pen(Color.White, 4);
        static Pen needleEndPenOrange = new Pen(Color.Orange, 4);
        static Pen needleEndPenRed = new Pen(Color.Red, 4);
        static Pen circlePen = new Pen(Color.White, 3);
        static Pen circlePenOrange = new Pen(Color.Orange, 3);
        static Pen circlePenRed = new Pen(Color.Red, 3);
        static Pen ackPen = new Pen(Color.Yellow, 4);
        static Brush[] targetColor = new Brush[] { new SolidBrush(Color.Red), new SolidBrush(Color.Orange), new SolidBrush(Color.Green) };
        static Brush[] overspeed = new Brush[] { new SolidBrush(Color.Empty), new SolidBrush(Color.Orange), new SolidBrush(Color.Red) };
        static Bitmap hmi, ackcmd, atoctrl, dormode, dorrel, drvmode, emergency, fault, departure, menu,
            selmode, sigmode, special, stopsig, num0, numn0, colon, hmitdt, life, distance, msg, rmpanel, bmconfirm, menuext, crewnumenter;
        static Bitmap tdtbackoff, tdtbackred, tdtbackgreen;
        static Bitmap hmi2, dooropenleft, dooropenright, trainkey, traindir1, traindir2, hmi2Green, hmi2Red, hmi2Yellow;
        static Image tdtdigitsred, tdtdigitsgreen;
        //static Bitmap tdtdigitsred, tdtdigitsgreen;
        static System.Drawing.Font drawFont, timeFont, distanceFont, hmi2Font, crewNumFont;
    }
}
