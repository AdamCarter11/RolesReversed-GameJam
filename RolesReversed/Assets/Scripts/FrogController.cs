using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogController : MonoBehaviour
{
    private Vector3 origPos, targetPos;
    public float moveTime;
    private float pauseTime;

    private void Start()
    {
        moveTime = Random.Range(0.3f,0.6f);
        pauseTime = Random.Range(0.9f, 1.3f);
        print("movetime " + moveTime + " pauseTime " + pauseTime);
        StartCoroutine(movePause());
    }

    void Update()
    {
        
    }
    IEnumerator movePause()
    {

        while (true)
        {
            yield return new WaitForSeconds(pauseTime);
            StartCoroutine(MoveFrog(Vector3.up));
        }
       
    }

    private IEnumerator MoveFrog(Vector3 direction)
    {

        float elapsedTime = 0;

        origPos = transform.position;
        targetPos = origPos + direction;

        while (elapsedTime < moveTime)
        {
            transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime/moveTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;

        
    }

}
