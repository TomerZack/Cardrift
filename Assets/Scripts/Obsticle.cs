using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obsticle : MonoBehaviour
{
    public float speed = 0;
    public float side_speed = 0;
    public int damage;
    public bool destroy = false;
    public float mSide_speed  = 1;
    public float rot = 0;
    public GameObject Explosion;
    float ExpTimer = -2;
    public string explosionOrigin;
    public void changeSpeed(int x)
    {
        speed = x;
    }
    public void changeSideSpeed(int x)
    {
        side_speed = x;
    }
    void Start()
    {
        GetComponent<Rigidbody2D>().AddForce(new Vector2(side_speed * mSide_speed, speed), ForceMode2D.Impulse);
        speed = 0;
        side_speed = 0;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (gameObject.name == "Obsticle Missle Variant(Clone)")
        {
            if (GetComponent<Transform>().position.y <= -9 && GetComponent<Rigidbody2D>().velocity.y < -10)
            {
                GetComponent<ParticleSystem>().Stop();
                speed = 15f;
            }
            if (GetComponent<Transform>().position.y > -9 && GetComponent<Rigidbody2D>().velocity.y > -15f)
            {
                GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -100f));
            }
            
        }
        if (gameObject.name == "Obsticle Burning Tire Variant(Clone)"&& GetComponent<BoxCollider2D>().enabled)
        {
            if (Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x) < 1.5) GetComponent<Animator>().Play("fallback");
        }
        if (side_speed != 0 || speed != 0)
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(side_speed * mSide_speed, speed), ForceMode2D.Impulse);
            speed = 0;
            side_speed = 0;
        }
        GetComponent<Rigidbody2D>().MoveRotation(rot);
    }
    void Update()
    {
        if (GetComponent<Transform>().position.y <= -15)
        {
            Destroy(gameObject);
        }
        if (destroy) Destroy(gameObject);
        if (ExpTimer > 0)
        {
            ExpTimer -= Time.deltaTime;
        }
        if (ExpTimer < 0 && ExpTimer > -1) Explode();
    }
    public void Explode()
    {
        GameObject obj = Instantiate<GameObject>(Explosion, transform.position, Quaternion.identity, GetComponentInParent<TilemapControl>().transform);
        obj.GetComponentInChildren<Obsticle>().explosionOrigin = gameObject.name;
        Destroy(gameObject);
    }
    public void Explode(float t)
    {
        ExpTimer = t;
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameObject.name == "Obsticle Missle Variant(Clone)") Explode();
    }
}
