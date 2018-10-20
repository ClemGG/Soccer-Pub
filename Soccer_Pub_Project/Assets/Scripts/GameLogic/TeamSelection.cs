using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TeamSelection : MonoBehaviour {

    [SerializeField] private Transform j1;
    [SerializeField] private Transform j2;

    [Space(20)]

    [SerializeField] private Image flag1;
    [SerializeField] private Image flag2;


    [SerializeField] private int indexOfSelectedTeam1;
    [SerializeField] private int indexOfSelectedTeam2;

    public Team currentSelectedTeam1;
    public Team currentSelectedTeam2;

    [Space(20)]

    [SerializeField] private List<Team> teams;


    public static TeamSelection instance;







    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Erreur : Il y a plus d'un script TeamSelection dans la scène.");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }



    }

    private void Start () {

        if(teams.Count < 2)
        {
            Debug.LogError("Erreur : Il y a moins de deux équipes dans la liste d'équipes disponibles.");
            return;
        }

        InitializeUIS();
        MainMenuButtons.instance.OnPlayEvent += GetSelectedTeams;
    }





    public void GetSelectedTeams()
    {
        TeamSetup.instance.listOfSelectedTeams = new List<Team> { currentSelectedTeam1, currentSelectedTeam2 };
        MainMenuButtons.instance.OnPlayEvent -= GetSelectedTeams;
    }








    private void InitializeUIS()
    {

        currentSelectedTeam1 = teams[0];
        currentSelectedTeam2 = teams[1];

        flag1.sprite = currentSelectedTeam1.flagToDisplay;
        flag2.sprite = currentSelectedTeam2.flagToDisplay;

        for (int i = 0; i < j1.childCount; i++)
        {
            if (i != currentSelectedTeam1.indexOfMeshToDisplay)
            {
                j1.GetChild(i).gameObject.SetActive(false);
            }
            else
            {
                j1.GetChild(i).gameObject.SetActive(true);
            }
        }
        for (int i = 0; i < j2.childCount; i++)
        {
            if (i != currentSelectedTeam2.indexOfMeshToDisplay)
            {
                j2.GetChild(i).gameObject.SetActive(false);
            }
            else
            {
                j2.GetChild(i).gameObject.SetActive(true);
            }
        }

        indexOfSelectedTeam1 = 0;
        indexOfSelectedTeam2 = 1;

    }


    private void UpdateUIS(int nb, Team selectedTeam)
    {
        //On regarde quelle équipe a appelé la fonction, et on lui applique les paramètres de la nouvelle équipe correspondante.
        //On met également à jour l'index correspondant.
        switch (nb)
        {
            case 1:
                flag1.sprite = selectedTeam.flagToDisplay;
                for (int i = 0; i < j1.childCount; i++)
                {
                    if(i != selectedTeam.indexOfMeshToDisplay)
                    {
                        j1.GetChild(i).gameObject.SetActive(false);
                    }
                    else
                    {
                        j1.GetChild(i).gameObject.SetActive(true);
                    }
                }
                indexOfSelectedTeam1 = teams.IndexOf(selectedTeam);
                currentSelectedTeam1 = selectedTeam;
            break;


            case 2:
                flag2.sprite = selectedTeam.flagToDisplay;
                for (int i = 0; i < j2.childCount; i++)
                {
                    if (i != selectedTeam.indexOfMeshToDisplay)
                    {
                        j2.GetChild(i).gameObject.SetActive(false);
                    }
                    else
                    {
                        j2.GetChild(i).gameObject.SetActive(true);
                    }
                }
                indexOfSelectedTeam2 = teams.IndexOf(selectedTeam);
                currentSelectedTeam2 = selectedTeam;
                break;
        }
    }











    public void UpdateSelectionTeam1(int one)
    {

        //'one' est une valeur donnée par les boutons de l'UI. En fonction de son signe, on incrémentera ou décrémentera l'index parcourant le tableau
        indexOfSelectedTeam1 += one;



        //Pour s'assurer que l'index ne sorte jamais des limites du tableau
        if (indexOfSelectedTeam1 == teams.Count)
        {
            indexOfSelectedTeam1 = 0;
        }
        else if (indexOfSelectedTeam1 == -1)
        {
            indexOfSelectedTeam1 = teams.Count - 1;
        }

        //Si on veut que les deux joueurs puissent sélectionner la même équipe :

        //UpdateUIS(1, teams[indexOfSelectedTeam1]);



        //Si on veut que les deux joueurs ne puissent pas sélectionner la même équipe :



        for (int i = 0; i < teams.Count; i++)
        {
            //On parcourt le tableau jusqu'à trouver une case inutilisée
            int index = indexOfSelectedTeam1 + i * one;


            //Pour s'assurer que l'index ne sorte jamais des limites du tableau
            if (index == teams.Count)
            {
                index = 0;
                i = 0;
            }
            else if (index == -1)
            {
                index = teams.Count - 1;
                i = 0;
            }


            //Si on a trouvé une équipe qui n'est pas déjà choisie par un des joueurs,  on applique ces nouveaux paramètres à l'équipe correspondante
            if (teams[index] != teams[indexOfSelectedTeam2])
            {
                UpdateUIS(1, teams[index]);
                break;
            }

        }
    }










    public void UpdateSelectionTeam2(int one)
    {
        //'one' est une valeur donnée par les boutons de l'UI. En fonction de son signe, on incrémentera ou décrémentera l'index parcourant le tableau
        indexOfSelectedTeam2 += one;






        //Pour s'assurer que l'index ne sorte jamais des limites du tableau
        if (indexOfSelectedTeam2 == teams.Count)
        {
            indexOfSelectedTeam2 = 0;
        }
        else if (indexOfSelectedTeam2 == -1)
        {
            indexOfSelectedTeam2 = teams.Count - 1;
        }

        //Si on veut que les deux joueurs puissent sélectionner la même équipe :

        //UpdateUIS(2, teams[indexOfSelectedTeam2]);





        //Si on veut que les deux joueurs ne puissent pas sélectionner la même équipe :


        for (int i = 0; i < teams.Count; i++)
        {

            //On parcourt le tableau jusqu'à trouver une case inutilisée
            int index = indexOfSelectedTeam2 + i * one;




            //Pour s'assurer que l'index ne sorte jamais des limites du tableau
            if (index == teams.Count)
            {
                index = 0;
                i = 0;
            }
            else if (index <= -1)
            {
                index = teams.Count - 1;
                i = 0;
            }





            //Si on a trouvé une équipe qui n'est pas déjà choisie par un des joueurs,  on applique ces nouveaux paramètres à l'équipe correspondante
            if (teams[index] != teams[indexOfSelectedTeam1])
            {
                UpdateUIS(2, teams[index]);
                break;
            }

        }

    }


}


[Serializable]
public class Team
{
    //La classe utilisée pour stocker les informations relatives au paramètres visuels des équipes une fois en jeu (en preview et lors d'un match)
    public string tag;
    public string teamName;
    public Sprite flagToDisplay;
    public int  indexOfMeshToDisplay;

}