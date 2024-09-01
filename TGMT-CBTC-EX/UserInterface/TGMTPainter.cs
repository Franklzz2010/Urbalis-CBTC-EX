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

namespace TGMTAts.OBCU {

    public static class TGMTPainter {

        public static string INIPath = Convert.ToString(Path.Combine(Config.PluginDir, "StationList.ini"));

        public static GDIHelper hHMI, hTDT;
        public static void Initialize() {
            var imgDir = Config.ImageAssetPath;

            hHMI = new GDIHelper(1024, 1024);
            hTDT = new GDIHelper(256, 256);
            
            

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

            drawFont = new System.Drawing.Font("思源黑体 CN Bold", 30);
            timeFont = new System.Drawing.Font("思源黑体 CN Bold", 15);
            distanceFont = new System.Drawing.Font("思源黑体 CN Bold", 16);

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
        }

        public static void Dispose() {
            hHMI.Dispose();
            hTDT.Dispose();
        }
        static int counter = 0;


        public static GDIHelper PaintHMI(AtsEx.PluginHost.Native.VehicleState state) {
            hHMI.BeginGDI();
            hHMI.DrawImage(hmi, 0, 0);

            if (TGMTAts.driveMode == 0)
            {
                hHMI.DrawImage(rmpanel, 138, 73);
            }

            if (TGMTAts.panel_[102] != 0)
            {
                if (state.Speed == 0)
                {
                    TGMTAts.panel_[105] = 1;
                }
                else
                {
                    TGMTAts.panel_[105] = 0;
                }
            }
            else
            {
                TGMTAts.panel_[105] = 0;
            }

            if (TGMTAts.selectingMode == -1 && TGMTAts.ackMessage == 0)
            {
                hHMI.DrawImage(msg, 115, 480, TGMTAts.msgContext1 * 18, 18);
                hHMI.DrawImage(msg, 115, 501, TGMTAts.msgContext2 * 18, 18);
                hHMI.DrawImage(msg, 115, 522, TGMTAts.msgContext3 * 18, 18);
            }



            hHMI.DrawImage(menu, 551, 520, TGMTAts.panel_[23] * 64, 64);
            hHMI.DrawImage(drvmode, 530, 133, TGMTAts.panel_[24] * 64, 64);
            hHMI.DrawImage(sigmode, 655, 133, TGMTAts.panel_[25] * 64, 64);
            hHMI.DrawImage(stopsig, 670, 200, TGMTAts.panel_[26] * 64, 64);
            hHMI.DrawImage(dorrel, 530, 267, TGMTAts.panel_[27] * 64, 64);
            hHMI.DrawImage(dormode, 530, 337, TGMTAts.panel_[28] * 64, 64);
            hHMI.DrawImage(departure, 670, 267, TGMTAts.panel_[32] * 64, 64);
            hHMI.DrawImage(emergency, 670, 337, TGMTAts.panel_[29] * 64, 64);
            hHMI.DrawImage(fault, 530, 405, TGMTAts.panel_[30] * 64, 64);
            hHMI.DrawImage(special, 670, 405, TGMTAts.panel_[31] * 64, 64);

            if (TGMTAts.selectingMode != -1 || TGMTAts.ackMessage != 0)
            {
                hHMI.DrawImage(ackcmd, 55, 475, TGMTAts.panel_[35] * 70, 70);
            }
                        
            hHMI.DrawImage(atoctrl, 32, 380, TGMTAts.panel_[21] * 64, 64);
            hHMI.DrawImage(selmode, 130, 380, TGMTAts.panel_[22] * 64, 64);
            hHMI.DrawImage(hmitdt, 530, 70, TGMTAts.panel_[105] * 64, 64);
            hHMI.DrawImage(life, 15, 560, TGMTAts.panel_[107] * 40, 40);

            if (TGMTAts.panel_[109] == 1)
            {
                hHMI.DrawImage(distance, 10, 144);
            }

            hHMI.DrawImage(num0, 289, 212, D((int)Math.Abs(Math.Ceiling(state.Speed)), 0) * 18, 18);
            hHMI.DrawImage(numn0, 275, 212, D((int)Math.Abs(Math.Ceiling(state.Speed)), 1) * 18, 18);



            var second = Convert.ToInt32(state.Time.TotalMilliseconds) / 1000 % 60;
            var minute = Convert.ToInt32(state.Time.TotalMilliseconds) / 1000 / 60 % 60;
            var hour = Convert.ToInt32(state.Time.TotalMilliseconds) / 1000 / 3600 % 60;

            var hh = Convert.ToString(hour);
            var mm = Convert.ToString(minute);
            var ss = Convert.ToString(second);
            hh = hh.PadLeft(2, '0');
            mm = mm.PadLeft(2, '0');
            ss = ss.PadLeft(2, '0');

            

            /*hHMI.DrawImage(num0, 60, 582, D(hrs, 1) * 18, 18);
            hHMI.DrawImage(num0, 74, 582, D(hrs, 0) * 18, 18);
            hHMI.DrawImage(num0, 102, 582, D(min, 1) * 18, 18);
            hHMI.DrawImage(num0, 116, 582, D(min, 0) * 18, 18);
            hHMI.DrawImage(num0, 144, 582, D(sec, 1) * 18, 18);
            hHMI.DrawImage(num0, 158, 582, D(sec, 0) * 18, 18);
            if (sec % 2 == 0) {
                hHMI.DrawImage(colon, 88, 582);
                hHMI.DrawImage(colon, 130, 582);
            }*/
            hHMI.EndGDI();

            var stringC = new StringFormat();
            stringC.Alignment = StringAlignment.Center;


            if (TGMTAts.selectingMode == -1 && TGMTAts.ackMessage == 0)
            {
                hHMI.Graphics.DrawString(TGMTAts.msgTime1, timeFont, new SolidBrush(Color.FromArgb(199, 199, 198)), 83, 479, stringC);
                hHMI.Graphics.DrawString(TGMTAts.msgTime2, timeFont, new SolidBrush(Color.FromArgb(199, 199, 198)), 83, 499, stringC);
                hHMI.Graphics.DrawString(TGMTAts.msgTime3, timeFont, new SolidBrush(Color.FromArgb(199, 199, 198)), 83, 519, stringC);
            }

            hHMI.Graphics.FillRectangle(overspeed[TGMTAts.panel_[10]], new Rectangle(20, 18, 80, 78));
            if (TGMTAts.panel_[36] != 0 && TGMTAts.time % 500 < 250) {
                hHMI.Graphics.DrawRectangle(ackPen, new Rectangle(53, 473, 280, 70));
            }
            if (TGMTAts.panel_[15] >= 0) {
                var tRecommend = ((double)TGMTAts.panel_[15] / 400 * 288 - 144) / 180 * Math.PI;
                hHMI.Graphics.FillPolygon(Brushes.Yellow, new Point[] {
                    Poc(288, 221, 130, 0, tRecommend), Poc(288, 221, 150, -11, tRecommend), Poc(288, 221, 150, 11, tRecommend)
                });
            }
            if (TGMTAts.panel_[16] >= 0) {
                var tLimit = ((double)TGMTAts.panel_[16] / 400 * 288 - 144) / 180 * Math.PI;
                hHMI.Graphics.FillPolygon(Brushes.Red, new Point[] {
                    Poc(288, 221, 130, 0, tLimit), Poc(288, 221, 150, -11, tLimit), Poc(288, 221, 150, 11, tLimit)
                });
            }
            var tSpeed = ((double)TGMTAts.panel_[1] / 400 * 288 - 144) / 180 * Math.PI;
            if (TGMTAts.panel_[15] >= 0)
            {
                var tRecommend = ((double)TGMTAts.panel_[15] / 400 * 288 - 144) / 180 * Math.PI;
                var tLimit = ((double)TGMTAts.panel_[16] / 400 * 288 - 144) / 180 * Math.PI;
                if (TGMTAts.panel_[29] == 2)
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
            if (TGMTAts.panel_[105] == 1)
            {
                hHMI.Graphics.DrawString(Convert.ToString(Math.Abs(TGMTAts.panel_[106])), drawFont, new SolidBrush(Color.FromArgb(199, 199, 198)), 715, 82, stringC);
            }
            

            if (TGMTAts.panel_[107] != 5)
            {
                if (counter == 0) {
                TGMTAts.panel_[107]++;
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
                    TGMTAts.panel_[107] = 0;
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

            hHMI.Graphics.DrawString(hh + ":" + mm + ":" + ss, timeFont, new SolidBrush(Color.FromArgb(199, 199, 198)), 225, 568);

            hHMI.Graphics.DrawString(System.DateTime.Now.ToShortDateString(), timeFont, new SolidBrush(Color.FromArgb(199, 199, 198)), 100, 568);


            if (TGMTAts.panel_[109] == 1)
            {
                hHMI.Graphics.FillRectangle(targetColor[TGMTAts.panel_[13] * 1 + TGMTAts.panel_[14] * 2], new Rectangle(68, 354 - TGMTAts.panel_[11], 20, TGMTAts.panel_[11]));
                hHMI.Graphics.DrawString(Convert.ToString(TGMTAts.panel_[17]), drawFont, new SolidBrush(Color.FromArgb(199, 199, 198)), 77, 104, stringC);
                hHMI.Graphics.DrawString(Convert.ToString(TGMTAts.panel_[108]) + "m", distanceFont, new SolidBrush(Color.FromArgb(199, 199, 198)), 128, 333, stringC);
            }

            var depSecond = StationManager.NextStation.DepartureTime / 1000 % 60;
            var depMinute = StationManager.NextStation.DepartureTime / 1000 / 60 % 60;
            var depHour = StationManager.NextStation.DepartureTime / 1000 / 3600 % 60;

            string dh = Convert.ToString(depHour);
            string dm = Convert.ToString(depMinute);
            string ds = Convert.ToString(depSecond);
            dh = dh.PadLeft(2, '0');
            dm = dm.PadLeft(2, '0');
            ds = ds.PadLeft(2, '0');


            int depTimeDisplayX = 450;
            int depTimeDisplayY = 45;
            int destStationDisplayX = 250;
            int destStationDisplayY = 45;
            int nextStationDisplayX = 150;
            int nextStationDisplayY = 45;


            FilesINI ConfigINI = new FilesINI();
            string stationNameStr = ConfigINI.INIRead("station", Convert.ToString(TGMTAts.nextStationNumber), INIPath);
            hHMI.Graphics.DrawString(stationNameStr, timeFont, new SolidBrush(Color.FromArgb(199, 199, 198)), nextStationDisplayX, nextStationDisplayY, stringC);
            string destStationNameStr = ConfigINI.INIRead("station", Convert.ToString(TGMTAts.DestinationNumber), INIPath);
            hHMI.Graphics.DrawString(destStationNameStr, timeFont, new SolidBrush(Color.FromArgb(199, 199, 198)), destStationDisplayX, destStationDisplayY, stringC);

            if (TGMTAts.panel_[105] == 1)
            {
                hHMI.Graphics.DrawString("发车时间", timeFont, new SolidBrush(Color.FromArgb(199, 199, 198)), depTimeDisplayX, 20, stringC);
                hHMI.Graphics.DrawString(dh + ":" + dm + ":" + ds, timeFont, new SolidBrush(Color.FromArgb(199, 199, 198)), depTimeDisplayX, depTimeDisplayY, stringC);
            }

            hHMI.Graphics.DrawString("T" + TGMTAts.TrainNumber, timeFont, new SolidBrush(Color.FromArgb(199, 199, 198)), 600, 20, stringC);

            hHMI.Graphics.DrawString("终点站", timeFont, new SolidBrush(Color.FromArgb(199, 199, 198)), destStationDisplayX, 20, stringC);
            hHMI.Graphics.DrawString("下一站", timeFont, new SolidBrush(Color.FromArgb(199, 199, 198)), nextStationDisplayX, 20, stringC);

            return hHMI;
        }


        static int last101 = 0, last102 = 0;
        public static GDIHelper PaintTDT(AtsEx.PluginHost.Native.VehicleState state) {
            if (TGMTAts.panel_[101] == last101 && TGMTAts.panel_[102] == last102) return null;
            hTDT.BeginGDI();
            Image digitImage;
            if (TGMTAts.panel_[102] == -1) {
                hTDT.DrawImage(tdtbackred, 0, 0);
                digitImage = tdtdigitsred;
            } else if (TGMTAts.panel_[102] == 1) {
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
                    var di = D(TGMTAts.panel_[101], i);
                    if (di == 10) di = 0;
                    hTDT.Graphics.DrawImageUnscaled(digitImage, xpos, 67 - 120 * di);
                }
                /*if (TGMTAts.panel_[102] == -1)
                {

                }
                else if (TGMTAts.panel_[102] == 1)
                {
                    hTDT.DrawImage(tdtdigitsgreen, 0, 0);
                }*/
            }

            last101 = TGMTAts.panel_[101]; last102 = TGMTAts.panel_[102];
            return hTDT;
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
            selmode, sigmode, special, stopsig, num0, numn0, colon, hmitdt, life, distance, msg, rmpanel;
        static Bitmap tdtbackoff, tdtbackred, tdtbackgreen;
        static Image tdtdigitsred, tdtdigitsgreen;
        //static Bitmap tdtdigitsred, tdtdigitsgreen;
        static System.Drawing.Font drawFont, timeFont, distanceFont;
    }
}
