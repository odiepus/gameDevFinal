using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileData : MonoBehaviour {

    private Entity owner;
    private Rigidbody2D rb2d;
    private float damage;
    private bool bulletDrop;

    void Start() {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        if (bulletDrop) {
            Vector2 velocity = rb2d.velocity;
            velocity.y += -.5f * Time.deltaTime;
            rb2d.velocity = velocity;
        }
    }

    public void SetProjectileData(Entity owner) {
        this.owner = owner;
    }

    public void SetDamage(float damage) {
        this.damage = damage;
    }

    public void SetBulletDrop(bool bulletDrop) {
        this.bulletDrop = bulletDrop;
    }

    void OnTriggerEnter2D(Collider2D other) {
        Entity hit = other.GetComponent<Entity>();
        if (other.tag == "Enviroment" && other.tag != "StairsEnviroment") {
            Destroy(gameObject);
        }
        else if (hit != null && hit != owner) {
            if (hit.Attackable) {
                hit.modHealth(-damage);
                Debug.Log("I, " + hit.name + " have been hit by " + owner.name + "'s bullet.");
                Destroy(gameObject);
            }
        }
    }
}
