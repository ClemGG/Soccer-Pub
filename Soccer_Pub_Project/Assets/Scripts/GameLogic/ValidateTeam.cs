using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ValidateTeam : MonoBehaviour {

    [SerializeField] private Button playButton;
    [SerializeField] private Text errorMsgText;

    [SerializeField] private List<UiInputs> players;

    [Space(20)]


    [SerializeField] private Transform toggles;
    [SerializeField] private List<Toggle> togglesOn;

    [Space(20)]

    [SerializeField] private string[] manettes;







    public static ValidateTeam instance;







    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Erreur : Il y a plus d'un script ValidateTeam dans la scène.");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        MainMenuButtons.instance.OnPlayEvent += GetActivePlayers;
    }

    // Use this for initialization
    void Start () {

        //Ces deux fonctions nous permettent de détecter le nombre de joueurs présents, et la (dé)connexion des manettes.
        //Etant donné qu'elles n'ont pas besoin d'être appelées régulièrement, on les appelle en continu en dehors de l'Update, avec un délai d'itération plus élevé afin d'éviter les calculs inutiles
        InvokeRepeating("CheckControllersState", 0f, 1f);
        InvokeRepeating("OnAllTogglesOn", 0f, .5f);


        //On récupère de base le nombre de manettes connectées au lancement de la scène
        manettes = InitialzeControllerArray();
        


        if(manettes.Length == 0)
        {
            errorMsgText.enabled = true;
            errorMsgText.text = "Aucune manette n'est connectée.";
            return;
        }
        else
        {
            errorMsgText.enabled = false;
        }

        //TestControllers(manettes);

        //On récupère de base les toogles correspondants aux manettes déjà connectées
        SetActiveToggles(manettes);

    }





    public void GetActivePlayers()
    {
        //On récupère ici les index des toggles activés (correspondant aux manettes connectées) et on leur ajoute 1 pour obtenir les numéros de chaque manette
        for (int i = 0; i < toggles.childCount; i++)
        {
            if (toggles.GetChild(i).gameObject.activeSelf)
            {
                TeamSetup.instance.listOfConnectedPlayers.Add(i + 1);
            }
        }

        MainMenuButtons.instance.OnPlayEvent -= GetActivePlayers;

    }









    private string[] InitialzeControllerArray()
    {
        //Cette fonction crée un nouveau tableau qui contiendra la liste de manettes actuellement connectées
        //Si certains emplacement de manettes sont vides, cela signifie que la manette n'est pas connectée ; on garde alors l'emplacement vide pour représenter les manettes absentes / déconnectées.

        string[] manettes = new string[toggles.childCount];

        if (Input.GetJoystickNames().Length == 0)
        {
            for (int i = 0; i < manettes.Length; i++)
            {
                manettes[i] = string.Empty;
            }
        }
        else
        {

            for (int i = 0; i < manettes.Length; i++)
            {
                if (i < Input.GetJoystickNames().Length)
                {
                    if (Input.GetJoystickNames()[i] != string.Empty)
                    {
                        manettes[i] = Input.GetJoystickNames()[i];
                    }
                }
                else
                {
                    manettes[i] = string.Empty;
                }
            }
        }
        return manettes;
    }







    private void SetActiveToggles(string[] connectedControllers)
    {
        /*
         * On parcourt la liste des manettes connectées, et on en compare chaque élément pour vérifier s'il est vide.
         * Si c'est le cas, alors la manette n'est pas connectée et on désactive le toggle correspondant.
         * Sinon, on l'active pour permettre au joueur de confirmer sa sélection d'équipe.
         */

        togglesOn = new List<Toggle>();

        for (int i = 0; i < toggles.childCount; i++)
        {
            Toggle t = toggles.GetChild(i).GetComponent<Toggle>();

            if (connectedControllers[i] != string.Empty)
            {
                togglesOn.Add(t);
            }
            //print(connectedControllers[i]);

            t.gameObject.SetActive(connectedControllers[i] != string.Empty);
            t.isOn = i > 2;
        }


    }














    private void CheckControllersState()
    {
        //Cette fonction permet de vérifier que des manettes n'ont pas été (dé)connectées depuis le lancement de la scène

        //On crée un tableau temporaire qui contient la nouvelle liste de manettes connectées
        string[] currentControllers = InitialzeControllerArray();
        bool changeList = false;


        //print(currentControllers.Length);
        //print(manettes.Length);


        //On compare currentControllers à manettes, et si un seul élement diffère entre les deux tableaux, on change la liste établie
        for (int i = 0; i < currentControllers.Length; i++)
        {
            //print(currentControllers[i] + " : " + i);
            //print(manettes[i] + " : " + i);

            if(currentControllers[i] == null)
            {
                currentControllers[i] = string.Empty;
            }

            if(currentControllers[i] != manettes[i])
            {
                changeList = true;
                break;
            }
        }


        //Si la liste doit être changée, on active les toggles en fonction de la liste temporaire, puis on remplace l'ancienne liste de manettes connectées par la nouvelle
        if(changeList)
        {
            SetActiveToggles(currentControllers);
            manettes = currentControllers;

            DisplayErrorMsg();

            if (manettes[0] == string.Empty)
                return;

            //Petite mesure de sécurité pour s'assurer que le joueur 1 puisse à nouveau sélectionner les boutons à l'écran
            if (togglesOn.Count == 1)
            {

                if (!players[0].eventSystem.currentSelectedGameObject)
                {
                    players[0].eventSystem.SetSelectedGameObject(players[0].t.gameObject);
                }
            }
        }


    }



    private void OnAllTogglesOn()
    {

        //Cette fonction permet de vérifier que tous les joueurs connectés ont confirmé leur équipe et sont prêts à jouer

        //Cette boucle permet de vérifier si tous les toggles actifs ont été cochés
        bool b = true;

        for (int i = 0; i < togglesOn.Count; i++)
        {
            if (!togglesOn[i].isOn)
            {
                b = false;
                break;
            }
        }


        //Si c'est le cas
        if (b)
        {
            


            for (int i = 0; i < players.Count; i++)
            {

                //Petite vérification pour que les joueurs 3 et 4 (qui n'ont pas besoin d'UI) ne perturbent pas le système
                if (!players[i].left && !players[i].right && !players[i].t && !players[i].eventSystem)
                {
                    //On ne prendra en compte que le joystickNumber du joueur
                    continue;
                }

                //On désactive l'eventSystem du joueur 2 (et les éventuels des joueurs 3, 4, etc...) pour qu'ils ne puissent plus changer d'équipe si leur toggle est coché
                if (i >= 1)
                {
                    players[i].eventSystem.SetSelectedGameObject(null);

                }
            }

            
            //On active le bouton Play pour démarrer le match. Seul le joueur 1 pour l'activer
            playButton.interactable = true;
            players[0].eventSystem.SetSelectedGameObject(playButton.gameObject);
        }
        else
        {
            for (int i = 0; i < players.Count; i++)
            {
                //Petite vérification pour que les joueurs 3 et 4 (qui n'ont pas besoin d'UI) ne perturbent pas le système
                if (!players[i].left && !players[i].right && !players[i].t && !players[i].eventSystem)
                {
                    //On ne prendra en compte que le joystickNumber du joueur
                    continue;
                }

                

                //Si tous les joueurs n'ont pas validé, on s'assure que ceux qui l'ont fait ne puissent pas sélectionner d'autres boutons que leur toggles
                if (players[i].t.isOn)
                {
                    if (i >= 1)
                    {
                        players[i].eventSystem.SetSelectedGameObject(players[i].t.gameObject);

                    }
                }
            }
            
            //On désactive le bouton Play
            playButton.interactable = false;
        }




        for (int i = 0; i < players.Count; i++)
        {
            players[i].LockInputsWhenReady(b);
        }

    }









    private void DisplayErrorMsg()
    {
        
        //Si la liste de manettes est vide, aucune manette n'est connectée
        if (togglesOn.Count == 0)
        {
            errorMsgText.enabled = true;
            errorMsgText.text = "Aucune manette n'est connectée.";
            return;
        }
        else
        {
            //Sinon, si le joueur 1 est déconnectée, on affiche un message exigeant que le joueur 1 soit connecté (car il est le seul à pouvoir lancer la partie)
           if(manettes[0] == string.Empty)
            {
                errorMsgText.enabled = true;
                errorMsgText.text = "Manette du Joueur 1 déconnectée.";
                return;
            }
            else
            {
                errorMsgText.enabled = false;
            }
        }
    }


    private void TestControllers(string[] manettes)
    {
        //Une fonction de test pour afficher le nom de chaque manette connectée ainsi que leur index
        for (int i = 0; i < manettes.Length; i++)
        {
            print(string.Format("{0} : {1}", manettes[i], i));

        }
    }
}
