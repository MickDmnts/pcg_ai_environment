using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// All the available game sounds.
/// Each enum number must be the same as the audioClips list index
/// for the system to work correctly.
/// </summary>
public enum GameSounds
{
    ClickSound = 0,
    DoorInteract = 1,
}

/* CLASS DOCUMENTATION *\
    * [Variable Specifics]
    * Inspector values: Inspector values must be set from the editor inpsector for the script to work correctly
    * Dynamically changed: These variables are changed throughout the game.
    * 
    * [Class Flow]
    * 1. The only purpose of this class file is to have a public method that other scripts can use to play
    *   a game audio clip.
    * 
    * [Must Know]
    * 1. The manager is accessible through GameManager.S.GameSoundsHandler field.
    * 2. The PlayAudio(...) method plays the passed sound as ONE SHOT.
    * 
*/

public class GameSoundsHandler : MonoBehaviour
{
    [Header("Set audio clips in inspector")]
    [SerializeField] List<AudioClip> audioClips;

    //Private variables
    AudioSource mainAudioSource;

    private void Awake()
    {
        mainAudioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Call to play the passed audio sound as ONE SHOT.
    /// </summary>
    /// <param name="sound">The audio clip to play from the audioClips List</param>
    public void PlayAudio(GameSounds sound)
    {
        if ((int)sound > audioClips.Count - 1)
        {
            Debug.Log("The passed sound index is greater than the list count." +
                "\nSound activation cancelled");
        }
        else if ((int)sound < 0)
        {
            Debug.Log("The passed sound index is smaller than 0, Passed sound index cannot be 0.");
        }
        else
        {
            mainAudioSource.PlayOneShot(audioClips[(int)sound]);
        }

    }
}
