using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    [SerializeField] float shakeMagnitude;
    [SerializeField] float dampeningSpeed;
    [HideInInspector] public Vector3 initialPos;
    [SerializeField] private CameraController cmController;
    float shakeDuration;

    private bool sceneTransitioned = false;

    void Start()
    {
        initialPos = transform.localPosition;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            transform.localPosition = initialPos + Random.insideUnitSphere * shakeMagnitude;
            shakeDuration -= Time.deltaTime * dampeningSpeed;
        }
        else
        {
            if (!cmController.changeScene)
            {
                transform.localPosition = new Vector3(initialPos.x, initialPos.y, initialPos.z);
            }
            else
            {
                initialPos = transform.position;
            }
            shakeDuration = 0f;
        }

        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -10);
    }
    public void TriggerShake()
    {
        shakeDuration = .5f;
        initialPos = transform.position;
    }
}
