using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Image progressBar;
    public GameObject player;
    void OnEnable()
    {
        player.GetComponent<PlayerController>().ProgressTo += ShowProgress;
    }

    void OnDisable()
    {
        player.GetComponent<PlayerController>().ProgressTo -= ShowProgress;
    }

    private void ShowProgress(float val)
    {
        progressBar.fillAmount = val;
    }
}
