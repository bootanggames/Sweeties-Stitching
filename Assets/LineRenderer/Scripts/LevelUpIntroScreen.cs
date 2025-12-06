using System.Collections;
using UnityEngine;

public class LevelUpIntroScreen : MonoBehaviour
{
    [SerializeField] GameObject[] plishieObj;

    private void OnEnable()
    {
        StopCoroutine(nameof(EnableOneByOne));
        StartCoroutine(nameof(EnableOneByOne));
    }

    IEnumerator EnableOneByOne()
    {
        for(int i = 0; i < plishieObj.Length; i++)
        {
            plishieObj[i].SetActive(true);
            yield return new WaitForSeconds(0.25f);
        }
        yield return new WaitForSeconds(1.0f);
        var levelUpScreen = ServiceLocator.GetService<ILevelUpScreen>();
        if (levelUpScreen != null)
        {
            levelUpScreen.StopSound();
            levelUpScreen.homeScreen.EnableSound();
            levelUpScreen.renderTextureImageObj.SetActive(false);
            levelUpScreen.levelUpScreen.SetActive(false);
            levelUpScreen.confettiCameraRenderObj.SetActive(false);
        }
        this.gameObject.SetActive(false);
    }
}
