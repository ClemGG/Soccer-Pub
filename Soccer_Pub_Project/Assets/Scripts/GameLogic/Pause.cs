using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour {

    public bool isGamePaused = false;
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject scoreCanvas;


    public static Pause instance;


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Erreur : Il y a plus d'un script Pause dans la scène.");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        pauseCanvas.SetActive(false);
    }





    public void PauseGame()
    {

        isGamePaused = !isGamePaused;
        pauseCanvas.SetActive(!pauseCanvas.activeSelf);
        scoreCanvas.SetActive(!scoreCanvas.activeSelf);

        if (isGamePaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}
