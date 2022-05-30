using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator fade;
    public void LoadNextLevel(int index)
    {
        StartCoroutine(LoadLevel(index));
    }

    public IEnumerator LoadLevel(int levelIndex)
    {
        fade.SetTrigger("Start");
        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(levelIndex);
    }
}
