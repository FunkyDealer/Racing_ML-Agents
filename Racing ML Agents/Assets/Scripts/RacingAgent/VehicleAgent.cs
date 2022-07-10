using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class VehicleAgent : Agent, IRacerAI
{
    // mlagents-learn --run-id=RacingAgent<id> ConfigFiles/RacingConfig.yaml

    // tensorboard --logdir results //for results

    private const string ACCEL = "Accel";
    private const string BRAKE = "Brake";
    private const string HORIZONTAL = "Horizontal";

    public enum Axel
    {
        Front,
        Rear
    }
    [Serializable]
    public struct Wheel
    {
        public GameObject model;
        public WheelCollider collider;
        public Axel axel;
    }

    [Header("Agent Stuff")]
    private Transform nextWaypoint;
    private Vector3 previousWayPoint;

    private Transform firstWayPoint;
    public void SetFirstWayPoint(Transform t) => firstWayPoint = t;

    private SpawnZone respawnPoint;
    public void SetRespawnPoint(SpawnZone t) => respawnPoint = t;
    public SpawnZone GetRespawnPoint() => respawnPoint;

    [Header("other Objects")]
    //[SerializeField]
    //private WayPointPointer pointer;
    [SerializeField]
    private RacingSweeper sweeper;
    [SerializeField]
    private RacingTrainerManager manager;

    private float baseRewardPerPercentageDone = 3;

    private int checkPointsDone = 0;
    private int totalCheckPointsInTrack = 0;
    private Vector3 startingPos;

    private bool goingForward = true;

    [SerializeField]
    private LayerMask environmentLayer;
    private int collisionCount = 0;

    private MeshRenderer renderer;
    private Color originalColor;

    [Header("Car Physics")]
    [SerializeField]
    private float maxAcceleration = 30.0f;
    [SerializeField]
    private float MotorForce = 600;
    [SerializeField]
    private float brakeAcceleration = 50.0f;
    [SerializeField]
    private float brakeForce = 700f;

    [SerializeField]
    private float turnSensitivity = 1.0f;
    [SerializeField]
    private float maxSteerAngle = 30.0f;

    [SerializeField]
    Vector3 centerOfMass;

    [SerializeField]
    private List<Wheel> wheels;

    float accelBrakeInput;
    float steerInput;
    bool reversing = false;

    [SerializeField]
    float rigidBodySpeed;    

    private Rigidbody myRigidBody;

    private string CurrentTrackName = "";
    private bool alive = false;

    private bool startedRace = false;


    void Awake()
    {
        sweeper.GetAgent(this);

    }

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
        myRigidBody.centerOfMass = centerOfMass;

        checkPointsDone = 0;
    }

    public override void OnEpisodeBegin()
    {
        
        //Set Agent Active
        gameObject.SetActive(true);
        alive = true;
        startedRace = false;

        //Set Agent Sweeper
        sweeper.transform.parent = transform.parent;
        //set agent Pointer 
        //pointer.transform.parent = transform.parent;

        //1. stop the Agent
        ResetVehicle();
        ResetInput();

        //1. Place Agent in starting position
        transform.localPosition = respawnPoint.transform.localPosition;
        transform.rotation = respawnPoint.transform.rotation;

        //2. get next WayPoint
        nextWaypoint = firstWayPoint;
        previousWayPoint = transform.position;

        checkPointsDone = 0;
        collisionCount = 0;
        startingPos = transform.localPosition;

        //pointer.GetNewTarget(nextWaypoint);
        sweeper.Reset();
    }

    // Update is called once per frame
    void Update()
    {
        //GetInput();
        //AnimateWheels();


    }

    private void FixedUpdate()
    {
        rigidBodySpeed = myRigidBody.velocity.magnitude;

        Move();
        Steer();
        BrakeOrReverse(); //if the car is not reversing, brake with the brakeInput, otherwise, move backward with the brakeInpute
       // BrakeForReverse(); //if the car is reversing, brake with the AccelInput
        ResetInput();
    }

    //input to the neural network
    public override void CollectObservations(VectorSensor sensor)
    {
        //Input what the agent observes to the network

        // 2 floats (Agent position)
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.z);

        // 2 float (next waypoint position)
        sensor.AddObservation(nextWaypoint.localPosition.x);
        sensor.AddObservation(nextWaypoint.localPosition.z);

        //1 float (distance to waypoint)
        float distanceToWayPoint = Vector3.Distance(transform.localPosition, nextWaypoint.localPosition);
        sensor.AddObservation(distanceToWayPoint);

        //2 floats (Direction to waypoint, in the plane)
        Vector3 directionToWayPoint = nextWaypoint.localPosition - transform.position;
        directionToWayPoint.Normalize();
        sensor.AddObservation(directionToWayPoint.x);
        sensor.AddObservation(directionToWayPoint.z);

        // 3 float (Velocity of the agent)
        sensor.AddObservation(myRigidBody.velocity);

        //total = 2 + 2 + 1 + 2 + 3 = 10

    }

    // process output of the neural network
    public override void OnActionReceived(ActionBuffers actions)
    {
        // obtain actions
        ActionSegment<float> axis = actions.ContinuousActions;
        steerInput = axis[0];
        accelBrakeInput = axis[1];

        if (myRigidBody.velocity.magnitude > 1f) { 
            AddReward(myRigidBody.velocity.magnitude * 0.001f); //add reward if the agent is moving

            Vector3 correctDir = (nextWaypoint.transform.position - transform.position).normalized;
            float reward = Vector3.Dot(myRigidBody.velocity.normalized, correctDir);
            AddReward(reward * 0.001f); //add reward if the agent is going toward the checkpoint
        }
    }

    // Generates the simulated output to the neural network
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        float accelBrakeInputTemp = Input.GetAxis(ACCEL);
        float steerInputTemp = Input.GetAxis(HORIZONTAL);

        ActionSegment<float> axis = actionsOut.ContinuousActions;
        axis[0] = steerInputTemp;
        axis[1] = accelBrakeInputTemp;
    }

    private void ResetVehicle()
    {
        foreach (var w in wheels)
        {
            w.collider.motorTorque = 0;
            w.collider.steerAngle = 0;
            w.collider.brakeTorque = 0;   
        }
        myRigidBody.velocity = Vector3.zero;
        myRigidBody.angularVelocity = Vector3.zero;
        reversing = false;
    }

    private void GetInput()
    {
            accelBrakeInput = Input.GetAxis(ACCEL);
            steerInput = Input.GetAxis(HORIZONTAL);
    }

    private void Steer()
    {
        foreach (var w in wheels)
        {
            if (w.axel == Axel.Front)
            {
                float steerAngle = steerInput * turnSensitivity * maxSteerAngle;
                w.collider.steerAngle = Mathf.Lerp(w.collider.steerAngle, steerAngle, 0.6f);
            }
        }
    }

    private void Move()
    {
        if (accelBrakeInput > 0) {
            foreach (var w in wheels)
            {
                if (w.axel == Axel.Rear)
                    w.collider.motorTorque = accelBrakeInput * MotorForce * maxAcceleration * Time.deltaTime;
            }
        }
    }

    private void BrakeOrReverse()
    {       
        foreach (var w in wheels)
        {
            if (accelBrakeInput < 0) w.collider.brakeTorque = -accelBrakeInput * brakeForce * brakeAcceleration * Time.deltaTime;
            else w.collider.brakeTorque = 0;
            //else if (myRigidBody.velocity.magnitude <= 0 && accelBrakeInput < 0 && w.axel == Axel.Rear)
            //{
            //    w.collider.motorTorque = accelBrakeInput * MotorForce * maxAcceleration * Time.deltaTime;
            //    reversing = true;
            //}
        }        
    }

    private void BrakeForReverse()
    {
        if (reversing)
        {
            foreach (var w in wheels)
            {
               if (myRigidBody.velocity.magnitude <= 0) w.collider.brakeTorque = accelBrakeInput * brakeForce * brakeAcceleration * Time.deltaTime;
            }
        }
    }

    private void AnimateWheels()
    {
        foreach (var w in wheels)
        {
            Quaternion rot;
            Vector3 pos;
            w.collider.GetWorldPose(out pos, out rot);
            w.model.transform.position = pos;
            w.model.transform.rotation = rot;
        }
    }

    /// <summary>
    ///Agent Extra Functions
    /// </summary>

    private void ResetInput()
    {
        reversing = false;
    }

    public void GetNextWayPoint(Transform nextPoint, bool end)
    {
        previousWayPoint = nextWaypoint.position;
        this.nextWaypoint = nextPoint;
        //pointer.GetNewTarget(nextWaypoint);

        checkPointsDone++;
        float percentageDone = (checkPointsDone * 100) / totalCheckPointsInTrack;
        percentageDone *= 0.1f; //make the number smaller
        AddReward(baseRewardPerPercentageDone * percentageDone);

        //Debug.Log($"percentage Done: {percentageDone} reward: {GetCumulativeReward()}");

        if (end)
        {
            manager.SucessChange(Color.green);
            End();
        }
    }

    public void GetPreviousWayPoint(Transform previousPoint, bool end)
    {
        previousWayPoint = nextWaypoint.position;
        this.nextWaypoint = previousPoint;
        //pointer.GetNewTarget(nextWaypoint);

        checkPointsDone++;
        float percentageDone = (checkPointsDone * 100) / totalCheckPointsInTrack;
        percentageDone *= 0.1f; //make the number smaller
        AddReward(baseRewardPerPercentageDone * percentageDone);

        //Debug.Log($"percentage Done: {percentageDone} reward: {GetCumulativeReward()}");

        if (end)
        {
            manager.SucessChange(Color.green);
            End();
        }
    }

    public Transform GetAINextWayPoint()
    {
        return nextWaypoint;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void Die()
    {
        float percentageDone = (checkPointsDone + 1 * 100) / totalCheckPointsInTrack;
        percentageDone *= 0.1f; //make the number smaller
        manager.SucessChange(new Color(1 - (0.1f * (percentageDone * 0.1f)), (0.1f * (percentageDone * 0.1f)), 0, 1));

        //if (checkPointsDone == 0)
        //{
        //    AddReward(-0.5f);
        //    if (Vector3.Distance(startingPos, transform.localPosition) < 1) AddReward(-0.5f);
        //}

        //calculate reward based on distance missing to next goal
        //float maxReward = baseRewardPerPercentageDone * (percentageDone);

        //float distanceToGoal = Vector3.Distance(transform.position, nextWaypoint.position);
        //float maxDistanceToGoal = Vector3.Distance(previousWayPoint, nextWaypoint.position);

        //AddReward(maxReward - (maxReward * (distanceToGoal / maxDistanceToGoal)));

        //Debug.Log($"percentage Done: {percentageDone} reward: {GetCumulativeReward()}");

        End();
    }

    private void End()
    {
        gameObject.SetActive(false);

        alive = false;
        ResetVehicle();
        ResetInput();

        sweeper.transform.parent = transform.parent;
        //pointer.transform.parent = transform.parent;        

        manager.EndAgentEpisode(this, GetCumulativeReward(), CurrentTrackName, goingForward);
    }

    public void ResetAgent()
    {
        gameObject.SetActive(true);
        EndEpisode();
    }

    private IEnumerable WarnSweeper()
    {
        yield return new WaitForSeconds(5);

        sweeper.GetWarned(transform);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (environmentLayer.Includes(collision.gameObject.layer))
        {
            collisionCount++;
            //AddReward(-0.3f * collisionCount);

           //renderer.material.color = Color.red;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (environmentLayer.Includes(collision.gameObject.layer))
        {
            //renderer.material.color = originalColor;
        }
    }

    public bool GetRaceDir() => goingForward;

    public void SetRaceDir(bool forward)
    {
        this.goingForward = forward;
    }
    
    public void SetTotalCheckPoints(int total)
    {
        totalCheckPointsInTrack = total + 1;
    }

    public void SetTrackName(string trackName)
    {
        this.CurrentTrackName = trackName;
    }

    public bool GetAliveStatus() => alive;

    public void StartRace()
    {
        startedRace = true;
    }
    
    public bool HasStartedRace() => startedRace;
}
