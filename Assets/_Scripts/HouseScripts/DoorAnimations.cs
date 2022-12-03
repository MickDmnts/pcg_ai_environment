using UnityEngine;

/* CLASS DOCUMENTATION *\
* [Variable Specifics]
* Dynamically - Private changed: These variables are changed throughout the game.
* 
* [Class Flow]
* 1. The entry of this class file is the Interact() method inherited from the IInteractable interface which calls the 
*   PlayDoorAnimation() method.
* 
* [Must Know]
* 1. This script must be attached to the child gameObject of the animator.
*/

public class DoorAnimations : MonoBehaviour, IInteractable
{
    #region PRIVATE_VARIABLES
    Animator doorAnimator;
    bool isOpen = false;
    #endregion

    private void Awake()
    {
        //Cache the animator attached to the parent gameObject
        doorAnimator = transform.parent.GetComponent<Animator>();
    }

    /// <summary>
    /// Call to play:
    /// <para>A. The OpenDoor animation if the door is closed.</para> 
    /// <para>B. The CloseDoor animation if the door is opened.</para>
    /// </summary>
    void PlayDoorAnimation()
    {
        if (isOpen)
        {
            PlayCloseAnimation();
            isOpen = false;
        }
        else
        {
            PlayDoorOpenAnimation();
            isOpen = true;
        }
    }

    /// <summary>
    /// Call to play the DoorClose animation.
    /// </summary>
    void PlayCloseAnimation()
    {
        doorAnimator.Play("DoorClose");
    }

    /// <summary>
    /// Call to play the DoorOpen animation and then play the Door Interaction audio clip
    /// through the GameManager.S.GameSoundsHandler...
    /// </summary>
    void PlayDoorOpenAnimation()
    {
        doorAnimator.Play("DoorOpen");
        GameManager.S.GameSoundsHandler.PlayAudio(GameSounds.DoorInteract);
    }

    /// <summary>
    /// *EXTERNALLY USED*
    /// <para>Call to play:</para>
    /// <para>A. The OpenDoor animation if the door is closed.</para> 
    /// <para>B. The CloseDoor animation if the door is opened.</para>
    /// </summary>
    public void Interact()
    {
        PlayDoorAnimation();
    }
}
