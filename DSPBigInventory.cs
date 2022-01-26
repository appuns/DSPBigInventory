using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System;
using System.IO;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using static UnityEngine.GUILayout;
using UnityEngine.Rendering;
using Steamworks;
using rail;
using xiaoye97;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]


namespace DSPBigInventory
{

    [BepInPlugin("Appun.DSP.plugin.BigInventory", "DSPBigInventory", "1.0.4")]
    [BepInProcess("DSPGAME.exe")]

    [HarmonyPatch]

    public class DSPBigInventory : BaseUnityPlugin
    {
        public static ConfigEntry<int> colCount;
        public static ConfigEntry<int> rowCount;

        public void Start()
        {
            LogManager.Logger = Logger;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            colCount = Config.Bind("General", "colCount", 16, "Inventory Column Count");
            rowCount = Config.Bind("General", "rowCount", 14, "Inventory Row Count");

        }

        //ロードしたらストレージサイズを修正
        [HarmonyPostfix, HarmonyPatch(typeof(Player), "Import")]
        public static void Mecha_Import_Postfix(Player __instance)
        {
            __instance.package.SetSize(colCount.Value * rowCount.Value);
        }

        //新規ゲームを開始したらストレージサイズを修正
        [HarmonyPostfix, HarmonyPatch(typeof(Player), "SetForNewGame")]
        public static void Mecha_SetForNewGame_Postfix(Player __instance)
        {
            __instance.package.SetSize(colCount.Value * rowCount.Value);
        }

        //テックが解除されたとき
        [HarmonyPrefix, HarmonyPatch(typeof(GameHistoryData), "UnlockTechFunction")]
        public static bool GameHistoryData_UnlockTechFunction_Prefix(int func)
        {
            if(func == 5)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        //プレーヤーインベントリを開いたとき
        [HarmonyPostfix,HarmonyPatch(typeof(UIGame), "OpenPlayerInventory")]
        public static void UIGame_OpenPlayerInventory_Postfix(UIGame __instance)
        {
            UIRoot.instance.uiGame.inventory.colCount = colCount.Value;
        }

        //組み立て機等を開いたとき
        [HarmonyPrefix,HarmonyPatch(typeof(UIAssemblerWindow), "OpenPlayerPackage")]
        public static bool UIAssemblerWindow_OpenPlayerPackage_Prefix(UIAssemblerWindow __instance)
        {
       
            if (!__instance.playerInventory.inited)
            {
                __instance.playerInventory.colCount = colCount.Value;
                __instance.playerInventory._Init(__instance.player.package);
                __instance.playerInventory._Open();
                __instance.windowTrans.sizeDelta = new Vector2((float)(colCount.Value * 50 + 80), (float)(rowCount.Value * 50 + 220));
            }
            return false;

        }

        //採掘機を開いたとき
        [HarmonyPrefix,HarmonyPatch(typeof(UIMinerWindow), "_OnOpen")]
        public static bool UIMinerWindow_OnOpen_Prefix(UIMinerWindow __instance)
        {
            if (GameMain.localPlanet != null && GameMain.localPlanet.factory != null)
            {
                //__instance.player.package.SetSize(colCount.Value * rowCount.Value);
                __instance.playerInventory.colCount = colCount.Value;
                //__instance.playerInventory.rowCount = rowCount.Value;
                __instance.windowTrans.sizeDelta = new Vector2((float)(colCount.Value * 50 + 80), (float)(rowCount.Value * 50 + 220));
            }
            return true;
        }

    }
    public class LogManager
    {
        public static ManualLogSource Logger;
    }

}