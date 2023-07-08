using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float speed = 6f;
    public bool changeScene = false;
    private Vector3 target;
    [SerializeField] GameObject topBound;
    [SerializeField] GameObject bottomBound;
    [SerializeField] GameObject endCollider;
    [SerializeField] float changeVal;
    Vector3 start;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (changeScene)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            if(transform.position == target)
            {
                topBound.transform.position = topBound.transform.position + new Vector3(changeVal, 0, 0);
                bottomBound.transform.position = bottomBound.transform.position + new Vector3(changeVal, 0, 0);
                endCollider.transform.position = endCollider.transform.position + new Vector3(changeVal, 0, 0);
                changeScene = false;
                GameManager.instance.GenerateStuff();
            }
        }
    }
    
    public void SceneTransition()
    {
        GameManager.instance.offset += changeVal;
        changeScene = true;
        //print(transform.position + new Vector3(8, 0, 0));
        target = transform.position + new Vector3(changeVal, 0,0);

        
    }
}
