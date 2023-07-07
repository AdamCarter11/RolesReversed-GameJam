using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cars : MonoBehaviour
{
    [Header("Movement vars")]
    [SerializeField] float MoveSpeed = 50;
    [SerializeField] float MaxSpeed = 15;
    [SerializeField] float Drag = 0.98f;
    [SerializeField] float SteerAngle = 20;
    [SerializeField] float Traction = 1;
    private Vector3 MoveForce;
    private float steerInput;

    [Header("Streak mark vars")]
    [SerializeField] float StreakLineWidth = 0.1f;
    [SerializeField] Color StreakLineColor = Color.red;
    [SerializeField] int MaxStreakPoints = 100;
    [SerializeField] float StreakSmoothness = 0.2f;
    [SerializeField] float ClearSpeed = 10f;
    private bool isDrifting;
    private List<Vector3> rearLeftStreakPoints = new List<Vector3>();
    private List<Vector3> rearRightStreakPoints = new List<Vector3>();
    private LineRenderer rearLeftStreakLine;
    private LineRenderer rearRightStreakLine;

    void Start()
    {
        // setup lines for skid marks
        InitialLineSetup();
    }

    // Update is called once per frame
    void Update()
    {
        // Movement
        MovementLogic();

        // Streak effect
        StreakMarkLogic();
    }

    #region Movement Logic
    private void MovementLogic()
    {
        // Moving
        MoveForce += transform.up * MoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
        transform.position += new Vector3(MoveForce.x, MoveForce.y) * Time.deltaTime;

        // Steering
        steerInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.forward * -steerInput * MoveForce.magnitude * SteerAngle * Time.deltaTime);

        // Drag and max speed limit
        MoveForce *= Drag;
        MoveForce = Vector2.ClampMagnitude(MoveForce, MaxSpeed);

        // Traction
        MoveForce = Vector2.Lerp(MoveForce.normalized, transform.up, Traction * Time.deltaTime) * MoveForce.magnitude;
    }
    #endregion

    #region Streak Mark Logic
    private void InitialLineSetup()
    {
        GameObject rearLeftStreakLineObj = new GameObject("RearLeftStreakLine");
        rearLeftStreakLine = rearLeftStreakLineObj.AddComponent<LineRenderer>();
        SetupLineRenderer(rearLeftStreakLine);

        GameObject rearRightStreakLineObj = new GameObject("RearRightStreakLine");
        rearRightStreakLine = rearRightStreakLineObj.AddComponent<LineRenderer>();
        SetupLineRenderer(rearRightStreakLine);
    }

    private void SetupLineRenderer(LineRenderer lineRenderer)
    {
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = lineRenderer.endWidth = StreakLineWidth;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.material.color = StreakLineColor;
    }

    private void StreakMarkLogic()
    {
        if (!isDrifting && Mathf.Abs(steerInput) > 0.2f && MoveForce.magnitude > 5f)
        {
            isDrifting = true;
        }
        else if (isDrifting && Mathf.Abs(steerInput) <= 0.2f)
        {
            isDrifting = false;
            ClearStreakLine(rearLeftStreakPoints, rearLeftStreakLine);
            ClearStreakLine(rearRightStreakPoints, rearRightStreakLine);
        }

        if (isDrifting)
        {
            Vector3 currentPoint = transform.position;
            Vector3 rearLeftPoint = currentPoint - transform.right * 0.5f - transform.up * 0.5f;
            Vector3 rearRightPoint = currentPoint + transform.right * 0.5f - transform.up * 0.5f;

            if (rearLeftStreakPoints.Count > 0)
            {
                Vector3 lastRearLeftPoint = rearLeftStreakPoints[rearLeftStreakPoints.Count - 1];
                float rearLeftDistance = Vector3.Distance(lastRearLeftPoint, rearLeftPoint);
                if (rearLeftDistance > StreakSmoothness)
                {
                    Vector3 rearLeftDirection = (rearLeftPoint - lastRearLeftPoint).normalized;
                    int rearLeftSegments = Mathf.CeilToInt(rearLeftDistance / StreakSmoothness);
                    float rearLeftSegmentLength = rearLeftDistance / rearLeftSegments;

                    for (int i = 0; i < rearLeftSegments; i++)
                    {
                        Vector3 point = lastRearLeftPoint + rearLeftDirection * rearLeftSegmentLength * i;
                        AddStreakPoint(rearLeftStreakPoints, rearLeftStreakLine, point);
                    }
                }
            }
            else
            {
                AddStreakPoint(rearLeftStreakPoints, rearLeftStreakLine, rearLeftPoint);
            }

            if (rearRightStreakPoints.Count > 0)
            {
                Vector3 lastRearRightPoint = rearRightStreakPoints[rearRightStreakPoints.Count - 1];
                float rearRightDistance = Vector3.Distance(lastRearRightPoint, rearRightPoint);
                if (rearRightDistance > StreakSmoothness)
                {
                    Vector3 rearRightDirection = (rearRightPoint - lastRearRightPoint).normalized;
                    int rearRightSegments = Mathf.CeilToInt(rearRightDistance / StreakSmoothness);
                    float rearRightSegmentLength = rearRightDistance / rearRightSegments;

                    for (int i = 0; i < rearRightSegments; i++)
                    {
                        Vector3 point = lastRearRightPoint + rearRightDirection * rearRightSegmentLength * i;
                        AddStreakPoint(rearRightStreakPoints, rearRightStreakLine, point);
                    }
                }
            }
            else
            {
                AddStreakPoint(rearRightStreakPoints, rearRightStreakLine, rearRightPoint);
            }
        }
        else
        {
            ClearStreakLine(rearLeftStreakPoints, rearLeftStreakLine);
            ClearStreakLine(rearRightStreakPoints, rearRightStreakLine);
        }
    }

    void AddStreakPoint(List<Vector3> streakPoints, LineRenderer streakLine, Vector3 point)
    {
        streakPoints.Add(point);

        if (streakPoints.Count > MaxStreakPoints)
        {
            streakPoints.RemoveAt(0);
        }

        streakLine.positionCount = streakPoints.Count;
        streakLine.SetPositions(streakPoints.ToArray());
    }

    void ClearStreakLine(List<Vector3> streakPoints, LineRenderer streakLine)
    {
        if (streakPoints.Count == 0)
            return;

        int clearAmount = (int)(ClearSpeed * Time.deltaTime);
        clearAmount = Mathf.Clamp(clearAmount, 1, streakPoints.Count);

        streakPoints.RemoveRange(0, clearAmount);
        streakLine.positionCount = streakPoints.Count;
        streakLine.SetPositions(streakPoints.ToArray());
    }
    #endregion
}