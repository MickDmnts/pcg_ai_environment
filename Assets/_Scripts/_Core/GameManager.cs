using UnityEngine;
using UnityEngine.SceneManagement;

/* CLASS DOCUMENTATION *\
     * [Variable Specifics]
     * 1. Every variable in this class is public and represents the X manager of the X system.
     * 
     * [Class Flow]
     * 1. The only purpose of this class file is to house every system manager reference of the game.
     * 
     * [Must Know]
     * 1. The ony way to access the GameManager and EACH manager is through the static S variable.
     * 
*/

[DefaultExecutionOrder(10)]
public class GameManager : MonoBehaviour
{
    //Game Manager singleton
    public static GameManager S;

    #region MANAGER_PUBLIC_CACHE                
    public GameEventsHandler GameEventsHandler { get; private set; }
    public GameSoundsHandler GameSoundsHandler { get; private set; }
    public UIManager UIManager { get; private set; }
    #endregion

    private void Awake()
    {
        //Enable VSync
        QualitySettings.vSyncCount = 1;

        if (S != this || S == null)
        {
            S = this;
        }

        CacheManagers();
    }

    /// <summary>
    /// Call to cache the needed managers of the game
    /// </summary>
    void CacheManagers()
    {
        GameEventsHandler = new GameEventsHandler();
        GameSoundsHandler = GetComponent<GameSoundsHandler>();
        UIManager = FindObjectOfType<UIManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadScene();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    /// <summary>
    /// Call to reload the scene with build index 0
    /// </summary>
    public void ReloadScene()
    {
        SceneManager.LoadScene(0);
    }

    private void OnDestroy()
    {
        //Nullify the singleton ref so we don't get NullRef errors.
        S = null;
    }
}
