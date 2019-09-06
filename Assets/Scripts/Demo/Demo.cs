using ControlNS;
using System.Collections.Generic;
using UnityEngine;

namespace GameModeSettingNS
{


    public class Demo : MonoBehaviour
    {
        GameModeSettingData settingData = new GameModeSettingData();

        ListViewer listViewer;
        RoundCheckBox peopleCount;

        Label tips;
        RoundCheckBox[] turn = new RoundCheckBox[3];
        RoundCheckBox[] joker = new RoundCheckBox[5];
        CheckBox highScoreMode;
        CheckBox freeMaPai;
        RoundCheckBox[] optional = new RoundCheckBox[3];
        SuitSelecter suitSelecter;

        RoundCheckBox[] costmode = new RoundCheckBox[2];
        RoundCheckBox[] cmpcard;
        CostInfo costinfo;

        void Start()
        {
            listViewer = GetComponent<ListViewer>();
            Create();
            SettingDataToViewer();
            tips.Text = "支持多张大小王代替其它牌的玩法";
        }


        public void Create()
        {
            listViewer.Destroy();
            CreateInfo1();
            CreateInfo2();
            CreateInfo3();
            CreateInfo4();
            CreateInfo5();
            CreateInfo6();
            CreateInfo7();
        }

        void SettingDataToViewer()
        {
            peopleCount.IsCheck = true;

            if (settingData.turnCount == 10)
                turn[0].IsCheck = true;
            else if (settingData.turnCount == 20)
                turn[1].IsCheck = true;
            else if (settingData.turnCount == 30)
                turn[2].IsCheck = true;

            switch (settingData.jokerCount)
            {
                case 0: joker[joker.Length - 1].IsCheck = true; break;
                case 2: joker[0].IsCheck = true; break;
                case 4: joker[1].IsCheck = true; break;
                case 6: joker[2].IsCheck = true; break;
                case 8: joker[3].IsCheck = true; break;
            }

            optional[settingData.opt].IsCheck = true;
            suitSelecter.SelectedSuit = settingData.suit;

            if (settingData.roomCostMode == 2)
                costmode[0].IsCheck = true;
            else
                costmode[1].IsCheck = true;

            cmpcard[0].IsCheck = true;

            //
            SetRoomCostInfo();
        }

        void SetRoomCostInfo()
        {
            int roomCardAmount;

            if (settingData.roomCostMode == 2)
            {
                roomCardAmount = 10;
                settingData.mustDimanodCount = 6;
                settingData.mustRoomCardCount = roomCardAmount;
                costinfo.DiamondAmount = settingData.mustDimanodCount;
                costinfo.RoomCardAmount = settingData.mustRoomCardCount;
            }

            else
            {
                settingData.mustRoomCardCount = 0;
                settingData.mustDimanodCount = 5;

                costinfo.DiamondAmount = settingData.mustDimanodCount;
                costinfo.RoomCardAmount = 0;
            }
        }

        void CreateInfo1()
        {
            Tabler tabler = Control.Create<Tabler>();
            tabler.SetDefaultCellMargin(new Margin(0, 0, 0, 0));
            listViewer.AddChild(tabler, 30);


            tips = Control.Create<Label>();
            tips.CtrlSizeChangeMode = ControlSizeChangeMode.FitContentSize;
            tips.Height = 25;
            tips.Text = "支持多张大小王作癞子的玩法";

            tabler.AddChild(tips, 0, 0);
            tabler.EnableTableLineAutoAdjustRichSize(0, true, LineDir.HORIZONTAL);
            tabler.EnableTableLineAutoAdjustRichSize(0, true, LineDir.VERTICAL);

        }

