using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {




    [Space(10)]
    [Header("FX : ")]
    [Space(10)]

    [SerializeField] private GameObject whistleAudio;
    [SerializeField] private GameObject countdownAudio;
    [SerializeField] private GameObject bgmAudio;





    [Space(10)]
    [Header("Timer Before Start : ")]
    [Space(10)]

    [SerializeField] private float timeBeforeStart = 3f;
    [SerializeField] private float BeforeStartPopupDelay = .3f;
    private float beforeStartTimer;
    [HideInInspector] public bool matchHasStarted = false;
    [SerializeField] private TextMeshProUGUI beforeStartText;






    [Space(10)]
    [Header("Countdown : ")]
    [Space(10)]

    [SerializeField] private float matchDuration = 90f;
    private float timer;
    [SerializeField] private TextMeshProUGUI MatchCountdownText;
    [SerializeField] private TextMeshProUGUI timesupText;




    [Space(10)]
    [Header("Scores : ")]
    [Space(10)]

    [SerializeField] private int scoreP1 = 0;
    [SerializeField] private int scoreP2 = 0;





    [Space(10)]
    [Header("UIs : ")]
    [Space(10)]

    [SerializeField] private Text scoreTextP1;
    [SerializeField] private Text scoreTextP2;
    [SerializeField] private TextMeshProUGUI goalText;
    [SerializeField] private TextMeshProUGUI winText;

    [SerializeField] private float goalPopupDelay;
    [SerializeField] private float timesUpPopupDelay;





    [Space(10)]
    [Header("UIs : ")]
    [Space(10)]

    public bool isGameEnded;

    public static ScoreManager instance;








    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Erreur : Il y a plus d'un script ScoreManager dans la scène.");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }


    private void Start()
    {
        UpdateScoreUI(0);
        beforeStartTimer = timeBeforeStart;
        countdownAudio.SetActive(true);
    }

    private void FixedUpdate()
    {
        if(!matchHasStarted)
        {
            CountdownBeforeMatch();
        }

        if (beforeStartTimer <= 0f && matchHasStarted)
        {
            MatchCountdown();
        }
    }





    private void CountdownBeforeMatch()
    {
        //Tant que le compte à rebours ne s'est pas terminé, on continue de faire descendre le timer
        if (beforeStartTimer > 0f)
        {
            beforeStartTimer -= Time.deltaTime;
            PlayerManager.instance.StopAllPlayersMovement();

            ConvertCountdownBeforeStart(beforeStartTimer);


            //Une fois le timer à 0, on rend le contrôle aux joueurs et on lance le compte à rebours du match
            if (beforeStartTimer < 0f)
            {
                beforeStartTimer = 0f;

                PlayerManager.instance.UnfreezePlayersMovement();

                timer = matchDuration;
                StartCoroutine(DisplayStartMatchText());
                matchHasStarted = true;


                whistleAudio.SetActive(true);
                bgmAudio.SetActive(true);
            }
        }


        
    }






    private void MatchCountdown()
    {
        //Tant que le timer n'a pas atteint 0, le match continue
        if(timer > 0f)
        {
            timer -= Time.deltaTime;
            if(timer < 0f)
            {
                timer = 0f;
            }

            ConvertTime(timer);

        }
        //Sinon, on arrête le match
        else
        {
            if(!isGameEnded)
                EndGame();
        }
    }





    private void ConvertTime(float timer)
    {
        //Utilisé pour convertir le temps en minutes et secondes avant de l'afficher sur l'UI
        int min = (int)Mathf.Floor(timer / 60);
        int sec = (int)(timer % 60);

        if(sec == 60)
        {
            sec = 0;
            min++;
        }

        string minutes = min.ToString("0");
        string seconds = sec.ToString("00");

        MatchCountdownText.text = string.Format("{0}:{1}", minutes, seconds);
    }
    

    private void ConvertCountdownBeforeStart(float timer)
    {
        //Utilisé pour convertir le temps en secondes avant de l'afficher sur l'UI
        int sec = (int)(timer % 60);

        string seconds = sec.ToString("0");

        beforeStartText.text = string.Format("{0}", seconds);
    }







    public void AddToScore(int points, int teamNumber)
    {
        //Fonction appelé par les goals lorsque la balle entre dans leurs colliders
        if(teamNumber == 1)
        {
            scoreP1 += points;
            UpdateScoreUI(teamNumber);
        }
        else
        {
            scoreP2 += points;
            UpdateScoreUI(teamNumber);
        }
    }














    private void UpdateScoreUI(int teamNumber)
    {
        //Affiche le nouveau score à l'écran
        scoreTextP1.text = scoreP1.ToString();
        scoreTextP2.text = scoreP2.ToString();


        if(teamNumber == 0)
        {
            winText.enabled = false;
            goalText.enabled = false;
            timesupText.enabled = false;

        }
        else
        {
            StopCoroutine(DisplayGoalText());
            StartCoroutine(DisplayGoalText());
        }
    }







    private IEnumerator DisplayGoalText()
    {
        PlayerManager.instance.StopAllPlayersMovement();
        goalText.enabled = true;
        yield return new WaitForSeconds(goalPopupDelay);
        goalText.enabled = false;
        PlayerManager.instance.RespawnPlayers();
    }













    private IEnumerator DisplayTimesUpText()
    {
        //Lorsque le match est terminé, on affiche le popup "Time's up!" à l'écran et on coupe le système de mouvement des joueurs

        PlayerManager.instance.StopAllPlayersMovement();

        timesupText.enabled = true;
        yield return new WaitForSeconds(timesUpPopupDelay);
        timesupText.enabled = false;

        //Une fois le popup terminé, on récupère le nom de l'équipe gagnante et on l'affiche à l'écran

        yield return new WaitForSeconds(timesUpPopupDelay);
        winText.enabled = true;
        winText.text = GetWinningTeamNumber();

        yield return new WaitForSeconds(timesUpPopupDelay);
        winText.enabled = false;

        //Puis, on retourne au menu ppal

        MainMenuButtons.instance.ReturnToMainMenu();

    }


    private IEnumerator DisplayStartMatchText()
    {
        beforeStartText.text = "Match";
        yield return new WaitForSeconds(BeforeStartPopupDelay);
        beforeStartText.enabled = false;
    }




    private string GetWinningTeamNumber()
    {
        //En fonction du score le plus élevé, on récupère le nom de l'équipe gagnante. Si les deux scores sont égaux, on renvoie "Egalité".

        string teamName = "0";

        if(scoreP1 == scoreP2)
        {
            return "Tie!";
        }
        else if(scoreP1 > scoreP2)
        {
            //i = "1";
            teamName = PlayerManager.instance.setupTeams[0].teamName;
        }
        else if(scoreP1 < scoreP2)
        {
            teamName = "2";
            teamName = PlayerManager.instance.setupTeams[1].teamName;
        }

        //return "L'équipe " + i + " a gagné!";
        return teamName + " won the match !";

    }

    private void EndGame()
    {
        isGameEnded = true;
        whistleAudio.SetActive(true);

        StopCoroutine(DisplayTimesUpText());
        StartCoroutine(DisplayTimesUpText());
    }
}
