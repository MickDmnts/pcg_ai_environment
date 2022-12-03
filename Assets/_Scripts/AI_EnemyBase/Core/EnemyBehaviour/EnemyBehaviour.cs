using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    #region INSPECTOR_VARIABLES
    [Header("Set in inspector")]
    [SerializeField, Range(0f, 100f)] float idleTime;
    [SerializeField] float waypointDistanceOffset;
    [SerializeField] MeshRenderer enemyFin;
    #endregion

    #region PRIVATE_VARIABLES
    Transform attackTarget;
    EnemyFSM enemyFSM;
    List<Transform> waypoints;
    #endregion

    #region BehaviourCaching
    //FOV Detector caching
    private EnemyFOVManager fovManager;
    public EnemyFOVManager FOVManager
    {
        get { return fovManager; }
        private set { fovManager = value; }
    }
    #endregion

    /// <summary>
    /// *INTERNAL USE ONLY*
    /// <para>Subscribe to this event to get notified when the FOV manager of this 
    /// agent finds its assigned target.</para>
    /// </summary>
    public Action onPlayerFound;
    public void OnPlayerFound()
    {
        if (onPlayerFound != null)
        {
            onPlayerFound();
        }
    }

    private void Awake()
    {
        CacheBehaviours();
    }

    /// <summary>
    /// Call to cache the required game object components.
    /// </summary>
    void CacheBehaviours()
    {
        attackTarget = FindObjectOfType<PlayerController>().transform;
        FOVManager = GetComponentInChildren<EnemyFOVManager>();
    }

    private void Start()
    {
        onPlayerFound += TargetFound;

        FindWaypoints();

        CreateFSM();
    }

    /// <summary>
    /// Call to create an instance of EnemyFSM and assign it to the enemyFSM variable.
    /// </summary>
    void CreateFSM()
    {
        enemyFSM = new EnemyFSM(idleTime, waypoints, waypointDistanceOffset, attackTarget, transform);
    }

    /// <summary>
    /// Call to forcefully change the FSM state to ChaseState.
    /// </summary>
    void TargetFound()
    {
        enemyFSM.SetState(enemyFSM.ChaseState);
    }

    /// <summary>
    /// Call to find all the world game objects tagged as Waypoint.
    /// </summary>
    void FindWaypoints()
    {
        waypoints = new List<Transform>();
        GameObject[] tempArray = GameObject.FindGameObjectsWithTag("Waypoint");

        //Add each found gameObject to the waypoint list.
        foreach (GameObject waypoint in tempArray)
        {
            waypoints.Add(waypoint.transform);
        }
    }

    //This Update method acts as a tick rate for our agent's FSM 
    private void Update()
    {
        if (enemyFSM != null)
        {
            //Update the FSM behaviour
            enemyFSM.Update();
        }
    }

    /// <summary>
    /// Call to get the assigned AttackTarget of this agent.
    /// </summary>
    /// <returns></returns>
    public Transform GetAttackTarget()
    {
        return attackTarget;
    }

    /// <summary>
    /// Call to change the fin color of the agent to the passed color value.
    /// (Sets the _EmissionColor of the shader)
    /// </summary>
    public void ChangeFinColor(Color newColor)
    {
        enemyFin.material.SetColor("_EmissionColor", newColor);
    }

    /// <summary>
    /// Call to forcefully set the agent FSM state to BackToOrigin.
    /// </summary>
    public void ForceBackToOriginalPosState()
    {
        enemyFSM.SetState(enemyFSM.BackToOrigin);
    }

    private void OnDestroy()
    {
        onPlayerFound -= TargetFound;
    }
}