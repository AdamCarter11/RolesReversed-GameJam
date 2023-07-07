using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAI : MonoBehaviour
{
    [SerializeField] GameObject enemyHero;
    [SerializeField] GameObject firingPoint;
    [SerializeField] GameObject bulletPrefab;
    private Transform target;
    [SerializeField] Rigidbody2D rb;
    public float rotateSpeed = .005f;

    public float fireRate;
    private float timeToFire;
    // Start is called before the first frame update
    void Start()
    {
        timeToFire = fireRate;
    }

    // Update is called once per frame
    void Update()
    {
        target = enemyHero.transform;
        RotateTowardsTarget();
        Shoot();
    }

    public void RotateTowardsTarget()
    {
        Vector2 targetDirection = target.position - transform.position;
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg - 90f;
        Quaternion q = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.localRotation = Quaternion.Slerp(transform.localRotation, q, rotateSpeed);
    }
    private void Shoot()
    {
        if(timeToFire <= 0f)
        {
            Instantiate(bulletPrefab, firingPoint.transform.position, firingPoint.transform.rotation);
            timeToFire = fireRate;
        }
        else
        {
            timeToFire -= Time.deltaTime;
        }
    }
    //public void ClusterAttack()
    //{

    //}

    
}