        void CreateInfo2()
        {
            Tabler tabler = Control.Create<Tabler>();
            tabler.SetDefaultCellMargin(new Margin(0, 0, 30, 0));
            listViewer.AddChild(tabler, 50);

            Label peopleCountLabel = Control.Create<Label>();
            peopleCountLabel.CtrlSizeChangeMode = ControlSizeChangeMode.FitContentSize;
            peopleCountLabel.Height = 25;
            peopleCountLabel.Text = "人数:";
            tabler.AddChild(peopleCountLabel, 0, 0);
            tabler.SetCellMargin(0, 0, new Margin(10, 0, 10, 0));

            peopleCount = Control.Create<RoundCheckBox>();
            peopleCount.CtrlSizeChangeMode = ControlSizeChangeMode.FitContentSize;
            peopleCount.Height = 25;
            peopleCount.Text = "3-8人";
            tabler.AddChild(peopleCount, 0, 1);

            Label turnLabel = Control.Create<Label>();
            turnLabel.CtrlSizeChangeMode = ControlSizeChangeMode.FitContentSize;
            turnLabel.Height = 25;
            turnLabel.Text = "局数:";
            tabler.AddChild(turnLabel, 0, 3);
            tabler.SetCellMargin(0, 3, new Margin(0, 0, 20, 0));

            string[] name = { "10局", "20局", "30局" };
            for (int i = 0; i < name.Length; i++)
            {
                turn[i] = Control.Create<RoundCheckBox>();
                turn[i].CtrlSizeChangeMode = ControlSizeChangeMode.FitContentSize;
                turn[i].Height = 25;
                turn[i].Text = name[i];
                turn[i].BindProcess = TrunProcess;
                tabler.AddChild(turn[i], 0, 4 + i);
            }

            tabler.EnableTableLineAutoAdjustRichSize(2, true, LineDir.VERTICAL);
            tabler.EnableTableLineAutoAdjustRichSize(0, true, LineDir.HORIZONTAL);
        }

        void CreateInfo3()
        {
            Tabler tabler = Control.Create<Tabler>();
            listViewer.AddChild(tabler, 100);

            Tabler ctabler0 = Control.Create<Tabler>();
            tabler.AddChild(ctabler0, 1, 0);
            Tabler ctabler1 = Control.Create<Tabler>();
            tabler.AddChild(ctabler1, 0, 0);
            tabler.EnableTableAllLineAutoAdjustRichSize(true);

            Label playLabel = Control.Create<Label>();
            playLabel.CtrlSizeChangeMode = ControlSizeChangeMode.FitContentSize;
            playLabel.Height = 25;
            playLabel.Text = "玩法:";
            ctabler0.AddChild(playLabel, 0, 0);
            ctabler0.SetCellMargin(0, 0, new Margin(10, 0, 0, 0));

            string[] joNames = new string[] { "2张王", "4张王", "6张王", "8张王", "增加1人加1王" };
            for (int i = 0; i <= 4; i++)
            {
                joker[i] = Control.Create<RoundCheckBox>();
                joker[i].CtrlSizeChangeMode = ControlSizeChangeMode.FitContentSize;
                joker[i].Height = 25;
                joker[i].Text = joNames[i];
                joker[i].BindProcess = JokerCountSelect;
                ctabler0.AddChild(joker[i], 0, i + 1);

                if (i != 4)
                    joker[i].IsDisabled = true;
            }
            ctabler0.EnableTableAllLineAutoAdjustRichSize(true);
            ctabler0.EnableTableLineAutoAdjustRichSize(0, false, LineDir.VERTICAL);

            Label playLabel2 = Control.Create<Label>();
            playLabel2.CtrlSizeChangeMode = ControlSizeChangeMode.FitContentSize;
            playLabel2.Height = 25;
            playLabel2.Text = "玩法:";
            playLabel2.IsHideText = true;
            ctabler1.SetCellMargin(0, 0, new Margin(10, 0, 0, 0));
            ctabler1.AddChild(playLabel2, 0, 0);
            ctabler1.EnableTableLineAutoAdjustRichSize(0, true, LineDir.HORIZONTAL);

            string[] modeNames = new string[] { "高分模式", "自由马牌" };
            for (int i = 0; i < modeNames.Length; i++)
            {
                CheckBox checkBox = Control.Create<CheckBox>();
                checkBox.CtrlSizeChangeMode = ControlSizeChangeMode.FitContentSize;
                checkBox.Height = 25;
                checkBox.FontSize = 25;
                checkBox.Text = modeNames[i];
                ctabler1.AddChild(checkBox, 0, i + 1);
                ctabler1.SetCellMargin(0, i + 1, new Margin(10, 0, 10, 0));

                if (i == 0)
                {
                    highScoreMode = checkBox;
                    highScoreMode.BindProcess = HighScore;
                }
                else
                {
                    freeMaPai = checkBox;
                    freeMaPai.BindProcess = FreeMaPai;
                }

            }
        }

