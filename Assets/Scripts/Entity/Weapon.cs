using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    int projectileCount = 0;

    public int maxAmmo = 10;
    private int currentAmmo;
    public float reloadTime = 1.0f;
    private bool isReloading = false;

    float minSpread = 1.0f;
    float maxSpread = 5.0f;
    float currentSpread = 1.0f;

    bool shooting = false;
    bool readyToFire = false;
    float fireRate = .05f;
    float timeFromLastFire;

    float damage = 10;
    bool bulletDrop = true;

    float recoilValue = 5.0f;

    public bool Shooting {
        get { return shooting; }
        set { shooting = value; }
    }

    public float RecoilValue {
        get { return recoilValue; }
        set { recoilValue = value; }
    }

    void Start() {
        readyToFire = true;
        currentSpread = minSpread;
    }

    void Update() {
        ////if (shooting) {
        //     //currentSpread += ((maxSpread - minSpread) / spreadTimeToMax) * Time.deltaTime;
        ////}
        ////else {
        //    currentSpread -= ((maxSpread - minSpread) / spreadTimeToMin) * Time.deltaTime;
        ////}
        //currentSpread = Mathf.Clamp(currentSpread, minSpread, maxSpread);


        if (!readyToFire && !isReloading) {
            timeFromLastFire += Time.deltaTime;
            if (timeFromLastFire > fireRate) {
                readyToFire = true;
                timeFromLastFire = 0;
            }
        }

        if (currentAmmo <= 0 && !isReloading) {
            StartCoroutine(Reload());
            return;
        }
    }

    private IEnumerator Reload() {
        isReloading = true;
        Debug.Log("Reloading...");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
    }

    public void Shoot(Entity owner, Vector3 startingPosition, Vector2 startingFacing) {
        if (readyToFire) {
            GameObject bullet = new GameObject("Bullet");
            Destroy(bullet, 10.0f);

            bullet.transform.position = startingPosition;
            float angle = Mathf.Atan2(startingFacing.y, startingFacing.x) * Mathf.Rad2Deg;
            currentSpread = Random.Range(minSpread, maxSpread);
            angle += Random.Range(-currentSpread, currentSpread);
            bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            SpriteRenderer renderer = bullet.AddComponent<SpriteRenderer>();
            renderer.sprite = Resources.Load<Sprite>("Sprites/Entity/sci-fi-bullet");
            renderer.sortingLayerName = "Bullets";

            BoxCollider2D collider = bullet.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;

            Rigidbody2D rb2d = bullet.AddComponent<Rigidbody2D>();
            rb2d.gravityScale = 0;
            rb2d.velocity = bullet.transform.right * 20.0f;

            ProjectileData projectileData = bullet.AddComponent<ProjectileData>();
            projectileData.SetProjectileData(owner);
            projectileData.SetDamage(damage);
            projectileData.SetBulletDrop(bulletDrop);

            currentAmmo--;
            readyToFire = false;
        }
    }
}