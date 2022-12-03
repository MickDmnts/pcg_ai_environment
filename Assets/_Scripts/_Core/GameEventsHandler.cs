using System;

/* CLASS DOCUMENTATION *\
 * [Must Know]
 * 1. The manager is accessible through GameManager.S.GameEventsHandler field.
 * 2. The sole purpose of this class file is to act as a central hub for all game-wide events.
*/

public class GameEventsHandler
{
    /// <summary>
    /// Called when the map generation finishes generating the map.
    /// </summary>
    public Action onMapGenerationFinish;
    public void OnMapGenerationFinish()
    {
        if (onMapGenerationFinish != null)
        {
            onMapGenerationFinish();
        }
    }
}
