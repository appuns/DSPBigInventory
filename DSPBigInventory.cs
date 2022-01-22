using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System;
using System.IO;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using xiaoye97;
using System.Security;
using System.Security.Permissions;

namespace DSPBigInventory
{

    [BepInPlugin("Appun.DSP.plugin.BigInventory", "DSPBigInventory", "1.0.2")]
    [BepInProcess("DSPGAME.exe")]

    public class DSPBigInventory : BaseUnityPlugin
    {
        public static ConfigEntry<int> colCount;
        public static ConfigEntry<int> rowCount;

        public static float originalWidth;
        public static float originalHeight;

        public void Start()
        {
            LogManager.Logger = Logger;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            colCount = Config.Bind("General", "colCount", 16, "Inventory Column Count");
            rowCount = Config.Bind("General", "rowCount", 14, "Inventory Row Count");


        }


        [HarmonyPatch(typeof(UIGame), "OpenPlayerInventory")]
        public static class UIGame_OpenPlayerInventory
        {
            [HarmonyPostfix]

            public static void Postfix(UIGame __instance)
            {
                GameMain.mainPlayer.package.SetSize(colCount.Value * rowCount.Value);
                UIRoot.instance.uiGame.inventory.colCount = colCount.Value;
               //RectTransform RT = UIRoot.instance.uiGame.inventory.GetComponent<RectTransform>();
                //RT.sizeDelta = new Vector2(RT.sizeDelta.x + 100, RT.sizeDelta.y);


            }
        }

        //[HarmonyPatch(typeof(UIGame), "_OnUpdate")]
        public static class UIGame_OnUpdate
        {
            [HarmonyPostfix]

            public static void Postfix(UIGame __instance)
            {
                UIRoot.instance.uiGame.inventory.colCount = colCount.Value;

            }
        }


    }
    public class LogManager
    {
        public static ManualLogSource Logger;
    }

}