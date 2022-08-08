using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphericalColider : MonoBehaviour
{
    public float lifetime = 5;
    public GameObject Blow;
    public int mode = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lifetime = lifetime - Time.deltaTime;
        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }
    public void blow()
    {
        mode = 1;
        for (int i = 0; i <= 8; i++)
        {
            GameObject blow = Instantiate(Blow, GetComponent<Transform>().position, Quaternion.Euler(0, 0, Random.Range(0, 360)), GetComponent<Transform>());
            Vector2 v = blow.GetComponent<Transform>().right;
            float j = Random.Range(-9.5f, -8.5f);
            v.Set(v.x * j, v.y * j);
            blow.GetComponent<Rigidbody2D>().AddForce(v, ForceMode2D.Impulse);
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (mode == 2 && collision.gameObject.tag == "Explodable") collision.GetComponent<Obsticle>().Explode(0.2f);
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if ((mode == 1||mode == 2)&&collision.gameObject.GetComponentInParent<WheelControl>() == null)
        {
            if (collision.attachedRigidbody.velocity.y > 0)
            {
                collision.GetComponent<Animator>().Play("fallback");
                if (collision.attachedRigidbody.velocity.x > collision.attachedRigidbody.velocity.y && collision.name == "Obsticle Cone Variant(Clone)")
                    collision.GetComponent<Obsticle>().rot = -45;
            }
            else
            {
                collision.GetComponent<Animator>().Play("fallforward");
                if (collision.attachedRigidbody.velocity.x < collision.attachedRigidbody.velocity.y && collision.name == "Obsticle Cone Variant(Clone)")
                    collision.GetComponent<Obsticle>().rot = -45;
            }
        }
    }
}
