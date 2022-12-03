using UnityEngine;

/* CLASS DOCUMENTATION *\
* [Variable Specifics]
* Dynamically changed: These variables are changed throughout the game.
* 
* [Class Flow]
* 1. The only purpose of this class file is to handle the pressure plate animation and sound events.
* 
* [Must know]
* 1. Animation and sound events can be activated only by the player.
*/

public class PressurePlateActivator : MonoBehaviour
{
    //Private variables
    Animator pressurePlateAnimator;

    private void Start()
    {
        //Cache the parent transform animator component.
        pressurePlateAnimator = transform.parent.GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Play the PlateDown animation and the plate click sound
            //when the player enters the trigger volume area.
            pressurePlateAnimator.Play("PlateDown");
            GameManager.S.GameSoundsHandler.PlayAudio(GameSounds.ClickSound);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Play the PlateUp animation and the plate click sound
            //when the player exits the trigger volume area.
            pressurePlateAnimator.Play("PlateUp");
            GameManager.S.GameSoundsHandler.PlayAudio(GameSounds.ClickSound);
        }
    }
}
