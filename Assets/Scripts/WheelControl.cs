using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelControl : MonoBehaviour
{
    public float speed = 5;
    public float side_speed = 0;
    public float height = 0;
    public GameObject laser;
    public Transform shadow;
    public bool endCard = false;
    int health = 120;
    int maxHealth = 120;
    public HealthBar bar;
    public float mirror = 2;
    public SpriteRenderer reflection;
    public GameObject wheelCopy;
    public GameObject sphere_effector;
    public GameObject ray_effector;
    public bool isTouchingBorders = false;
    public bool isPortal = false;
    public float portalTimer = -1f;

    public void throughPortal(GameObject portal)
    {
        Vector2 p = GetComponent<Transform>().position;
        p.x *= -1;
        transform.position = p;
        Vector2 T = wheelCopy.transform.position;
        T.x = p.x * -1;
        wheelCopy.transform.position = T;
        isPortal = true;
        
    }

    private void FixedUpdate()
    {
        if (portalTimer > 0) portalTimer -= Time.fixedDeltaTime;
        if (portalTimer <0&&-1< portalTimer)
        {
            portalTimer = -1;
            GetComponentInParent<GameControl>().portal(false);
            foreach (BoxCollider2D t in GetComponentInParent<GameControl>().GetComponents<BoxCollider2D>()) t.enabled = true;
            isPortal = false;
        }
        wheelCopy.SetActive(Mathf.Abs(transform.position.x) > 4&&portalTimer >0);
        if (Mathf.Abs(transform.position.x) <= 4.4 && isPortal)
        {
            isPortal = false;
        }
        if (Mathf.Abs(transform.position.x) > 4.5 && !isPortal) throughPortal(gameObject);
        Vector2 p = GetComponent<Transform>().position;
        if (isTouchingBorders&& side_speed > 0 && transform.position.x > 0) side_speed = 0;
        if (isTouchingBorders && side_speed < 0 && transform.position.x < 0) side_speed = 0;
        p.x = p.x + side_speed * Time.fixedDeltaTime;
        p.y = -2 + height;

        GetComponentInParent<GameControl>().speed = speed;
        if (endCard == true)
        {
            isPortal = false;
            endCard = false;
            p.x = Mathf.Round(p.x);

        }
        if (mirror <= 0)
        {
            Vector2 pR = reflection.GetComponent<Transform>().position;
            pR.x = p.x * 3;
            p.x = p.x * -1;
            mirror = 2;
            reflection.GetComponent<Transform>().position = pR;
        }
        GetComponent<Transform>().position = p;
        if (mirror <= 1) mirror = mirror - Time.fixedDeltaTime;
        if (health <= 0)
        {
            GetComponentInParent<GameControl>().confirmLoss();
        }
        Vector3 shp = shadow.position;
        shp.y = -2.2f;
        shadow.position = shp;
        if (transform.position.x >0)
        {
            Vector3 tr = wheelCopy.transform.position;
                tr.x = transform.position.x - 9;
            wheelCopy.transform.position = tr;
        }
        else
        {
            Vector3 tr = wheelCopy.transform.position;
                tr.x = transform.position.x + 9;
            wheelCopy.transform.position = tr;
        }
        wheelCopy.GetComponent<Animator>().runtimeAnimatorController = GetComponent<Animator>().runtimeAnimatorController;
    }
    public void shootLaser()
    {
        Instantiate(laser,new Vector3(transform.position.x, transform.position.y+0.5f),Quaternion.identity, GetComponent<Transform>());
    }
    public void acticateCard(string card)
    {
        GetComponent<Animator>().Play(card);
        if (card == "card laser once Variant(Clone)")
            shootLaser();
        if (card == "card mirror Variant(Clone)")
        {
            mirror = 0.333333f;
            Vector2 pR = reflection.GetComponent<Transform>().position;
            pR.x = GetComponent<Transform>().position.x * -1;
            reflection.GetComponent<Transform>().position = pR;
        }
        if (card == "card blow Variant(Clone)")
        {
            blow();
        }
        if (card == "card switch Variant(Clone)")
        {
            if (transform.position.x < 0) GetComponent<Animator>().Play("card switch right");
            if (transform.position.x > 0) GetComponent<Animator>().Play("card switch left");
        }
        if (card == "card portals Variant(Clone)")
        {
            Debug.Log("portals!");
            GetComponentInParent<GameControl>().portal(true);
            foreach (BoxCollider2D t in GetComponentInParent<GameControl>().GetComponents<BoxCollider2D>()) t.enabled = false;
            portalTimer = 12;
        }
    }
    public void subHealth(int d, GameObject obs )
    {
        if (obs.name == "Obsticle Cone Variant(Clone)")
        {
            obs.GetComponent<SpriteRenderer>().sortingOrder = 8;
            if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsTag("thrust"))
            {
                obs.GetComponent<Animator>().Play("fallback");
                obs.GetComponent<Obsticle>().changeSpeed(13);
                obs.GetComponent<Obsticle>().changeSideSpeed(4);
                return;
            }
            else
            {
                GetComponent<Animator>().Play("DriveOverObsticle", 1);
                obs.GetComponent<Animator>().Play("fallback");
                if (side_speed > 0)
                {
                    obs.GetComponent<Obsticle>().rot = -45;
                    obs.GetComponent<Rigidbody2D>().AddForce(new Vector2(0.5f, 0.5f), ForceMode2D.Impulse);
                }
                else if (side_speed < 0)
                {
                    obs.GetComponent<Obsticle>().rot = 45;
                    obs.GetComponent<Rigidbody2D>().AddForce(new Vector2(-0.5f, 0.5f), ForceMode2D.Impulse);
                }
                else obs.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 0.5f), ForceMode2D.Impulse);
            }
        }
        if (obs.gameObject.name == "Explosion Damage")
        {
            Debug.Log(Vector2.Distance(transform.position, obs.transform.position));
            health = health - Mathf.RoundToInt(d * (1 / Vector2.Distance(transform.position, obs.transform.position)) / 5) * 5;
            Debug.Log(Mathf.RoundToInt(d * (1 / Vector2.Distance(transform.position, obs.transform.position)) / 5) * 5);
            GetComponentInParent<GameControl>().addDamageToCount(obs, Mathf.RoundToInt(d * (1 / Vector2.Distance(transform.position, obs.transform.position)) / 5) * 5);
        }
        else
        {
            health = health - d;
            GetComponentInParent<GameControl>().addDamageToCount(obs,d);
            obs.GetComponent<SpriteRenderer>().sortingOrder = 8;
        }
        bar.changeBar(maxHealth, health);
        if (d > 0) bar.GetComponentInChildren<Animator>().Play("Lost_Health");
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        try
            {
            if (collision.gameObject.GetComponent<GameControl>() != null) isTouchingBorders = true;
                if (collision.IsTouching(GetComponent<BoxCollider2D>()))
                {
                if (collision.gameObject.tag == "Explodable" && collision.IsTouching(GetComponent<BoxCollider2D>()))
                {
                    collision.GetComponent<Obsticle>().Explode();
                }

                    if (collision.gameObject.name == "Coin Variant(Clone)")
                    {
                        GameControl.coins++;
                        collision.gameObject.GetComponent<Animator>().Play("Destroy");
                    }
                    if (collision.gameObject.name == "Coins Bag Variant(Clone)")
                {
                    GameControl.coins += 10;
                    collision.gameObject.GetComponent<Animator>().Play("Destroy");
                }
                    else
                    {
                        GameControl.ObsticlesColided++;
                        if (collision.gameObject.name == "Obsticle Cone Variant(Clone)") collision.gameObject.GetComponent<Obsticle>().mSide_speed = 1 + (-2 * Random.Range(0, 2));
                        subHealth(collision.gameObject.GetComponent<Obsticle>().damage, collision.gameObject);
                        collision.gameObject.GetComponent<Obsticle>().damage = 0;
                    }
                }
            }
            catch (System.Exception e) { }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<GameControl>() != null) isTouchingBorders = false;
    }
    public void blow()
    {

            GameObject obj = Instantiate(sphere_effector, GetComponent<Transform>().position,Quaternion.identity ,GetComponent<Transform>());
            obj.GetComponent<SphericalColider>().blow();
    }
    public void blowDirectional(int di)
    {
        GameObject obj = Instantiate(ray_effector, GetComponent<Transform>().position, Quaternion.identity, GetComponent<Transform>());
        obj.GetComponent<Transform>().Rotate(0, 0, 90 + di);
        obj.GetComponent<RayColider>().blow(di);
    }
    public void resetHealth()
    {
        health = maxHealth/2;
        bar.changeBar(maxHealth, health);
    }
}
