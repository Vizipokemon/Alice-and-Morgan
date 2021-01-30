using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeScene : MonoBehaviour
{
    [SerializeField] private Animator transition;

    public void LoadGameScene()
    {
        StartCoroutine(LoadGameSceneWithFade());
    }

    public void LoadMenuSceneFromGameScene()
    {
        StartCoroutine(LoadMenuSceneWithFade());
    }

    private IEnumerator LoadGameSceneWithFade()
    {
        transition.SetTrigger("StartFade");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private IEnumerator LoadMenuSceneWithFade()
    {
        transition.SetTrigger("StartSlowFade");
        yield return new WaitForSeconds(10f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
