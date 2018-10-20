
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA_Goal : PlayerSystem {


    [Space(10)]
    [Header("IA Goal : ")]
    [Space(10)]

    [SerializeField] private Color zoneDefenseColor;
    [SerializeField] private Vector3 zoneDefenseSize;
    [SerializeField] private float zoneDefenseRadius;

    [SerializeField] private Color zoneDashColor;
    [SerializeField] private float zoneDashRadius;

    [SerializeField] private Color passeColor;
    [SerializeField] private Color tirColor;
    [SerializeField] private float maxDstPasse;
    [SerializeField] private float maxDstTir;

    [Space(10)]
    [Header("Gizmos : ")]
    [Space(10)]

    [SerializeField] private bool showZoneDefense;
    [SerializeField] private bool showZoneDash;
    [SerializeField] private bool showDstLimits;

    protected override void GetInput()
    {
        //Si cette IA est contrôlée par le joueur, alors on n'applique que le GetInput() de base
        if (isControlledByPlayer)
        {
            base.GetInput();
            return;
        }


        if (!hasBall)
        {
            //Si la balle est dans la zone de recherche de l'IA et qu'elle n'est pas transportée, OU qu'elle est transportée par un ennemi, alors on dirige l'IA vers la balle. 
            //Sinon, elle revient à sa startPos, qui sera son point de défense par défaut.
            if (Vector3.Distance(startPos, BallPhysics.instance.t.position) < zoneDefenseRadius &&
                (!BallPhysics.instance.isHeldByPlayer || (BallPhysics.instance.lastPlayerID % 2 != ID % 2 && BallPhysics.instance.isHeldByPlayer)))
            {
                MoveTowardsBall();


                if(Vector3.Distance(t.position, BallPhysics.instance.t.position) < zoneDashRadius)
                {
                    DashInput();
                }

            }
            else
            {
                GoBackToDefense();
            }

            
        }





        // S'il attrape la balle, il cherchera parmi les joueurs de son équipe le plus proche et lui enverra la balle
        if (hasBall)
        {
            horizontalInput = 0f;
            verticalInput = 0f;

            GoBackToDefense();

        }
        else
        {
            aInput = 0f;
            bInput = 0f;
            yInput = 0f;
        }
    }












    private void MoveTowardsBall()
    {
        Vector3 dir = (BallPhysics.instance.t.position - t.position).normalized;
        horizontalInput = dir.x;
        verticalInput = dir.z;
        

    }
    private void GoBackToDefense()
    {
        Vector3 dir = Vector3.zero;

        if (Vector3.Distance(startPos, t.position) > .1f)
        {
            dir = (startPos - t.position).normalized;
            horizontalInput = dir.x;
            verticalInput = dir.z;
        }
        else
        {
            dir = Vector3.zero;
            horizontalInput = 0f;
            verticalInput = 0f;


            if (hasBall)
            {
                SendBallToNearestAlly();
            }
            else
            {
                //On réoriente le joueur dans sa direction d'origine
                meshToRotate.rotation = Quaternion.Slerp(meshToRotate.rotation, startRot, rotationSpeed * Time.deltaTime);
            }

        }
        

    }





    private void DashInput()
    {

        //Tant que l'IA n'a pas confirmé le dash, on active la touche de dash et on réoriente les inputs de direction dans la direction de la cible. 
        //Cette condition permet à l'IA de se synchroniser avec la FixedUpdate.
        if (!isDashing)
        {
            xInput = 1f;
        }
        else
        {
            xInput = 0f;
        }
    }





    private void SendBallToNearestAlly()
    {
        float smallestDst = Mathf.Infinity;
        Transform targetedPlayer = null;

        //On parcourt la liste d'alliés jusqu'à trouver lequel est le plus proche du joueur
        for (int i = 0; i < teamOfThisPlayer.Count; i++)
        {
            if (teamOfThisPlayer[i].ID == ID || teamOfThisPlayer[i].isStunned)
                continue;

            float f = Vector3.Distance(t.position, teamOfThisPlayer[i].t.position);
            if (f < smallestDst)
            {
                smallestDst = f;
                targetedPlayer = teamOfThisPlayer[i].t;
            }
        }

        if (targetedPlayer == null)
        {
            return;
        }


        //On récupère la direction de son allié le plus proche
        Vector3 dir = (targetedPlayer.position - t.position);

        //On oriente le joueur en fonction de la direction dans laquelle se trouve l'allié
        Quaternion targetRotation = Quaternion.FromToRotation(meshToRotate.forward, dir) * meshToRotate.rotation;
        meshToRotate.rotation = Quaternion.Slerp(meshToRotate.rotation, targetRotation, rotationSpeed * Time.deltaTime);


        //On vérifie que l'IA est bien orientée vers son allié, et si c'st le cas, il lance la balle
        if (Quaternion.Angle(meshToRotate.rotation, targetRotation) < 5f)
        {
            if(dir.magnitude <= maxDstPasse)
            {
                aInput = 1f;
            }
            else if(dir.magnitude > maxDstPasse && dir.magnitude <= maxDstTir)
            {
                bInput = 1f;
            }
            else if(dir.magnitude > maxDstTir)
            {
                yInput = 1f;
            }

        }

    }







    private void OnDrawGizmosSelected()
    {

        if (showZoneDefense)
        {
            if (startPos != Vector3.zero)
            {
                Gizmos.color = zoneDefenseColor;
                Gizmos.DrawWireSphere(startPos, zoneDefenseRadius);

                Gizmos.color = Color.red;
                Gizmos.DrawSphere(startPos, .1f);

            }
            else
            {
                Gizmos.color = zoneDefenseColor;
                Gizmos.DrawWireSphere(t.position, zoneDefenseRadius);
            }
        }

        if (showZoneDash)
        {
            Gizmos.color = zoneDashColor;
            Gizmos.DrawWireSphere(t.position, zoneDashRadius);
        }


        if (showDstLimits)
        {
            Gizmos.color = passeColor;
            Gizmos.DrawSphere(t.position, maxDstPasse);
            Gizmos.color = tirColor;
            Gizmos.DrawSphere(t.position, maxDstTir);
        }
        
    }

}
