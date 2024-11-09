using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Zbx1425.DXDynamicTexture;

namespace UrbalisAts.OBCU.UserInterface
{
    public static class HMITouch
    {
        public static int crewNumberPreEnter;
        public static string crewNumberPreEnterStr;

        public static void OnHMITexMouseDown(object sender, TouchEventArgs e)
        {
            //强制后备模式提示框
            if (UrbalisAts.panel_[50] == 1)
            {
                if (e.Y >= 437 && e.Y <= 483)
                {
                    if (e.X >= 586 && e.X <= 661)
                    {
                        if (UrbalisAts.panel_[50] == 1) UrbalisAts.selectedMode = 3;
                        UrbalisAts.FixIncompatibleModes();
                        UrbalisAts.panel_[50] = 0;
                    }
                    else if (e.X >= 682 && e.X <= 758)
                    {
                        if (UrbalisAts.panel_[50] == 1) UrbalisAts.selectedMode = 4;
                        UrbalisAts.panel_[50] = 0;
                    }
                }
            }

            if (e.X >= 357 && e.X <= 396)
            {
                if (MsgManager.canUp && e.Y >= 522 && e.Y <= 569)
                {
                    MsgManager.page++;
                    MsgManager.UpdateMsg();
                }
                else if (MsgManager.canDown && e.Y >= 449 && e.Y <= 495)
                {
                    MsgManager.page--;
                    MsgManager.UpdateMsg();
                }

            }

            //司机号输入
            if (e.X >= 668 && e.X <= 779 && e.Y >= 16 && e.Y <= 68 && UrbalisAts.panel_[23] == 0 && UrbalisAts.panel_[51] == 0)
            {
                UrbalisAts.panel_[61] = 1;
            }
            //司机号输入键盘
            if (UrbalisAts.panel_[61] == 1 && UrbalisAts.panel_[51] == 0)
            {
                //第一列
                if (e.X >= 566 && e.X <= 610)
                {
                    //7
                    if (e.Y >= 267 && e.Y <= 301)
                    {
                        SetCrewNumber(7);
                    }

                    //4
                    if (e.Y >= 312 && e.Y <= 347)
                    {
                        SetCrewNumber(4);
                    }
                    //1
                    if (e.Y >= 359 && e.Y <= 391)
                    {
                        SetCrewNumber(1);
                    }
                    //关闭键盘
                    if (e.Y >= 404 && e.Y <= 436)
                    {
                        UrbalisAts.panel_[60] = UrbalisAts.panel_[61] = UrbalisAts.panel_[62] = 0;
                    }
                }

                //第二列
                if (e.X >= 633 && e.X <= 675)
                {
                    //8
                    if (e.Y >= 267 && e.Y <= 301)
                    {
                        SetCrewNumber(8);
                    }
                    //5
                    if (e.Y >= 312 && e.Y <= 347)
                    {
                        SetCrewNumber(5);
                    }
                    //2
                    if (e.Y >= 359 && e.Y <= 391)
                    {
                        SetCrewNumber(2);
                    }
                    //0
                    if (e.Y >= 404 && e.Y <= 436)
                    {
                        SetCrewNumber(0);
                    }

                }

                //第三列
                if (e.X >= 700 && e.X <= 742)
                {
                    //9
                    if (e.Y >= 267 && e.Y <= 301)
                    {
                        SetCrewNumber(9);
                    }
                    //6
                    if (e.Y >= 312 && e.Y <= 347)
                    {
                        SetCrewNumber(6);
                    }
                    //3
                    if (e.Y >= 359 && e.Y <= 391)
                    {
                        SetCrewNumber(3);
                    }
                    //输入
                    if (e.Y >= 404 && e.Y <= 436)
                    {
                        UrbalisAts.panel_[63] = UrbalisAts.panel_[60];

                        crewNumberPreEnter = UrbalisAts.panel_[60] = UrbalisAts.panel_[61] = UrbalisAts.panel_[62] = 0;
                    }
                }
            }

            //菜单
            if (e.Y >= 522 && e.Y <= 567)
            {
                //菜单展开
                if (e.X >= 456 && e.X <= 604 && UrbalisAts.panel_[23] == 0 && UrbalisAts.panel_[51] == 0)
                {
                    UrbalisAts.panel_[51] = 1;
                }
                //菜单收起
                else if (e.X >= 498 && e.X <= 776 && UrbalisAts.panel_[51] == 1)
                {
                    UrbalisAts.panel_[51] = 0;
                }
            }
        }

        public static void SetCrewNumber(int number)
        {
            if (UrbalisAts.panel_[62] == 0)
            {
                UrbalisAts.panel_[60] += number * 100;
                crewNumberPreEnter = crewNumberPreEnter * 10 + number;
                crewNumberPreEnterStr = crewNumberPreEnter.ToString().PadLeft(1, '0');
                UrbalisAts.panel_[62]++;
            }
            else if (UrbalisAts.panel_[62] == 1)
            {
                UrbalisAts.panel_[60] += number * 10;
                crewNumberPreEnter = crewNumberPreEnter * 10 + number;
                crewNumberPreEnterStr = crewNumberPreEnter.ToString().PadLeft(2, '0');
                UrbalisAts.panel_[62]++;
            }
            else if (UrbalisAts.panel_[62] == 2)
            {
                UrbalisAts.panel_[60] += number;
                crewNumberPreEnter = crewNumberPreEnter * 10 + number;
                crewNumberPreEnterStr = crewNumberPreEnter.ToString().PadLeft(3, '0');
                UrbalisAts.panel_[62]++;
            }
        }
    }
}
