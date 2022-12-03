using UnityEngine;

using TMPro;

/* CLASS DOCUMENTATION *\
* [Variable Specifics]
* Inspector values: Inspector values must be set from the editor inpsector for the script to work correctly
* 
* [Class Flow]
* 1. The only purpose of this class file is to have public methods that can be externally accessed to change each UI element text field string.
* 
* [Must Know]
* 1. The manager is accessible through GameManager.S.UIManager field.
*/

public class UIManager : MonoBehaviour
{
    [Header("Set in inspector")]
    [SerializeField] TextMeshProUGUI infoText;
    [SerializeField] TextMeshProUGUI promptText;

    /// <summary>
    /// Call to set the UI Element text (Info Text) to the passed string. 
    /// </summary>
    public void ChangeInfoText(string text)
    {
        infoText.SetText(text);
    }

    /// <summary>
    /// Call to set the UI Element text (Prompt Text) to the passed string. 
    /// </summary>
    public void ChangePromptText(string text)
    {
        promptText.SetText(text);
    }
}
