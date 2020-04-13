using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Extend
{

    public static class GameUtils
    {

        public static string CombinePath(params string[] args)
        {
            return string.Join(Separator + "", args);
        }

        public static char Separator
        {
            get
            {
#if UNITY_EDITOR_WIN
                return '/';
#else
                return System.IO.Path.DirectorySeparatorChar;
#endif
            }
        }

        public static string GetStreamingAssetsFilePath(string subfoldername, string filename)
        {
            string sub = "";
            if (!string.IsNullOrEmpty(filename))
            {
                sub = filename + "/";
            }
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return "jar:file://" + Application.dataPath + "!/assets/" + sub + subfoldername;
                case RuntimePlatform.IPhonePlayer:
                    return "file://" + Application.dataPath + "/Raw/" + sub + subfoldername;
                default:
                    return "file://" + Application.streamingAssetsPath + "/" + sub + subfoldername;
            }
        }

        public static float GetTimer(bool ignoreTimeScale = false)
        {
            if (ignoreTimeScale)
            {
                return Time.realtimeSinceStartup;
            }
            return Time.time;
        }

        public static int GetMillisecond(bool ignoreTimeScale = false)
        {
            float time = GetTimer(ignoreTimeScale);
            return (int)(time * 1000);
        }

        public static float GetDeltaTime()
        {
            return Time.deltaTime;
        }

        public static double GetTimeStampMillsecond()
        {
            System.TimeSpan dateTime = System.DateTime.UtcNow - new System.DateTime(1970, 1, 1, 0, 0, 0);
            return dateTime.TotalMilliseconds;
        }

        public static double GetTimeStampSecond()
        {
            System.TimeSpan dateTime = System.DateTime.UtcNow - new System.DateTime(1970, 1, 1, 0, 0, 0);
            return dateTime.TotalSeconds;
        }

        public static Transform FindChild(Transform root, string name)
        {
            foreach (Transform t in root.transform)
            {
                if (t.name.CompareTo(name) == 0)
                {
                    return t;
                }
                FindChild(t, name);
            }
            return null;
        }

        static Color redColor = new Color32(255, 62, 154, 255); // new Color32(209, 0, 255, 255);//
        static Color greenColor = new Color32(137, 227, 35, 255);
        static Color blueColor = new Color32(95, 121, 212, 255);
        static Color32 redColor32 = new Color32(255, 62, 154, 255); // new Color32(209, 0, 255, 255);//
        static Color32 greenColor32 = new Color32(137, 227, 35, 255);
        static Color32 redColor_light = new Color32(255, 142, 196, 255);
        static Color32 greenColor_light = new Color32(154, 255, 42, 255);
        static Color32 redColor_dark = new Color32(101, 16, 56, 255);
        static Color32 greenColor_dark = new Color32(53, 82, 19, 255);
        //public static Color TranslateToColor(int groupId)
        //{

        //    int a = 0;
        //    if (a == 0)
        //    {
        //        return SpriteCampProxy.GetInstance().GetCampInfo(groupId).campColor;
        //    }

        //    switch (groupId)
        //    {
        //        case 1:// GameConstants.RED_TEAM:
        //            return redColor;
        //        case 0:// GameConstants.GREEN_TEAM:
        //            return greenColor;
        //        default:
        //            return blueColor;
        //    }
        //}

        //public static Color32 TranslateToColor32(int color)
        //{
        //    return color == 1 ? redColor32 : greenColor32;
        //}

        public static Color32 TranslateToLightColor(int groupId)
        {
            switch (groupId)
            {
                case 1:// GameConstants.RED_TEAM:
                    return redColor_light;
                case 0: //GameConstants.GREEN_TEAM:
                    return greenColor_light;
                default:
                    return blueColor;
            }
        }

        public static Color32 TranslateToDarkColor(int groupId)
        {
            switch (groupId)
            {
                case 1:// GameConstants.RED_TEAM:
                    return redColor_dark;
                case 0: //GameConstants.GREEN_TEAM:
                    return greenColor_dark;
                default:
                    return blueColor;
            }
        }
        public static string ModelName(int teamGroup)
        {
            return teamGroup == 2 ? "BigStrong_mod" : "little_girl_mod";
            //return teamGroup == 0 ? "BigStrong_mod" : "little_girl_mod";
        }

        public static Vector2 TranslateToDouDimenSuffix(int singledimen, int length)
        {
            return new Vector2(singledimen / length, singledimen % length);
        }

    }

}
