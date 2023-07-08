using System.Collections;
using System.Collections.Generic;
using System.Data;
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
    private Vector3 previousPosition;

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
        previousPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.rotation.z > -180 )
        {

        }
        // Movement


        // Streak effect
        StreakMarkLogic();

        //Mathf.Clamp(transform.rotation.z, -180, 0);
    }
    private void FixedUpdate()
    {
        MovementLogic();
        PositionCorrection();
        previousPosition = this.transform.position;
    }

    private void PositionCorrection()
    {
        // Check if there was a collision between the previous position and the current position
        RaycastHit2D hit = Physics2D.Linecast(previousPosition, this.transform.position);

        if (hit.collider != null && hit.collider.gameObject != this.gameObject)
        {
            // A collision occurred
            Debug.Log("Collision detected with: " + hit.collider.gameObject.name);
            this.transform.position = previousPosition;
        }
    }

    #region Movement Logic
    private void MovementLogic()
    {
        // Moving
        //MoveForce += transform.up * MoveSpeed * Time.deltaTime;
        MoveForce += transform.up * MoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime; // this line if we want the player to be able to control all axis
        transform.position += new Vector3(MoveForce.x, MoveForce.y) * Time.deltaTime;

        // Steering
        
        steerInput = Input.GetAxis("Horizontal");
        print(steerInput);
        Vector3 turnVal = Vector3.forward * -steerInput * MoveForce.magnitude * SteerAngle * Time.deltaTime;
        //print(turnVal);
        transform.Rotate(turnVal);
        Vector3 eulerAngle = transform.eulerAngles;

        float clampedZRotation = eulerAngle.z;

        if (clampedZRotation < 100)
        {
            clampedZRotation += 360f;
        }
        clampedZRotation = Mathf.Clamp(clampedZRotation, 180f, 360f);
        eulerAngle.z = clampedZRotation;
        transform.eulerAngles = eulerAngle;
        /*
        if (transform.rotation.z > -150 && transform.rotation.z < 0)
        {
            Vector3 turnVal = Vector3.forward * -steerInput * MoveForce.magnitude * SteerAngle * Time.deltaTime;
            //print(turnVal);
            transform.Rotate(turnVal);
        }
        
        if(transform.rotation.z <= -150 && steerInput < 0) 
        {
            Vector3 turnVal = Vector3.forward * -steerInput * MoveForce.magnitude * SteerAngle * Time.deltaTime;
            //print(turnVal);
            transform.Rotate(turnVal);
        }
        if (transform.rotation.z >= 0 && steerInput > 0)
        {
            Vector3 turnVal = Vector3.forward * -steerInput * MoveForce.magnitude * SteerAngle * Time.deltaTime;
            //print(turnVal);
            transform.Rotate(turnVal);
        }
        */

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