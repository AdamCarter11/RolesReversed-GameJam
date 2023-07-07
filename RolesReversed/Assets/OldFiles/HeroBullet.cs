using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class HeroBullet : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float speed;
    [SerializeField] GameObject bulletPrefab;
    private float lifetime = 3f;
    [SerializeField] int clusterAmount;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        rb.velocity = transform.up * speed * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag != "Cluster")
        {
            if (gameObject.tag == "Cluster")
            {

                for (int i = 0; i <= clusterAmount; i++)
                {
                    Vector3 spawnPosition = transform.position;
                    //if (i >10)
                    Quaternion spawnRotation = Quaternion.Euler(0f, 360/clusterAmount-i, 0f); // Set the rotation (0 degrees)
                    Instantiate(bulletPrefab, spawnPosition, spawnRotation);
                    Destroy(gameObject);
                    //Instantiate(bulletPrefab, gameObject.transform.position, Quaternion.Euler(Vector3.forward * 90));
                }
            }
        }
        
    }
}
