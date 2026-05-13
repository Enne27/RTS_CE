using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AntButton
{
    public Button buttonComponent;
    public Ant antScript;
    public GameObject previewModel;
    public string antName;
}

public class ReferencesAntPreview : MonoBehaviour
{
    [Header("References for broodChamber")]
    public RenderTexture previewTexture;
    public List<AntButton> antsButton;

}
