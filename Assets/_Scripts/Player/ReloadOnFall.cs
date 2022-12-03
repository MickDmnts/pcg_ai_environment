using UnityEngine;

/*[CLASS DOCUMENTATION]
 * 
 * This script is attached on the player and reloads the level if the player falls off the generated map.
 * 
 */

public class ReloadOnFall : MonoBehaviour
{
    private void Update()
    {
        if (transform.position.y <= -20f)
        {
            GameManager.S.ReloadScene();
        }
    }
}
