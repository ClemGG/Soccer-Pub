using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallPhysics : MonoBehaviour {

    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Transform t;
    [HideInInspector] public Vector3 startPos;
    [HideInInspector] public AudioSource aud;

    public bool isHeldByPlayer;
    public int lastPlayerID = -1;

    public static BallPhysics instance;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Erreur : Il y a plus d'un script BallPhysics dans la scène.");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        aud = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        t = transform;
        startPos = t.position;
    }








    public void AttachToPlayer(PlayerSystem newPlayer)
    {
        if (newPlayer.isStunned)
            return;


        //Si on veut que l'IA ne vole pas le contrôle au joueur

        //if (lastPlayerID != -1)
        //{

        //    if (newPlayer.ID % 2 == lastPlayerID % 2    //Si l'IA et le joueur sont de la même équipe
        //        &&  newPlayer.ID != lastPlayerID            // Si le joueur ne ramasse pas sa propre balle
        //        && !newPlayer.isControlledByPlayer         // Si l'IA n'est pas contrôlée par un autre joueur
        //        && PlayerManager.instance.allPlayers[lastPlayerID].isControlledByPlayer)   // Et si le joueur contrôle toujours le lanceur de la balle
        //    {

        //        //Alors on échange leur contrôle
        //        newPlayer.isControlledByPlayer = true;
        //        newPlayer.joystickNumber = PlayerManager.instance.allPlayers[lastPlayerID].joystickNumber;
        //        PlayerManager.instance.allPlayers[lastPlayerID].isControlledByPlayer = false;
        //        PlayerManager.instance.allPlayers[lastPlayerID].joystickNumber = 0;

        //        newPlayer.UpdatePlayerNumberUI();
        //        PlayerManager.instance.allPlayers[lastPlayerID].UpdatePlayerNumberUI();

        //        PlayerManager.instance.UpdateCameraTargets();
        //    }
        //}




        //Si on veut que l'IA vole le contrôle au joueur
        

        if (newPlayer.ID != lastPlayerID            // Si le joueur ne ramasse pas sa propre balle
            && !newPlayer.isControlledByPlayer)     // Si l'IA n'est pas contrôlée par un autre joueur
               
        {

            //On cherche quel joueur est le plus proche de l'IA
            float smallestDst = Mathf.Infinity;
            int index = -1;

            for (int i = 0; i < newPlayer.teamOfThisPlayer.Count; i++)
            {

                float f = Vector3.Distance(t.position, newPlayer.teamOfThisPlayer[i].t.position);

                if (f < smallestDst && newPlayer.teamOfThisPlayer[i].ID != newPlayer.ID && newPlayer.teamOfThisPlayer[i].isControlledByPlayer)
                {
                    smallestDst = f;
                    index = i;
                }



            }
            if (index != -1)
            {
                //Puis on échange leur contrôle
                newPlayer.isControlledByPlayer = true;
                newPlayer.joystickNumber = newPlayer.teamOfThisPlayer[index].joystickNumber;
                newPlayer.teamOfThisPlayer[index].isControlledByPlayer = false;
                newPlayer.teamOfThisPlayer[index].joystickNumber = 0;

                newPlayer.UpdatePlayerNumberUI();
                newPlayer.teamOfThisPlayer[index].UpdatePlayerNumberUI();

                PlayerManager.instance.UpdateCameraTargets();
            }
        }


        isHeldByPlayer = true;
        lastPlayerID = newPlayer.ID;
        newPlayer.hasBall = true;

        rb.constraints = RigidbodyConstraints.FreezeAll;
        t.position = newPlayer.shotPoint.position;
        t.rotation = newPlayer.shotPoint.rotation;
        t.parent = newPlayer.shotPoint;

    }

    public void DetachFromPlayer(PlayerSystem player, Vector3 knockBbackForce)
    {
        t.parent = null;
        rb.constraints = RigidbodyConstraints.None;
        rb.AddRelativeForce(knockBbackForce, ForceMode.Impulse);
        rb.AddRelativeTorque(knockBbackForce, ForceMode.Impulse);
        player.hasBall = false;
        isHeldByPlayer = false;


    }




    private void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            aud.Play();
        }
    }

}
