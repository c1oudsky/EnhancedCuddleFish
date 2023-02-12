using System.Reflection;
using HarmonyLib;
using UnityEngine;
using BepInEx;

namespace SN_EnhancedCuddleFishBepInEx
{
    [BepInPlugin("com.C1oudS.EnhancedCuddleFishBepInEx", "EnhancedCuddleFishBepInEx", "1.1")]
    public class MyPlugin : BaseUnityPlugin
    {
        private void Start()
        {
            var harmony = new Harmony("com.C1oudS.EnhancedCuddleFishBepInEx");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    class EnhancedCuddlefishPatch
    {
        [HarmonyPatch(typeof(CuteFish))]
        [HarmonyPatch("Start")]
        internal class PatchCFishStart
        {
            [HarmonyPostfix]
            public static void Postfix(CuteFish __instance)
            {
               __instance.gameObject.AddComponent<EnhancedCuddlefish>();

            }
        }
        //
        [HarmonyPatch(typeof(CuteFishHandTarget))]
        [HarmonyPatch("PrepareCinematicMode")]
        internal class HealFish
        {
            [HarmonyPostfix]
            public static void Postfix(CuteFishHandTarget __instance)
            {
                var livemixin = __instance.cuteFish.gameObject.GetComponent<LiveMixin>();
                livemixin.health = livemixin.maxHealth;
                /*
                FMODUWE.PlayOneShot([FMODAsset or string path?], __instance.gameObject.transform, 1f);
                                                  ↑↑↑
                seems FMODAsset is a scriptableobject, though that method also supports a path passed as string, whatever it means...
                Feel free to share your ways to make it sound.
                */
            }
        }
    }

    class EnhancedCuddlefish : MonoBehaviour
    {
        private LiveMixin livemixin;
        private PingInstance indicator;
        public void Start()
        {
            if(!this.gameObject.TryGetComponent<PingInstance>(out indicator)) indicator = this.gameObject.AddComponent<PingInstance>();
            indicator.SetLabel(Language.main.Get("Cutefish"));
            indicator.pingType = PingType.Signal; /*seems icon is passed as a sprite into uGUI_Pings : OnIconChange(PingInstance) {
            ...blah-blah...
            uGUI_Ping.SetIcon(SpriteManager.Group.Pings, PingManager.sCachedPingTypeStrings.Get(instance.pingType)))
            }
            So I guess you'd need to create and load a sprite asset to make a custom icon.
            */
            indicator.origin = this.gameObject.transform;
            livemixin = this.gameObject.GetComponent<LiveMixin>();
        }

        public void Update()
        {
            string line = Language.main.Get("Cutefish") + " [" + livemixin.health + "/" + livemixin.maxHealth + "]";
            indicator.SetLabel(line);
        }
    }
}
