using UnityEngine;

using Cinemachine;

/* CLASS DOCUMENTATION *\
* [Variable Specifics]
* Inspector values: Inspector values must be set from the editor inpsector for the script to work correctly
* Dynamically changed: These variables are changed throughout the game.
* 
* [Class Flow]
* 1. The Update() method of this class creates a ray and checks for IInteractable ray hits, in case it finds one it enables a
*   UI prompt to prompt the user to press a button. If pressed, it calls the hit gameObjects Interact() method through the 
*   IInteractable interface.
*/

public class EnvironmentInteractions : MonoBehaviour
{
    [Header("Set in inspector")]
    [SerializeField] CinemachineVirtualCamera playerCamera;
    [SerializeField] float rayLength;

    private void Awake()
    {
        //Cache the gameObject marked with the "PlayerCamera" tag.
        playerCamera = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        //Create the forward pointing ray
        Ray interactionRay = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hitInfo;

        //Cast the ray...
        if (Physics.Raycast(interactionRay, out hitInfo, rayLength))
        {
            //...if the ray hits an Interactable tagged gameObject
            if (hitInfo.transform.CompareTag("Interactable"))
            {
                GameManager.S.UIManager.ChangePromptText("Interact with E");

                //...and the player presses the E button...
                if (Input.GetKeyDown(KeyCode.E))
                {
                    //...Cache the interaction script of the hit gameObject...
                    IInteractable interaction = hitInfo.collider.GetComponent<IInteractable>();

                    if (interaction != null)
                    {
                        //...and finally call its Interact() method.
                        interaction.Interact();
                    }
                }
            }
        }
        else
        {
            GameManager.S.UIManager.ChangePromptText(System.String.Empty);
        }
    }
}