        void CreateInfo4()
        {
            Tabler tabler = Control.Create<Tabler>();
            tabler.SetDefaultCellMargin(new Margin(0, 0, 30, 0));
            listViewer.AddChild(tabler, 50);


            Label selectLabel = Control.Create<Label>();
            selectLabel.CtrlSizeChangeMode = ControlSizeChangeMode.FitContentSize;
            selectLabel.Height = 25;
            selectLabel.Text = "可选:";
            tabler.AddChild(selectLabel, 0, 0);
            tabler.SetCellMargin(0, 0, new Margin(10, 0, 10, 0));

            string[] name = { "默认", "纯一色", "加一色" };
            for (int i = 0; i < name.Length; i++)
            {
                optional[i] = Control.Create<RoundCheckBox>();
                optional[i].MatchType = MatchType.None;
                optional[i].CtrlSizeChangeMode = ControlSizeChangeMode.FitContentSize;
                optional[i].Height = 25;
                optional[i].Text = name[i];
                optional[i].BindProcess = Optional;
                tabler.AddChild(optional[i], 0, i + 1);
            }

            suitSelecter = Control.Create<SuitSelecter>();
            suitSelecter.MatchType = MatchType.None;
            suitSelecter.CtrlSizeChangeMode = ControlSizeChangeMode.FitContentSize;
            suitSelecter.Height = 40;
            suitSelecter.Width = 150;
            tabler.AddChild(suitSelecter, 0, 4);

            tabler.EnableTableLineAutoAdjustRichSize(0, true, LineDir.HORIZONTAL);

        }

        void CreateInfo5()
        {
            Tabler tabler = Control.Create<Tabler>();
            tabler.SetDefaultCellMargin(new Margin(0, 0, 30, 0));
            listViewer.AddChild(tabler, 50);


            Label selectLabel = Control.Create<Label>();
            selectLabel.CtrlSizeChangeMode = ControlSizeChangeMode.FitContentSize;
            selectLabel.Height = 25;
            selectLabel.Text = "付费方式:";
            tabler.AddChild(selectLabel, 0, 0);
            tabler.SetCellMargin(0, 0, new Margin(10, 0, 10, 0));

            costmode[0] = Control.Create<RoundCheckBox>();
            costmode[0].CtrlSizeChangeMode = ControlSizeChangeMode.FitContentSize;
            costmode[0].Height = 25;
            costmode[0].Text = "房主付费";
            costmode[0].BindProcess = CostMode;
            tabler.AddChild(costmode[0], 0, 1);

            costmode[1] = Control.Create<RoundCheckBox>();
            costmode[1].CtrlSizeChangeMode = ControlSizeChangeMode.FitContentSize;
            costmode[1].Height = 25;
            costmode[1].Text = "平摊付费";
            costmode[1].BindProcess = CostMode;
            tabler.AddChild(costmode[1], 0, 2);

            tabler.EnableTableLineAutoAdjustRichSize(0, true, LineDir.HORIZONTAL);

        }

        void CreateInfo6()
        {
            Tabler tabler = Control.Create<Tabler>();
            tabler.SetDefaultCellMargin(new Margin(0, 0, 30, 0));
            listViewer.AddChild(tabler, 50);


            Label selectLabel = Control.Create<Label>();
            selectLabel.CtrlSizeChangeMode = ControlSizeChangeMode.FitContentSize;
            selectLabel.Height = 25;
            selectLabel.Text = "强制比牌:";
            tabler.AddChild(selectLabel, 0, 0);
            tabler.SetCellMargin(0, 0, new Margin(10, 0, 10, 0));



            string[] name = new string[]
            {
                "180秒", "300秒", "无限制"
            };

            cmpcard = new RoundCheckBox[name.Length];
            for (int i = 0; i < name.Length; i++)
            {
                cmpcard[i] = Control.Create<RoundCheckBox>();
                cmpcard[i].MatchType = MatchType.None;
                cmpcard[i].CtrlSizeChangeMode = ControlSizeChangeMode.FitContentSize;
                cmpcard[i].Height = 25;
                cmpcard[i].Text = name[i];
                cmpcard[i].BindProcess = CmpCard;
                tabler.AddChild(cmpcard[i], 0, i + 1);
            }


            tabler.EnableTableLineAutoAdjustRichSize(0, true, LineDir.HORIZONTAL);

        }

        void CreateInfo7()
        {
            Tabler tabler = Control.Create<Tabler>();
            tabler.SetDefaultCellMargin(new Margin(0, 0, 30, 0));
            listViewer.AddChild(tabler, 80);

            Label costLabel = Control.Create<Label>();
            costLabel.CtrlSizeChangeMode = ControlSizeChangeMode.FitContentSize;
            costLabel.Height = 25;
            costLabel.Text = "房费:";
            tabler.AddChild(costLabel, 0, 0);
            tabler.SetCellMargin(0, 0, new Margin(10, 0, 10, 0));

            costinfo = Control.Create<CostInfo>();
            costinfo.MatchType = MatchType.None;
            costinfo.CtrlSizeChangeMode = ControlSizeChangeMode.FitContentSize;
            costinfo.Height = 25;
            tabler.AddChild(costinfo, 0, 1);

            tabler.EnableTableLineAutoAdjustRichSize(0, true, LineDir.HORIZONTAL);

            //
            tabler = Control.Create<Tabler>();
            listViewer.AddChild(tabler, 280);
        }


