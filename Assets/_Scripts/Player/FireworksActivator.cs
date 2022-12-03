using UnityEngine;

/* CLASS DOCUMENTATION *\
* [Variable Specifics]
* Inspector values: Inspector values must be set from the editor inpsector for the script to work correctly
* 
* [Class Flow]
* 1. This script handles the firework instantiation when the Player enters its box trigger.
*/

[RequireComponent(typeof(BoxCollider))]
public class FireworksActivator : MonoBehaviour
{
    [Header("Set in inspector")]
    [SerializeField] GameObject fireworksPrefab;
    [SerializeField] Transform firePoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Instantiate a fireworks prefab in the firePoint position.
            Instantiate(fireworksPrefab, firePoint.position, Quaternion.identity, null);
        }
    }
}
