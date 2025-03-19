using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Loading : MonoBehaviour
{
    public GameObject loadingBar;
    public Image loadingBarFill;
    private AsyncOperation operation;
    private float fakeProgress = 0f;
    public TextMeshProUGUI playerTotalAttributes;
    public TextMeshProUGUI enemyTotalAttributes;

    public GameObject resultBtn;
    public TextMeshProUGUI resultText;

    public void Load()
    {
        List<SpriteRenderer> playerModelParts = PKController.instance.playerModelParts;
        if (playerModelParts[6].sprite == null && playerModelParts[7].sprite == null && playerModelParts[8].sprite == null && playerModelParts[9].sprite == null && playerModelParts[10].sprite == null)
        {
            return;
        }
        PKController.instance.PK();
        StartCoroutine(LoadDressUpAsync());
    }

    IEnumerator LoadDressUpAsync()
    {
        fakeProgress = 0f;
        loadingBar.SetActive(true);

        while (fakeProgress < 1f)
        {
            fakeProgress += Time.deltaTime * 0.5f;
            loadingBarFill.fillAmount = fakeProgress;
            yield return null;
        }

        PKController.instance.loading.SetActive(false);

        int playerPoint = PKController.instance.CalculateTotalAttributes();
        int enemyPoint = PKController.instance.RandomItemsForEnemy();

        StartCoroutine(IncrementNumber(playerTotalAttributes, playerPoint, 2f));
        StartCoroutine(IncrementNumber(enemyTotalAttributes, enemyPoint, 2f));

        yield return new WaitForSeconds(4f);
        PKController.instance.enemy.SetActive(false);
        enemyTotalAttributes.text = "";
        playerTotalAttributes.text = "";
        PKController.instance.player.transform.DOMove(new Vector3(0, 2, 0), 1f).SetEase(Ease.OutQuint);
        PKController.instance.player.transform.DOScale(0.85f, 1f).SetEase(Ease.OutQuint);
        if (playerPoint > enemyPoint)
        {
            resultText.text = LanguagesManager.instance.GetLocalizedString("win_key");
            SoundController.instance.PlayWinSound();
            PKController.instance.winstreak++;
            PKController.instance.SaveWinStreak();
        }
        else
        {
            resultText.text = LanguagesManager.instance.GetLocalizedString("lose_key");
            SoundController.instance.PlayLoseSound();
            PKController.instance.winstreak = 0;
            PKController.instance.SaveWinStreak();
        }
        resultBtn.SetActive(true);
        int selectedLanguage = PlayerPrefs.GetInt("Language", 0);
        LanguagesManager.instance.ChangeLanguage(selectedLanguage);
    }

    private IEnumerator IncrementNumber(TextMeshProUGUI point, int target, float duration)
    {
        int currentNumber = 1;
        float timeInterval = duration / target;

        while (currentNumber <= target)
        {
            point.text = currentNumber.ToString();
            currentNumber++;

            yield return new WaitForSeconds(timeInterval);
        }
    }
}
