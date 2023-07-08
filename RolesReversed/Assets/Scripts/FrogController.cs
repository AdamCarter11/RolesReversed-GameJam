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
        moveTime = Random.Range(0.1f,0.5f);
        pauseTime = Random.Range(.6f, 1.2f);
        //print("movetime " + moveTime + " pauseTime " + pauseTime);
        StartCoroutine(movePause());
    }

    void Update()
    {
        
    }
    IEnumerator movePause()
    {

        while (true)
        {
            StartCoroutine(MoveFrog(Vector3.up));
            yield return new WaitForSeconds(pauseTime);
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
            if(transform.position.y >= 5)
            {
                GameManager.instance.health--;
                GameManager.instance.amountOfFrogs--;
                Destroy(this.gameObject);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;

        
    }

}
