using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoppraGames
{
    public class Main : MonoBehaviour
    {
        public static Main instance;

        public GameObject MainMenu;
        public DailyRewardsWindow DailyRewardsWindow;
        public QuestWindow QuestWindow;
        public SpinWheelController SpinWheelWindow;


        void Awake()
        {
            instance = this;

            ShowQuestWindow(false);
            ShowSpinWheelWindow(false);
            ShowMainMenu(true);
        }

        public void OnClickDailyRewardsButton()
        {
            ShowQuestWindow(false);
            ShowSpinWheelWindow(false);
            ShowMainMenu(false);
        }

        public void OnClickQuestButton()
        {
            ShowQuestWindow(true);
            ShowSpinWheelWindow(false);
            ShowMainMenu(false);
        }

        public void OnClickSpinwheelButton()
        {
            ShowQuestWindow(false);
            ShowSpinWheelWindow(true);
            ShowMainMenu(false);
        }


        public void ShowMainMenu(bool isTrue)
        {
            if (MainMenu)
            {
                MainMenu.gameObject.SetActive(isTrue);
            }
        }



        //QUESTS OPTIONS
        public void ShowQuestWindow(bool isTrue)
        {
            if (QuestWindow)
            {
                QuestWindow.gameObject.SetActive(isTrue);

                if (isTrue)
                    QuestWindow.Init();
                else
                    ShowMainMenu(true);
            }
        }

        public void OnClickResetQuest()
        {
            QuestManager.instance.ResetAllDailyQuests();
        }

        //SPINWHEEL OPTIONS
        public void ShowSpinWheelWindow(bool isTrue)
        {
            if (SpinWheelWindow)
            {
                SpinWheelWindow.gameObject.SetActive(isTrue);

                if (isTrue)
                    SpinWheelWindow.Init();
                else
                    ShowMainMenu(true);
            }
        }

    }
}