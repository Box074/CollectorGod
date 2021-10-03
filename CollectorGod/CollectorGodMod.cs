using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modding;
using UnityEngine;

namespace CollectorGod
{
    public class CollectorGodMod : Mod
    {
        public static GameObject exp = null;
        public override void Initialize(Dictionary<string,Dictionary<string,GameObject>> gos)
        {
            On.SpawnJarControl.Behaviour += SpawnJarControl_Behaviour;
            exp = UnityEngine.Object.Instantiate( Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(
                x => x.name == "Gas Explosion Recycle L"
                ));
            exp.transform.parent = null;
            exp.SetActive(false);
            UnityEngine.Object.DontDestroyOnLoad(exp);
            UnityEngine.Object.Destroy(exp.LocateMyFSM("damages_enemy"));

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        }

        private void SceneManager_activeSceneChanged(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.Scene arg1)
        {
            foreach(var v in UnityEngine.Object.FindObjectsOfType<GameObject>().Where(x => x.name == "Jar Collector"))
            {
                if (v.GetComponent<Collector>() == null) v.AddComponent<Collector>();
            }
        }

        private System.Collections.IEnumerator SpawnJarControl_Behaviour(On.SpawnJarControl.orig_Behaviour orig, SpawnJarControl self)
        {
            if (self.GetComponent<JarSpawner>() != null)
            {
                yield break;
            }
            yield return orig(self);
        }
    }
}
