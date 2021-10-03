using System.Collections;
using UnityEngine;

namespace CollectorGod
{
    public class JarSpawner : MonoBehaviour
    {
        public bool YBreak = false;
        public GameObject go = null;
        public int hp = 100;
        public float grivateScale = 0;
        public Vector2 v = Vector2.zero;
        SpawnJarControl sj = null;
        Rigidbody2D body = null;
        CircleCollider2D col = null;
        SpriteRenderer sprite = null;
        IEnumerator DoBreak()
        {
            sj.dustTrail.Stop();
            sj.ptBreakS.Play();
            sj.ptBreakL.Play();
            sj.strikeNailR.Spawn(transform.position);
            col.enabled = false;
            body.velocity = Vector2.zero;
            body.angularVelocity = 0f;
            sprite.enabled = false;
            sj.breakSound.SpawnAndPlayOneShot(sj.audioSourcePrefab, sj.transform.position);
            if (go != null)
            {
                GameObject g = Instantiate(go);
                g.transform.position = transform.position;
                Modding.Logger.Log("Spawn: " + g.name);
                g.SetActive(true);
                HealthManager hm = g.GetComponent<HealthManager>();
                if (hm != null)
                {
                    hm.hp = hp;
                }
                
            }

            yield return new WaitForSeconds(1.5f);
            Destroy(gameObject);
        }
        bool b = false;
        void DoTouch(GameObject go)
        {
            if ((go.layer == (int)GlobalEnums.PhysLayers.TERRAIN || go.transform.root.gameObject.name == "Knight") && !b && !YBreak)
            {
                b = true;
                StartCoroutine(DoBreak());
            }
        }
        void Update()
        {
            if (YBreak)
            {
                if(transform.position.y<= 94.55f && !b)
                {
                    b = true;
                    body.velocity = Vector2.zero;
                    body.gravityScale = 0;
                    transform.SetPositionY(94.55f);
                    StartCoroutine(DoBreak());
                }
            }
        }
        void Start()
        {
            
            sj = GetComponent<SpawnJarControl>();
            sj.StopAllCoroutines();
            body = GetComponent<Rigidbody2D>();
            sprite = GetComponent<SpriteRenderer>();
            col = GetComponent<CircleCollider2D>();
            if (sj == null)
            {
                Destroy(this);
                return;
            }
            col.enabled = true;
            body.velocity = v;
            body.gravityScale = grivateScale;
            body.angularVelocity =(Random.Range(0, 2) > 0) ? -300 : 300;
            sj.dustTrail.Play();
            sprite.enabled = true;
        }
        void OnTriggerEnter2D(Collider2D collider) => DoTouch(collider.gameObject);
        void OnCollisionEnter2D(Collision2D collision) => DoTouch(collision.gameObject);
    }
}