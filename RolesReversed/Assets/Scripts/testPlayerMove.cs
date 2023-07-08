using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testPlayerMove : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] CameraController cameraScript;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        rb.AddForce(rb.velocity);
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraScript.changeScene)
        {
            rb.velocity = Vector3.zero;
        }
        else
        {
            rb.velocity = Vector3.right * 5;
        }
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //rb.velocity= Vector3.zero;
        cameraScript.SceneTransition();
    }

}
