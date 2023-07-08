using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBordersBehavior : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject leftBorder;
    [SerializeField] private GameObject rightBorder;
    [SerializeField] private GameObject topBorder;
    [SerializeField] private GameObject bottomBorder;
    [SerializeField]private float boarderThickness = 0.1f;

    private float cameraHeight;
    private float cameraWidth;

    private void Start()
    {
        cameraHeight = 2f * mainCamera.orthographicSize;
        cameraWidth = cameraHeight * mainCamera.aspect;

        // Set the positions and scales of the child game objects
        SetBorderPositionAndScaleLeftRight(leftBorder, mainCamera.transform.position.x - cameraWidth / 2f, mainCamera.transform.position.y, cameraHeight);
        SetBorderPositionAndScaleLeftRight(rightBorder, mainCamera.transform.position.x + cameraWidth / 2f, mainCamera.transform.position.y, cameraHeight);
        SetBorderPositionAndScaleTopBottom(topBorder, mainCamera.transform.position.x, mainCamera.transform.position.y + cameraHeight / 2f, cameraWidth);
        SetBorderPositionAndScaleTopBottom(bottomBorder, mainCamera.transform.position.x, mainCamera.transform.position.y - cameraHeight / 2f, cameraWidth);
    }

    private void SetBorderPositionAndScaleLeftRight(GameObject border, float x, float y, float size)
    {
        border.transform.position = new Vector2(x, y);
        border.transform.localScale = new Vector3(boarderThickness, size, border.transform.localScale.z);
    }
    private void SetBorderPositionAndScaleTopBottom(GameObject border, float x, float y, float size)
    {
        border.transform.position = new Vector2(x, y);
        border.transform.localScale = new Vector3(size, boarderThickness, border.transform.localScale.z);
    }
}
