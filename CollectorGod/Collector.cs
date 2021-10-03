using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ModCommon;
using ModCommon.Util;

namespace CollectorGod
{
    class Collector : MonoBehaviour
    {
        GameObject spawner = null;
        GameObject roar = null;
        GameObject spitter = null;
        tk2dSpriteAnimator anim = null;
        PlayMakerFSM ctrl = null;
        void Awake()
        {
            roar = gameObject.GetFSMActionOnState<CreateObject>("Roar").gameObject.Value;
            ctrl = gameObject.LocateMyFSM("Control");
            List<FsmState> states = ctrl.Fsm.States.ToList();
            states.Add(new FsmState(ctrl.Fsm)
            {
                Name = "Ext Attack"
            });
            ctrl.Fsm.States = states.ToArray();

            ctrl.InsertMethod("Lunge Swipe", 0, () =>
            {
                Modding.Logger.Log("Catch: Lunge Swipe");
                ctrl.SetState("Ext Attack");
                ctrl.FsmVariables.FindFsmGameObject("Lunge Hit").Value.SetActive(false);
                int r = UnityEngine.Random.Range(0, 5);
                if (r < 3)
                {
                    StartCoroutine(ShotExp());
                }
                else
                {
                    StartCoroutine(RoarFall());
                }
            });
            ctrl.InsertMethod("Stun", 0, () =>
            {
                for (int i = 0; i < UnityEngine.Random.Range(0, 3); i++)
                {
                    float x = UnityEngine.Random.Range(42.94f, 65.90f);
                    GameObject go = Instantiate(spawner);
                    go.transform.SetPositionX(x);
                    Fire(CollectorGodMod.exp, 0, new Vector2(x, 106.52f), new Vector2(0, -25), 1, true);
                }
            });
            ctrl.InsertMethod("Summon?", 0, () =>
            {
                Modding.Logger.Log("Catach: Summon?");
                if (UnityEngine.Random.Range(0, 8) < 3)
                {
                    Modding.Logger.Log("Roof");
                    ctrl.SetState("Ext Attack");
                    StartCoroutine(Roof());
                }
            });

            spitter = ctrl.FsmVariables.FindFsmGameObject("Spitter Prefab").Value;

            anim = GetComponent<tk2dSpriteAnimator>();
            spawner = gameObject.GetFSMActionOnState<SpawnObjectFromGlobalPool>("Spawn").gameObject.Value;
        }
        IEnumerator Roof()
        {
            Modding.Logger.Log("Collector: Roof");
            yield return new WaitForSeconds(0.5f);
            int count = UnityEngine.Random.Range(5, 15);
            for(int i = 0; i < count; i++)
            {
                yield return new WaitForSeconds(0.35f);
                float x = Mathf.Clamp(HeroController.instance.transform.position.x + UnityEngine.Random.Range(-3, 3), 43, 65);
                yield return new WaitForSeconds(0.55f);
                Fire(CollectorGodMod.exp, 0, new Vector2(x, 106.52f), new Vector2(0, -25), 1, true);
            }
            yield return new WaitForSeconds(0.75f);
            ctrl.SetState("Return Antic");
        }
        IEnumerator RoarFall()
        {
            ctrl.FsmVariables.FindFsmGameObject("Lunge Hit").Value.SetActive(false);
            Modding.Logger.Log("Collector: Roar Fall");
            
            yield return anim.PlayAnimWait("Antic");
            GameObject r = Instantiate(roar);
            r.transform.parent = ctrl.FsmVariables.FindFsmGameObject("Roar Point").Value.transform;
            r.transform.localPosition = Vector2.zero;
            anim.Play("Roar");
            for(int i = 0; i < UnityEngine.Random.Range(1, 8); i++)
            {
                float x = UnityEngine.Random.Range(42.94f, 65.90f);
                GameObject go = Instantiate(spawner);
                go.transform.SetPositionX(x);
                go.GetComponent<SpawnJarControl>().SetEnemySpawn(spitter, 30);
            }
            yield return new WaitForSeconds(3);
            FSMUtility.SendEventToGameObject(r, "END");
            yield return anim.PlayAnimWait("Land");
            ctrl.FsmVariables.FindFsmGameObject("Lunge Hit").Value.SetActive(false);
            ctrl.SetState("Hop Start Antic");
        }
        IEnumerator ShotExp()
        {
            Modding.Logger.Log("Collector: Shot Exp");
            yield return Shot(CollectorGodMod.exp, 0);
            ctrl.FsmVariables.FindFsmGameObject("Lunge Hit").Value.SetActive(false);
        }
        IEnumerator Shot(GameObject go, int hp)
        {
            Vector2 v;
            if (transform.position.x < HeroController.instance.transform.position.x)
            {
                transform.SetScaleX(-1);
                v = new Vector2(25, 0);
            }
            else
            {
                transform.SetScaleX(1);
                v = new Vector2(-25, 0);
            }
            Fire(go, hp, transform.position, v, 0, false);
            yield return anim.PlayAnimWait("Lunge Slash");
            ctrl.SetState("Lunge Recover");
        }
        void Fire(GameObject g,int hp,Vector2 spawn,Vector2 v,float gs, bool YO)
        {
            GameObject go = Instantiate(spawner);
            go.transform.position = spawn;
            var s = go.AddComponent<JarSpawner>();
            s.go = g;
            s.hp = hp;
            s.grivateScale = gs;
            s.v = v;
            s.YBreak = YO;
        }
    }
}
