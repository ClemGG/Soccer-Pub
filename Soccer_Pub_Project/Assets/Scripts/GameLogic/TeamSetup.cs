using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeamSetup : MonoBehaviour {

    //On utilise cette classe pour passer les paramètres visuels (meshes, mats, etc...) des équipes choisies d'une scène à l'autre



    public List<int> listOfConnectedPlayers;
    public List<Team> listOfSelectedTeams;


    public static TeamSetup instance;







    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Erreur : Il y a plus d'un script TeamSetup dans la scène.");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        //Empêche les donées stockées d'être perdues une fois l'écran de sélection quitté
        DontDestroyOnLoad(gameObject);

        listOfConnectedPlayers = new List<int>();
        listOfSelectedTeams = new List<Team>();

    }


    private void OnLevelWasLoaded(int level)
    {
        //Une fois arrivé dans la scène de jeu, on met en place les paramètres visuels et on connecte les IAs à leurs joueurs correspondants
        if(level == 2)
        {
            SetUpBothTeams();
            Destroy(gameObject);
        }
    }

    private void SetUpBothTeams()
    {
        PlayerManager.instance.setupTeams = listOfSelectedTeams;
        PlayerManager.instance.connectedPlayers = listOfConnectedPlayers;
    }
}

