using System;
using System.Collections.Generic;
using Bizza.Library;

namespace Bizza.Channel.Analytics
{
    public class Manager
    {
        public static event Action<string, Dictionary<string, object>> OnCustomEvent;

        public static void SendCustomEvent(string eventName, Dictionary<string, object> @params = null)
        {
            @params ??= new Dictionary<string, object>();
            // todo public param
            //var instance = GameDataModule.Instance;
            //if (instance)
            //{
            //    @params.Add("totalLogInDay", instance.PlayerData.analyticsData.totalLoginDay.ToString());
            //}

            // todo event
            OnCustomEvent?.Invoke(eventName, @params);
            
            string content = string.Concat("send custom event \nkey::: ", eventName, "\nparams::: ", @params.ToPairString());
            // LogUtil.VerboseD(content);
        }

        public static void SendGameStartEvent()
        {
            //// todo gamestart
            //var gameplay = World.Current.GetGameMode<GameMode_GamePlay>();
            //var data = GameDataModule.Instance;
            //if (gameplay == null || data == null) { return; }
            //var playerData = data.PlayerData;

            //int level = gameplay.CurChapter;
            //string levelName = level.ToString();

            //string planeId = playerData.curCharacter;

            //int talentLevel = playerData.talentData.activeIdx;

            //playerData.analyticsData.levelStartCount.Complete(level + 1);
            //playerData.analyticsData.levelStartCount[level]++;
            //int count = playerData.analyticsData.levelStartCount[level];

            //var @params = new Dictionary<string, object>()
            //{
            //    { "level", levelName },
            //    { "planeId", planeId },
            //    { "levelStartCount" , count.ToString() },
            //    { "Talentlevel", talentLevel.ToString() },
            //};

            //// todo voodoo
            //TinySauceManager.SendGameStartEvent(levelName);

            //// todo custom
            //SendCustomEvent(EventName.GameStart, @params);
        }

        public static void SendGameFinishEvent(bool success)
        {
            //var gameplay = World.Current.GetGameMode<GameMode_GamePlay>();
            //var character = GameModeSys_Character.Instance;
            //var levelCrtl = GameModeSys_Level.Instance;
            //var data = GameDataModule.Instance;
            //if (gameplay == null || character == null || data == null || levelCrtl == null) { return; }
            //var playerData = data.PlayerData;

            //int level = gameplay.CurChapter;
            //string levelName = level.ToString();
            //int wave = gameplay.CurRoomIdx;

            //string planeId = playerData.curCharacter;

            //int talentLevel = playerData.talentData.activeIdx;

            //playerData.analyticsData.levelStartCount.Complete(level + 1);
            //int count = playerData.analyticsData.levelStartCount[level];

            //var player = character.MainActor;
            //int second = (int)levelCrtl.RoomTimeSinceStartup;
            //// second = second / 10 * 10;

            //var @params = new Dictionary<string, object>()
            //{
            //    { "level", levelName },
            //    { "planeId", planeId },
            //    { "levelStartCount" , count.ToString() },
            //    { "talentLevel", talentLevel.ToString() },
            //    { "hp", player.Health.ToString() },
            //    { "time", second.ToString() },
            //    { "wave", wave.ToString() },
            //};

            //// todo voodoo
            //TinySauceManager.SendGameFinishEvent(levelName, success, levelCrtl.CurLevelKillEnemyNum);

            //if (success)
            //{
            //    SendCustomEvent(EventName.GameWin, @params);
            //}
            //else
            //{
            //    SendCustomEvent(EventName.GameLose, @params);
            //}
        }

        public static void ShowViewEvent(string viewName)
        {
            string eventName = "Show_" + viewName;
            SendCustomEvent(eventName);
        }

        public static void ShowTeachEvent(int step)
        {
            string eventName = EventName.Teach;
            var @params = new Dictionary<string, object>()
            {
                { ParamName.Step, step },
            };
            SendCustomEvent(eventName, @params);
        }

        public static void SendSwitchEvent(string eventName, bool on)
        {
            var @params = new Dictionary<string, object>()
            {
                { ParamName.On, on.ToString() },
            };
            SendCustomEvent(eventName, @params);
        }
    }

    public static class EventName
    {
        public const string Teach = "Teach";
        public const string GameStart = "Game_Start";
        public const string GameWin = "Game_Win";
        public const string GameLose = "Game_Lose";
        public const string SelectPlane = "Select_Plane";
        public const string SelectMainPage = "Select_Main_Page";
        public const string UpgradeTalentSuccess = "Upgrade_Talent_Success";
        public const string UpgradeTalentFail = "Upgrade_Talent_Fail";
        public const string ClickCard = "Click_Card";
        public const string MusicSwitch = "Music_Switch";
        public const string SoundSwitch = "Sound_Switch";
        public const string VibrateSwitch = "Vibrate_Switch";
        public const string SelectLanguage = "Select_Language";
        public const string CollectCard = "Collect_Card";
        public const string GameFPS = "Game_FPS";
    }

    public static class ParamName
    {
        public const string Step = "step";
        public const string On = "on";
    }
}

