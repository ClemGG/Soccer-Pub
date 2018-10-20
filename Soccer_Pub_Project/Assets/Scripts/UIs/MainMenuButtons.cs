
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour {

    [SerializeField] private GameObject controlsPanel;
    
    public static MainMenuButtons instance;

    public event Action OnPlayEvent;


    private void Awake()
    {



        if (instance != null)
        {
            Debug.LogError("Erreur : Il y a plus d'un script MainMenuButtons dans la scène.");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }



        if (!controlsPanel)
            return;

        if(controlsPanel.activeSelf == true)
        {
            controlsPanel.SetActive(false);
        }


    }














    private void OnLevelWasLoaded(int level)
    {
        Time.timeScale = 1f;
    }


    public void PlayGame()
    {
        if(OnPlayEvent != null)
        {
            OnPlayEvent();
        }
        
        SceneFader.instance.FadeToScene(2);
    }

    public void GoToSelectionMenu()
    {
        SceneFader.instance.FadeToScene(1);
    }

    public void ReturnToMainMenu()
    {
        SceneFader.instance.FadeToScene(0);
    }

    public void ShowControls()
    {
        controlsPanel.SetActive(!controlsPanel.activeSelf);
    }

    public void Quit()
    {
        SceneFader.instance.FadeToQuitScene();
    }
}