        void TrunProcess(Control control)
        {
            RoundCheckBox curtTurn = control as RoundCheckBox;

            if (curtTurn.IsCheck == true)
            {
                for (int i = 0; i < turn.Length; i++)
                {
                    if (turn[i] != curtTurn)
                        turn[i].IsCheck = false;
                }

                //数据设置
                for (int i = 0; i < turn.Length; i++)
                {
                    if (turn[i] == curtTurn)
                    {
                        if (i == 0)
                            settingData.turnCount = 10;
                        else if (i == 1)
                            settingData.turnCount = 20;
                        else
                            settingData.turnCount = 30;

                        SetRoomCostInfo();
                        break;
                    }
                }
            }
        }

        void JokerCountSelect(Control control)
        {
            RoundCheckBox curtJoker = control as RoundCheckBox;

            if (curtJoker.IsCheck == true)
            {

                for (int i = 0; i < joker.Length; i++)
                {
                    if (joker[i] != curtJoker)
                        joker[i].IsCheck = false;
                }

                //数据设置
                for (int i = 0; i < joker.Length; i++)
                {
                    if (joker[i] == curtJoker)
                    {
                        switch (i)
                        {
                            case 0: settingData.jokerCount = 0; break;
                            case 1: settingData.jokerCount = 2; break;
                            case 2: settingData.jokerCount = 4; break;
                            case 4: settingData.jokerCount = 6; break;
                            case 5: settingData.jokerCount = 8; break;
                        }
                        break;
                    }
                }
            }
        }

        void HighScore(Control control)
        {
            CheckBox highScore = control as CheckBox;
            settingData.isHighScoreMode = highScore.IsCheck;
        }

        void FreeMaPai(Control control)
        {
            CheckBox freeMaPai = control as CheckBox;
            settingData.isFreeMaPai = freeMaPai.IsCheck;
            tips.Text = "测试消息4";
        }

        void Optional(Control control)
        {
            RoundCheckBox curtOpt = control as RoundCheckBox;

            if (curtOpt.IsCheck == true)
            {
                if (curtOpt == optional[optional.Length - 1])
                {
                    tips.Text = "测试消息3";
                    suitSelecter.IsDisabled = false;
                    suitSelecter.SelectedSuit = Suits.Spade;
                }

                for (int i = 0; i < optional.Length; i++)
                {
                    if (optional[i] == curtOpt && i == 1)
                    {
                        tips.Text = "测试消息2";
                    }

                    if (optional[i] != curtOpt)
                    {
                        optional[i].IsCheck = false;

                        if (i == optional.Length - 1)
                        {
                            suitSelecter.SelectedSuit = Suits.None;
                            suitSelecter.IsDisabled = true;
                        }
                    }
                }

                //
                for (int i = 0; i < optional.Length; i++)
                {
                    if (optional[i] == curtOpt)
                    {
                        settingData.opt = i;

                        if (i == optional.Length - 1)
                            settingData.suit = suitSelecter.SelectedSuit;
                        else
                            settingData.suit = Suits.None;
                    }
                }
            }
        }

        void CostMode(Control control)
        {
            RoundCheckBox curtCost = control as RoundCheckBox;

            if (curtCost.IsCheck == true)
            {
                for (int i = 0; i < costmode.Length; i++)
                {
                    if (costmode[i] == curtCost && i == 1)
                    {
                        tips.Text = "测试消息1";
                    }

                    if (costmode[i] != curtCost)
                        costmode[i].IsCheck = false;
                }


                //
                for (int i = 0; i < costmode.Length; i++)
                {
                    if (costmode[i] == curtCost)
                    {
                        if (i == 0)
                            settingData.roomCostMode = 2;
                        else
                            settingData.roomCostMode = 1;


                        SetRoomCostInfo();
                    }
                }
            }
        }

        void CmpCard(Control control)
        {
            RoundCheckBox curtcmp = control as RoundCheckBox;

            if (curtcmp.IsCheck == true)
            {
                for (int i = 0; i < cmpcard.Length; i++)
                {
                    if (cmpcard[i] != curtcmp)
                        cmpcard[i].IsCheck = false;
                }


                //
                for (int i = 0; i < cmpcard.Length; i++)
                {
                    if (cmpcard[i] == curtcmp)
                    {
                        settingData.time = 180;
                    }
                }
            }
        }
    }
}
