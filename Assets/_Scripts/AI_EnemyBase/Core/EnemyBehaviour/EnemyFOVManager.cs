using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
class DetectorValue
{
    public string detectorName;
    public Vector3 facing;
    public Color radiusColor;
    public Color frustrumColor;
    public float maxAngle;
    public float maxRadius;
}

public class EnemyFOVManager : MonoBehaviour
{
    [Header("Set in inspector")]
    [SerializeField] List<DetectorValue> inspectorData;
    [SerializeField] LayerMask detectionLayers;

    #region PRIVATE_VARIABLES
    EnemyBehaviour enemyBehaviour;

    Dictionary<GameObject, DetectorValue> gameObjectDetectors;

    List<EnemyFOVDetector> detectors;
    #endregion

    private void Awake()
    {
        //Initialize and cache the needed components
        enemyBehaviour = GetComponentInParent<EnemyBehaviour>();
        gameObjectDetectors = new Dictionary<GameObject, DetectorValue>();
    }

    private void Start()
    {
        detectors = new List<EnemyFOVDetector>();

        InitializeHoldersAndDetectors();

        SetTargetAllDetectors(enemyBehaviour.GetAttackTarget());
    }

    /// <summary>
    /// Call to create a parent gameobject for each DetectorValue given from the inspector.
    /// </summary>
    void InitializeHoldersAndDetectors()
    {
        foreach (DetectorValue detector in inspectorData)
        {
            //Create an empty gameObject
            GameObject tempGO = new GameObject();
            tempGO.name = detector.detectorName;

            //Make it a child of the agent
            tempGO.transform.SetParent(transform);
            tempGO.transform.position = transform.position;

            InitializeDetector(detector, tempGO.transform);

            //Add the created gameObject and the detector value to the detectors dictionary.
            gameObjectDetectors.Add(tempGO, detector);
        }
    }

    /// <summary>
    /// Call to create an instance of an EnemyFOVDetector and assign each inspector data value to it.
    /// Then add it to the detectors list.
    /// </summary>
    /// <param name="inspectorData"></param>
    /// <param name="parent"></param>
    void InitializeDetector(DetectorValue inspectorData, Transform parent)
    {
        EnemyFOVDetector tempDetector = new EnemyFOVDetector(this, parent, inspectorData.radiusColor,
            inspectorData.frustrumColor, inspectorData.maxAngle, inspectorData.maxRadius, inspectorData.facing, detectionLayers);

        detectors.Add(tempDetector);
    }

    /// <summary>
    /// Call to set the passed transform as the target of each detector in the detectors list.
    /// </summary>
    /// <param name="target"></param>
    void SetTargetAllDetectors(Transform target)
    {
        foreach (EnemyFOVDetector detector in detectors)
        {
            detector.SetTarget(target);
        }
    }

    /// <summary>
    /// Call to deactivate each detector's update function.
    /// </summary>
    public void DeactivateAllDetectors()
    {
        foreach (EnemyFOVDetector detector in detectors)
        {
            detector.IsEnabled(false);
        }
    }

    /// <summary>
    /// Call to Activate each detector's update function.
    /// </summary>
    public void ActivateAllDetectors()
    {
        foreach (EnemyFOVDetector detector in detectors)
        {
            detector.IsEnabled(true);
        }
    }

    private void Update()
    {
        UpdateDetectors();
    }

    /// <summary>
    /// Call to invoke every detector's Update() method inside the detectors list.
    /// </summary>
    void UpdateDetectors()
    {
        foreach (EnemyFOVDetector detector in detectors)
        {
            detector.Update();
        }
    }

    /// <summary>
    /// Call to get the EnemyBehaviour this detector is attached to.
    /// </summary>
    /// <returns></returns>
    public EnemyBehaviour GetEnemyEntity()
    { return enemyBehaviour; }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!EditorApplication.isPlaying && inspectorData.Count > 0)
        {
            foreach (DetectorValue inspectorSetup in inspectorData)
            {
                //Draw the detection radius around the enemy
                Handles.color = inspectorSetup.radiusColor;
                Handles.DrawWireDisc(transform.position, Vector3.up, inspectorSetup.maxRadius);

                //Create the left and right linesOfSight
                Vector3 fovLineFront1 = Quaternion.AngleAxis(inspectorSetup.maxAngle, Vector3.up) * transform.forward * inspectorSetup.maxRadius;
                Vector3 fovLineFront2 = Quaternion.AngleAxis(-inspectorSetup.maxAngle, Vector3.up) * transform.forward * inspectorSetup.maxRadius;

                //Draw the FOV
                Gizmos.color = inspectorSetup.frustrumColor;
                Gizmos.DrawRay(transform.position, fovLineFront1);
                Gizmos.DrawRay(transform.position, fovLineFront2);

                //Draw the middle black ray
                Gizmos.color = Color.black;
                Gizmos.DrawRay(transform.position, transform.forward * inspectorSetup.maxRadius);

                //Quaternions to Vector3 
                Vector3 start = transform.position;
                float angle = inspectorSetup.facing.y;
                float radiants = angle * Mathf.Deg2Rad;

                float x = Mathf.Cos(radiants) * inspectorSetup.maxRadius + transform.position.x;
                float y = transform.position.y;
                float z = Mathf.Sin(radiants) * inspectorSetup.maxRadius + transform.position.z;

                Vector3 end = new Vector3(x, y, z);

                Gizmos.color = Color.green;
                Gizmos.DrawLine(start, end);
            }
        }
        else
        {
            foreach (EnemyFOVDetector detector in detectors)
            {
                detector.OnDrawGizmos();
            }
        }
    }
#endif
}