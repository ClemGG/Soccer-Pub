using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {



    [SerializeField] private GameObject goalParticle;
    [SerializeField] private GameObject goalAudio;
    

    public enum TypeOfGoal { UpperGoal, LowerGoal };
    public TypeOfGoal typeOfGoal;

    [Range(1, 2)] public int teamNumber = 1;
    public int scorePoints;


    private void OnTriggerEnter(Collider col)
    {
        //Si la balle entre dans les cages, on lance les FX et on ajoute le spoints au score
        if (col.gameObject.tag == "Entity/Ball" && /*!BallPhysics.instance.isHeldByPlayer &&*/ !ScoreManager.instance.isGameEnded)
        {
            ScoreManager.instance.AddToScore(scorePoints, teamNumber);
            goalParticle.SetActive(true);
            goalAudio.SetActive(false);
            goalAudio.SetActive(true);
        }

    }

}
