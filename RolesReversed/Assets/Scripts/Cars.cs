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
    [SerializeField] float constantSpeed = .2f;
    [SerializeField] CameraController cameraScript;
    private Vector3 MoveForce;
    private float steerInput;
    private Vector3 previousPosition;
    private Vector3 tempMoveForce;

    [Header("Streak mark vars")]
    [SerializeField] float StreakLineWidth = 0.1f;
    [SerializeField] Color StreakLineColor = Color.red;
    [SerializeField] int MaxStreakPoints = 100;
    [SerializeField] float StreakSmoothness = 0.2f;
    [SerializeField] float ClearSpeed = 10f;
    [SerializeField] Sprite[] carSprites;
    private bool isDrifting;
    private List<Vector3> rearLeftStreakPoints = new List<Vector3>();
    private List<Vector3> rearRightStreakPoints = new List<Vector3>();
    private LineRenderer rearLeftStreakLine;
    private LineRenderer rearRightStreakLine;

    private GameManager gameManager;
    [SerializeField] ParticleSystem splatterRed;
    [SerializeField] ParticleSystem splatterRed2;
    [SerializeField] ParticleSystem splatterGreen;

    [SerializeField] AudioSource frogSplat;
    [SerializeField] AudioSource humanSplat;
    void Start()
    {
        gameManager = GameManager.instance;
        // setup lines for skid marks
        InitialLineSetup();
        previousPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Movement


        // Streak effect
        StreakMarkLogic();

        //Mathf.Clamp(transform.rotation.z, -180, 0);
    }
    private void FixedUpdate()
    {
        if (cameraScript.changeScene)
        {
            if(MoveForce.x != 0)
                tempMoveForce = MoveForce;
            MoveForce = new Vector3(0, 0, 0);
        }
        else
        {
            if(tempMoveForce.x != 0 && MoveForce.x == 0)
            {
                MoveForce = tempMoveForce;
            }
            MovementLogic();
        }
        //PositionCorrection();
        //previousPosition = this.transform.position;
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
    public void IncreaseSpeed()
    {
        MoveSpeed += .5f;
        MaxSpeed += .5f;
        constantSpeed += .1f;
        cameraScript.speedIncrease += 1f;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Frog"))
        {
            Instantiate(splatterRed, collision.gameObject.transform.position, Quaternion.Euler(transform.rotation.x,transform.rotation.y,(transform.eulerAngles.z -270f)));
            Instantiate(splatterGreen, collision.gameObject.transform.position, Quaternion.Euler(transform.rotation.x, transform.rotation.y, (transform.eulerAngles.z - 270f)));
            splatterGreen.Play();
            splatterRed.Play();

            Destroy(collision.gameObject);
            gameManager.amountOfFrogs--;
            GameManager.instance.frogsDestroyed++;
            GetComponent<SpriteRenderer>().sprite = carSprites[GameManager.instance.frogsDestroyed];

            frogSplat.Play();
            
        }
        if (collision.gameObject.CompareTag("Human"))
        {
            Instantiate(splatterRed2, collision.gameObject.transform.position, Quaternion.Euler(transform.rotation.x, transform.rotation.y, (transform.eulerAngles.z - 270f)));
            splatterRed2.Play();

            Destroy(collision.gameObject);
            GameManager.instance.health--;

            humanSplat.Play();
        }
        if (collision.gameObject.CompareTag("endBound"))
        {

            endBoundLoad();
        }
    }
    public void endBoundLoad()
    {
        GameManager.instance.NextLevel();
        cameraScript.SceneTransition();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Frog"))
        {
            Destroy(collision.gameObject);
            gameManager.amountOfFrogs--;
        }
    }

    #region Movement Logic
    private void MovementLogic()
    {
        // Moving
        //MoveForce += transform.up * MoveSpeed * Time.deltaTime;
        float forwardForce = Input.GetAxis("Vertical") + constantSpeed;
        if(forwardForce < 0)
        {
            forwardForce = 0 + constantSpeed;
            //MoveForce = new Vector3(0, 0, 0); // if we want stopping 
        }
        MoveForce += transform.up * MoveSpeed * forwardForce * Time.deltaTime; // this line if we want the player to be able to control all axis
        transform.position += new Vector3(MoveForce.x, MoveForce.y) * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -10, 10), transform.position.z);
        //print("car pos: " + transform.position);

        // Steering
        
        steerInput = Input.GetAxis("Horizontal");
        //print(steerInput);
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

        // Drag and max speed limit
        MoveForce *= Drag;
        MoveForce = Vector2.ClampMagnitude(MoveForce, MaxSpeed);

        // Traction
        MoveForce = Vector2.Lerp(MoveForce.normalized, transform.up, Traction * Time.deltaTime) * MoveForce.magnitude;
    }
    public void MoveForceReset()
    {
        MoveForce = new Vector3(0, 0, 0);
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
            Vector3 rearLeftPoint = currentPoint - transform.right * 0.6f - transform.up * 0.9f;
            Vector3 rearRightPoint = currentPoint + transform.right * 0.6f - transform.up * 0.9f;

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
    private void ClearAll(List<Vector3> streakPoints, LineRenderer streakLine)
    {
        if (streakPoints.Count == 0)
            return;

        streakPoints.Clear();
        streakLine.positionCount = 0;
    }
    public void ClearHelper()
    {
        ClearAll(rearLeftStreakPoints, rearLeftStreakLine);
        ClearAll(rearRightStreakPoints, rearRightStreakLine);
    }
    #endregion
}