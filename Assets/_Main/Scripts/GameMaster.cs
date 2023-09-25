using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    [SerializeField] private AudioClip uiButtonSound;
    [SerializeField] private RectTransform gameTitleUI;
    [SerializeField] private RectTransform gameOverUI;
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private RectTransform gamePausedUI;
    [SerializeField] private RectTransform enemyNumberUI;
    [SerializeField] private Image transitionImage;
    private float transitionColorAlpha;
    public int enemiesKilled;

    private void Awake()
    {
        enemiesKilled = 0;
        FreezeTime();
        gameTitleUI.gameObject.SetActive(true);
    }

    public void StartGame()
    {
        UnfreezeTime();
        AudioSource.PlayClipAtPoint(uiButtonSound, Camera.main.transform.position);
        StartCoroutine(PlayGameStartAnimation());
    }

    public void SetPlayerGameOver()
    {
        gameOverUI.gameObject.SetActive(true);
        gameOverText.text = "Good Game! Enemies Killed: " + enemiesKilled;
        FreezeTime();
    }

    public void PauseGame()
    {
        AudioSource.PlayClipAtPoint(uiButtonSound, Camera.main.transform.position);
        gamePausedUI.gameObject.SetActive(true);
        FreezeTime() ;
    }

    public void ResumeGame()
    {
        AudioSource.PlayClipAtPoint(uiButtonSound, Camera.main.transform.position);
        gamePausedUI.gameObject.SetActive(false);
        UnfreezeTime();
    }

    private void FreezeTime()
    {
        Time.timeScale = 0f;
    }

    private void UnfreezeTime()
    {
        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
        AudioSource.PlayClipAtPoint(uiButtonSound, Camera.main.transform.position);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    public void ExitGame()
    {
        AudioSource.PlayClipAtPoint(uiButtonSound, Camera.main.transform.position);
        Application.Quit();
    }

    private IEnumerator PlayGameStartAnimation()
    {
        yield return StartCoroutine(TransitionFadeOut(1f, Color.black));
        yield return new WaitForSeconds(0.5f);
        gameTitleUI.gameObject.SetActive(false);
        yield return StartCoroutine(TransitionFadeIn(1f, Color.black));
    }

    private IEnumerator TransitionFadeIn(float fadeInSec, Color transitionColor)
    {
        for (float time = 0; time < fadeInSec; time = time + Time.deltaTime)
        {
            transitionColorAlpha = 1f - time / fadeInSec;
            transitionImage.color = new Color(transitionColor.r, transitionColor.g, transitionColor.b, transitionColorAlpha);
            yield return new WaitForSeconds(0);
        }
    }

    private IEnumerator TransitionFadeOut(float fadeOutSec, Color transitionColor)
    {
        for (float time = 0; time < fadeOutSec; time = time + Time.deltaTime)
        {
            transitionColorAlpha = time / fadeOutSec;
            transitionImage.color = new Color(transitionColor.r, transitionColor.g, transitionColor.b, transitionColorAlpha);
            yield return new WaitForSeconds(0);
        }
    }

    private IEnumerator TransitionCrossFade(float fadeOutSec, float fadeInSec, float blackOutSec)
    {
        for (float time = 0; time < fadeOutSec; time = time + Time.deltaTime)
        {
            transitionColorAlpha = time / fadeOutSec;
            transitionImage.color = new Color(Color.black.r, Color.black.g, Color.black.b, transitionColorAlpha);
            yield return new WaitForSeconds(0);
        }

        yield return new WaitForSeconds(blackOutSec);

        for (float time = 0; time < fadeInSec; time = time + Time.deltaTime)
        {
            transitionColorAlpha = 1f - time / fadeInSec;
            transitionImage.color = new Color(Color.black.r, Color.black.g, Color.black.b, transitionColorAlpha);
            yield return new WaitForSeconds(0);
        }
    }
}
