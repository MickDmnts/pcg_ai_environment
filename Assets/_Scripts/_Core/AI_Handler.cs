using System.Collections;
using UnityEngine;
using Unity.AI.Navigation;

/* CLASS DOCUMENTATION *\
* [Variable Specifics]
* Inspector values: Inspector values must be set from the editor inpsector for the script to work correctly
* Dynamically changed: These variables are changed throughout the game.
* 
* [Class Flow]
* 1. The only purpose of this class file is to Instantiate the enemy inside the BigHouse gameObject and build the AI NavMesh in runtime
*  WHEN the MapGeneration finishes.
* 
* [Must Know]
* 1. StartAICoroutine gets called when the onMapGenerationFinish is invoked.
* 
*/

public class AI_Handler : MonoBehaviour
{
    [Header("Set in inspector")]
    [SerializeField] GameObject enemyPrefab;

    //Private Variables
    GameObject bigHousePos;

    private void Start()
    {
        if (GameManager.S != null)
        {
            //Invoked when the map generation finishes
            GameManager.S.GameEventsHandler.onMapGenerationFinish += StartAICoroutine;
        }
    }

    #region EVENT_INVOKED

    /// <summary>
    /// Call to start the AI Creation and NavMesh Baking generation on the map.
    /// </summary>
    void StartAICoroutine()
    {
        StartCoroutine(InitiateAI());
    }

    /// <summary>
    /// Invoke to start the AI Creation and NavMesh Baking generation on the map.
    /// </summary>
    IEnumerator InitiateAI()
    {
        //Cache the BigHouse gameobject in runtime
        bigHousePos = GameObject.FindGameObjectWithTag("BigHouse");

        //Cache the first created tile of the generated map
        NavMeshSurface navMeshSurface = FindObjectOfType<MapGenerator>().GetNavMeshSurface();

        GameManager.S.UIManager.ChangeInfoText("Generating AI NavMesh...");
        yield return new WaitForSeconds(.1f);

        //... and finally bake the navMesh in runtime - Uses Experimental NavMesh Components
        navMeshSurface.BuildNavMesh();

        GameManager.S.UIManager.ChangeInfoText(System.String.Empty);

        //Initialize the enemy
        if (bigHousePos != null)
        {
            Instantiate(enemyPrefab, bigHousePos.transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("BigHouse object not found\n" +
                "Can't spawn enemy");
        }

        yield return null;
    }
    #endregion

    private void OnDestroy()
    {
        if (GameManager.S != null)
        {
            GameManager.S.GameEventsHandler.onMapGenerationFinish -= StartAICoroutine;
        }
    }
}
