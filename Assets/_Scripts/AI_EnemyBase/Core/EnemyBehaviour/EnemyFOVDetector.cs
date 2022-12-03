using UnityEngine;
using UnityEditor;

[System.Serializable]
public class EnemyFOVDetector
{
    #region DETECTOR_VALUES
    Transform target;
    float maxAngle;
    float maxRadius;
    LayerMask detectionLayers;

    bool isInFOV = false;
    bool isEnabled = false;

    EnemyFOVManager fovManager;
    Transform detector;
    Color radiusColor;
    Color frustrumColor;
    #endregion

    /// <summary>
    /// Overiden constructor.
    /// </summary>
    /// <param name="enemyFOVManager">The enemyFOVmanager of this detector.</param>
    /// <param name="origin">The center of the detector</param>
    /// <param name="radiusColor">The radious color of the detector</param>
    /// <param name="frustrumColor">The FOV fructrum color of the detector</param>
    /// <param name="maxAngle">The fructrum max angle of the detector</param>
    /// <param name="maxRadious">The max sphere radius of the detector</param>
    /// <param name="rotation">The rotation (facing) of the detector</param>
    /// <param name="detectionLayers">The layers the detector should detect collision on.</param>
    public EnemyFOVDetector(EnemyFOVManager enemyFOVManager,
        Transform origin, Color radiusColor, Color frustrumColor,
        float maxAngle, float maxRadious, Vector3 rotation, LayerMask detectionLayers)
    {
        this.fovManager = enemyFOVManager;

        this.detector = origin;

        this.detector.rotation = Quaternion.Euler(rotation);

        radiusColor.a = 255f;
        frustrumColor.a = 255f;

        this.radiusColor = radiusColor;
        this.frustrumColor = frustrumColor;

        this.maxAngle = maxAngle;
        this.maxRadius = maxRadious;
        this.detectionLayers = detectionLayers;

        IsEnabled(true);
    }

    /// <summary>
    /// Call to set the detector target to the passed transform
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    /// <summary>
    /// Call to update this detectors functionality.
    /// NOT CALLED FROM UNITY API
    /// </summary>
    public void Update()
    {
        //Early exit if the target is null or the detector is deactivated.
        if (target == null || !isEnabled) return;

        //If the target is not in fov range
        if (!isInFOV)
        {
            //...update the fov
            isInFOV = IsInFOV(detector, target, maxAngle, maxRadius);
        }
        else
        {
            //...else notify the attached enemy entity that the target is in range.
            fovManager.GetEnemyEntity().OnPlayerFound();
            isInFOV = false;
        }
    }

    /// <summary>
    /// Call to check if the given target is inside the defined range given from maxAngle and maxRadious.
    /// </summary>
    /// <param name="observerAgent">The origin transform</param>
    /// <param name="target">The target to check for</param>
    /// <param name="maxAngle">The max angle of the fov frustrum</param>
    /// <param name="maxRadius">The max radious of the circle around the agent.</param>
    /// <returns>True if the target is inside the frustrum and the radious, false otherwide.</returns>
    private bool IsInFOV(Transform observerAgent, Transform target, float maxAngle, float maxRadius)
    {
        //Cast a sphere around the agent and cache each collision
        Collider[] overlaps = new Collider[50];
        int count = Physics.OverlapSphereNonAlloc(observerAgent.position, maxRadius, overlaps, detectionLayers);

        for (int i = 0; i < count + 1; i++)
        {
            //Early exit if the collision is null
            if (overlaps[i] == null) continue;

            //If the target is inside the sphere radius
            if (overlaps[i].transform.Equals(target))
            {
                //...create an angle value between the agent and the target...
                Vector3 dirBetween = (target.position - observerAgent.position).normalized;
                float angle = Vector3.Angle(observerAgent.forward, dirBetween);

                //...if the calculated angle is smaller than the maxAngle...
                if (angle <= maxAngle)
                {
                    //...cast a ray towards the target...
                    Ray ray = new Ray(observerAgent.position, target.position - observerAgent.position);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, maxRadius))
                    {
                        //...finally if the ray hits the target directly this means there is nothing obscuring him.
                        if (hit.transform.Equals(target))
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Call to set the isEnabled bool to the passed value.
    /// </summary>
    public void IsEnabled(bool state)
    {
        this.isEnabled = state;
    }

    /// <summary>
    /// Call to set the maxAngle value of the detector
    /// </summary>
    public void SetAngle(float value)
    {
        this.maxAngle = value;
    }

    /// <summary>
    /// Call to set the maxRadious value of the detector.
    /// </summary>
    /// <param name="value"></param>
    public void SetRadius(float value)
    {
        this.maxRadius = value;
    }

    #region VISUALIZATION
#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        if (target == null || !isEnabled) return;

        //Draw the detection radius around the enemy
        Handles.color = radiusColor;
        Handles.DrawWireDisc(detector.transform.position, Vector3.up, maxRadius);

        //Create the left and right linesOfSight
        Vector3 fovLineFront1 = Quaternion.AngleAxis(maxAngle, Vector3.up) * detector.forward * maxRadius;
        Vector3 fovLineFront2 = Quaternion.AngleAxis(-maxAngle, Vector3.up) * detector.forward * maxRadius;

        //Draw the FOV
        Gizmos.color = frustrumColor;
        Gizmos.DrawRay(detector.position, fovLineFront1);
        Gizmos.DrawRay(detector.position, fovLineFront2);

        if (!isInFOV)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.green;
        }

        Gizmos.DrawRay(detector.position, (target.position - detector.position).normalized * maxRadius);

        //Draw the middle black ray
        Gizmos.color = Color.black;
        Gizmos.DrawRay(detector.position, detector.forward * maxRadius);
    }
#endif
    #endregion
}