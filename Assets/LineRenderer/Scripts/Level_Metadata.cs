using UnityEngine;

public class Level_Metadata : MonoBehaviour
{
    public int totalCorrectLinks;
    public int noOfCorrectLinks;
    public PlushieActiveStitchPart plushieActivePartToStitch;
    void CameraFocus(PlushieActiveStitchPart currentActivePart)
    {
        switch(currentActivePart)
        {
            case PlushieActiveStitchPart.righteye:
                break;
            case PlushieActiveStitchPart.lefteye:
                break;
            case PlushieActiveStitchPart.rightear:
                break;
            case PlushieActiveStitchPart.leftear:
                break;
            case PlushieActiveStitchPart.rightarm:
                break;
            case PlushieActiveStitchPart.leftarm:
                break;
            case PlushieActiveStitchPart.rightleg:
                break;
            case PlushieActiveStitchPart.leftleg:
                break;
        }
    }
}
