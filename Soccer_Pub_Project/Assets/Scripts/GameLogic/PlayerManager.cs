using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour {

    public static PlayerManager instance;


    [Space(10)]
    [Header("FX : ")]
    [Space(10)]

    [SerializeField] private GameObject specialMoveActivationAudio;




    [Space(10)]
    [Header("Teams settings : ")]
    [Space(10)]

    [SerializeField] private Transform playersTransform;
    [HideInInspector] public List<PlayerSystem> allPlayers;
    public List<PlayerSystem> playersOfTeam1;
    public List<PlayerSystem> playersOfTeam2;
    




    [Space(10)]
    [Header("Teams setup : ")]
    [Space(10)]


    public List<Team> setupTeams;
    public List<Image> flagImages;
    public List<int> connectedPlayers;







    [Space(10)]
    [Header("Special hits : ")]
    [Space(10)]

    [SerializeField] private float numberOfHits;
    [Range(0f, 1f)] [SerializeField] private float jaugeFillSpeed;

    [SerializeField] private Slider team1Slider;
    [SerializeField] private Slider team2Slider;
    private float team1Meter = 0f;
    private float team2Meter = 0f;

    private bool updateSlider1;
    private bool updateSlider2;
    public bool isJauge1Full;
    public bool isJauge2Full;




    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Erreur : Il y a plus d'un script PlayerManager dans la scène.");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        
        //On désacive les icônes des sliders. Ils ne s'activeront qu'une fois les jauges des coups spéciaux remplies
        team1Slider.transform.GetChild(2).gameObject.SetActive(false);
        team2Slider.transform.GetChild(2).gameObject.SetActive(false);


    }

    private void Start()
    {
        
        allPlayers = new List<PlayerSystem>();

        //On récupère tous les joueurs stockées dans la Transform "playersTransform".
        //Pour que ce script fonctionne, tous les joueurs doivent être attachés à la transform de sorte à ce que les joueurs de l'équipe 1 aient un index pair et ceux de l'équipe 2 aient un index impair
        for (int i = 0; i < playersTransform.childCount; i++)
        {
            allPlayers.Add(playersTransform.GetChild(i).GetComponent<PlayerSystem>());
            allPlayers[i].ID = i;
        }

        //L'ID de chaque joueur sera égal à son index dans la Transform.
        //Si l'ID est pair, il sera dans l'équipe 1. Sinon, il sera dans l'équipe 2.
        for (int i = 0; i < allPlayers.Count; i++)
        {
            if(allPlayers[i].ID % 2 == 0)
            {
                allPlayers[i].meshToRotate.GetChild(setupTeams[0].indexOfMeshToDisplay).gameObject.SetActive(true);
                playersOfTeam1.Add(allPlayers[i]);
            }
            else
            {
                allPlayers[i].meshToRotate.GetChild(setupTeams[1].indexOfMeshToDisplay).gameObject.SetActive(true);
                playersOfTeam2.Add(allPlayers[i]);
            }
        }

        //Ici, on inscrit l'IDinTeam de chaque joueur. Cet ID est utilisé pour le changement de joueur
        for (int i = 0; i < allPlayers.Count; i++)
        {
            if(allPlayers[i].ID % 2 == 0)
            {
                allPlayers[i].teamOfThisPlayer = playersOfTeam1;
                allPlayers[i].IDinTeam = playersOfTeam1.IndexOf(allPlayers[i]);
            }
            else
            {
                allPlayers[i].teamOfThisPlayer = playersOfTeam2;
                allPlayers[i].IDinTeam = playersOfTeam2.IndexOf(allPlayers[i]);
            }
        }

        //On initialise la position des joueurs, la connexion des manettes aux IAs, les cibles de la caméra et les drapeaux attachés à chaque score
        RespawnPlayers();
        SetUpPlayerControllers();
        UpdateCameraTargets();
        SetupFlagUIS();


    }






    private void SetupFlagUIS()
    {
        if (flagImages != null)
        {
            flagImages[0].sprite = setupTeams[0].flagToDisplay;
            flagImages[1].sprite = setupTeams[1].flagToDisplay;
        }
    }

    private void SetUpPlayerControllers()
    {


        for (int i = 0; i < allPlayers.Count; i++)
        {
            if(i < connectedPlayers.Count)
            {
                allPlayers[i].joystickNumber = connectedPlayers[i];
                allPlayers[i].UpdatePlayerNumberUI();
            }

            allPlayers[i].isControlledByPlayer = allPlayers[i].joystickNumber > 0;
        }
    }




    private void Update()
    {
        UpdateSliders();
    }









    private void UpdateSliders()
    {
        if (updateSlider1)
        {
            team1Slider.value = Mathf.Lerp(team1Slider.value, team1Meter, jaugeFillSpeed);

            if(Mathf.Approximately(team1Slider.value, team1Meter)){
                updateSlider1 = false;
            }
        }
        if (updateSlider2)
        {
            team2Slider.value = Mathf.Lerp(team2Slider.value, team2Meter, jaugeFillSpeed);


            if (Mathf.Approximately(team2Slider.value, team2Meter)){
                updateSlider2 = false;
            }
        }
        
    }





    public void UpdateCameraTargets()
    {
        MultipleTargetCam cam = Camera.main.GetComponent<MultipleTargetCam>();
        cam.targets.Clear();

        for (int i = 0; i < allPlayers.Count; i++)
        {
            if (allPlayers[i].isControlledByPlayer)
            {
                cam.targets.Add(allPlayers[i].t);
            }
        }
        cam.targets.Add(BallPhysics.instance.t);

    }




    public void StopAllPlayersMovement()
    {
        for (int i = 0; i < allPlayers.Count; i++)
        {
            allPlayers[i].canMove = false;
        }
    }


    public void UnfreezePlayersMovement()
    {
        for (int i = 0; i < allPlayers.Count; i++)
        {
            allPlayers[i].canMove = true;
        }
    }




    public void RespawnPlayers()
    {
        for (int i = 0; i < playersOfTeam1.Count; i++)
        {
            playersOfTeam1[i].t.position = playersOfTeam1[i].startPos;
            Quaternion targetRotation = Quaternion.FromToRotation(playersOfTeam1[i].meshToRotate.forward, Vector3.right);
            playersOfTeam1[i].meshToRotate.rotation = targetRotation;

            if(playersOfTeam1[i].startRot != targetRotation)
            {
                playersOfTeam1[i].startRot = targetRotation;
            }

        }

        for (int i = 0; i < playersOfTeam2.Count; i++)
        {
            playersOfTeam2[i].t.position = playersOfTeam2[i].startPos;
            Quaternion targetRotation = Quaternion.FromToRotation(playersOfTeam2[i].meshToRotate.forward, Vector3.left);
            playersOfTeam2[i].meshToRotate.rotation = targetRotation;

            if (playersOfTeam2[i].startRot != targetRotation)
            {
                playersOfTeam2[i].startRot = targetRotation;
            }
        }

        BallPhysics.instance.isHeldByPlayer = false;
        BallPhysics.instance.t.parent = null;
        BallPhysics.instance.t.position = BallPhysics.instance.startPos;
        BallPhysics.instance.t.rotation = Quaternion.identity;
        BallPhysics.instance.lastPlayerID = -1;
        BallPhysics.instance.rb.velocity = Vector3.zero;

        for (int i = 0; i < allPlayers.Count; i++)
        {
            allPlayers[i].ResetInputs();
            allPlayers[i].hasBall = false;
            allPlayers[i].canMove = true;
            allPlayers[i].isStunned = false;
            allPlayers[i].DisableDashFX();
            allPlayers[i].UpdateStunFX(false);
        }
    }


    public void IncreaseJauge(int teamNumber)
    {
        //On utilise l'ID en paramètre pour déterminer quelle équipe a appelé la fonction
        //On augmente ensuite la jauge de chargement de l'équipe associée, et une fois pleine, on active l'icon indiquant que le coup spécial est disponible
        switch (teamNumber)
        {
            case 0:
                team1Meter += 1f / numberOfHits;
                if (team1Meter >= 1f)
                {
                    team1Meter = 1f;
                    isJauge1Full = true;


                    team1Slider.transform.GetChild(2).gameObject.SetActive(true);



                    specialMoveActivationAudio.SetActive(false);
                    specialMoveActivationAudio.SetActive(true);
                }
                updateSlider1 = true;
                break;

            case 1:
                team2Meter += 1f / numberOfHits;
                if (team2Meter >= 1f)
                {
                    team2Meter = 1f;
                    isJauge2Full = true;
                    team2Slider.transform.GetChild(2).gameObject.SetActive(true);



                    specialMoveActivationAudio.SetActive(false);
                    specialMoveActivationAudio.SetActive(true);
                }
                updateSlider2 = true;
                break;

            default:
                break;
        }
    }

    public void DecreaseJauge(int teamNumber)
    {
        //Cette fonction est appelée une fois que le coup spécial a été activé
        //On utilise l'ID en paramètre pour déterminer quelle équipe a appelé la fonction, et on réduit la jauge correspndante à 0
        switch (teamNumber)
        {
            case 0:
                team1Meter = 0f;
                isJauge1Full = false;
                updateSlider1 = true;


                team1Slider.transform.GetChild(2).gameObject.SetActive(false);
                break;

            case 1:
                team2Meter = 0f;
                isJauge2Full = false;
                updateSlider2 = true;

                
                team2Slider.transform.GetChild(2).gameObject.SetActive(false);
                break;

            default:
                break;
        }
    }


    public bool GetTeamJaugeFull(int teamNumber)
    {
        //Fonction utilisée pour déterminer si le coup spécial peut être activé
        switch (teamNumber)
        {
            case 0:
                return isJauge1Full;

            case 1:
                team2Meter = 0f;
                return isJauge2Full;

            default:
                return false;
        }
    }

}
