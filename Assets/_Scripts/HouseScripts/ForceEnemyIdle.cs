using UnityEngine;

/* CLASS DOCUMENTATION *\
* [Variable Specifics]
* Dynamically - Private changed: These variables are changed throughout the game.
* 
* [Class Flow]
* 1. The only purpose of this class file is to force the enemy to go back to his original position.
* 
* [Must Know]
* 1. The OnTriggerEnter(...) can be activated only with the "Player" tag
* 2. A box collider with trigger its trigger set to true must be attached.
*/

[RequireComponent(typeof(BoxCollider))]
public class ForceEnemyIdle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Find the EnemyBehaviour script of the level and call its ForceBackToOriginalPosState() method.
            FindObjectOfType<EnemyBehaviour>().ForceBackToOriginalPosState();
        }
    }
}
