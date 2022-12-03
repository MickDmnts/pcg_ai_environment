using UnityEngine;

/* CLASS DOCUMENTATION *\
* [Variable Specifics]
* Dynamically - Private changed: These variables are changed throughout the game.
* 
* [Class Flow]
* 1. The only purpose of this class file is to play the Door open animation of the trigger activated door (BigHouse).
* 2. The trigger can be activated only once.
* 
* [Must Know]
* 1. A DoorAnimations script must be attached to the root gameObject.
*/

[RequireComponent(typeof(BoxCollider))]
public class DoorActivator : MonoBehaviour
{
    #region PRIVATE_VARIABLES
    BoxCollider doorCollider;
    DoorAnimations doorAnimations;

    //Works as a flag that ensures the single-activation of the trigger event.
    bool activated;
    #endregion

    private void Awake()
    {
        SetupTrigger();
        CacheDoorAnimationsScript();
    }

    /// <summary>
    /// Call to make the attached BoxCollider a trigger.
    /// </summary>
    void SetupTrigger()
    {
        doorCollider = GetComponent<BoxCollider>();
        doorCollider.isTrigger = true;
    }

    /// <summary>
    /// Call to cache the DoorAnimations() script attached to the root gameObject
    /// </summary>
    void CacheDoorAnimationsScript()
    {
        doorAnimations = transform.root.GetComponentInChildren<DoorAnimations>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Early exit if the trigger got activated once
        if (activated) return;

        if (other.CompareTag("Player"))
        {
            //Play the door open animation.
            doorAnimations.Interact();

            activated = true;
        }
    }
}
