using Clement.Utilities.Maths;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA_Attack : PlayerSystem {



    [Space(10)]
    [Header("IA Attack : ")]
    [Space(10)]

    [SerializeField] private Transform goalToReach;

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


    int rand;


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
            if (!BallPhysics.instance.isHeldByPlayer || (BallPhysics.instance.lastPlayerID % 2 != ID % 2 && BallPhysics.instance.isHeldByPlayer))
            {

                rand = 0;
                MoveTowardsBall();

                if (Vector3.Distance(t.position, BallPhysics.instance.t.position) < zoneDashRadius)
                {
                    DashInput();
                }

            }
            //Si la balle est tenue par un allié, alors l'attaquant peut aller jusqu'aux buts
            else if(BallPhysics.instance.lastPlayerID % 2 == ID % 2 && BallPhysics.instance.isHeldByPlayer)
            {
                if (rand == 0f)
                {
                    rand = Random.Range(2, 6);
                }

                MoveTowardsGoal();
            }


            
        }





        // S'il attrape la balle, il cherchera parmi les joueurs de son équipe le plus proche et lui enverra la balle
        if (hasBall)
        {
            if (rand == 0f)
            {
                rand = Random.Range(2, 6);
            }
            
            MoveTowardsGoal();
        }
        else
        {
            aInput = 0f;
            bInput = 0f;
            yInput = 0f;
            
        }
    }





    private void MoveTowardsGoal()
    {
        Vector3 dir = (goalToReach.GetChild(rand).transform.position - t.position);

        //Tant qu'on n'a pas atteint un des points de tir, on s'en rapproche
        if (dir.magnitude > 1f)
        {
            horizontalInput = dir.normalized.x;
            verticalInput = dir.normalized.z;
        }
        else
        {
            horizontalInput = 0f;
            verticalInput = 0f;
            
            //On récupère la direction vers le but
            Vector3 goalDir = (goalToReach.position - t.position);


            //On oriente le joueur dans la direction du but
            Quaternion targetRotation = Quaternion.FromToRotation(meshToRotate.forward, goalDir) * meshToRotate.rotation;
            meshToRotate.rotation = Quaternion.Slerp(meshToRotate.rotation, targetRotation, rotationSpeed * Time.deltaTime);



            //On vérifie que l'IA est bien orientée vers son allié, et si c'st le cas, il lance la balle
            if (Quaternion.Angle(meshToRotate.rotation, targetRotation) < 5f)
            {
                if (goalDir.magnitude <= maxDstPasse)
                {
                    aInput = 1f;
                }
                else if (goalDir.magnitude > maxDstPasse && goalDir.magnitude <= maxDstTir)
                {
                    bInput = 1f;
                }
                else if (goalDir.magnitude > maxDstTir)
                {
                    yInput = 1f;
                }
            }
        }
    }

    private void MoveTowardsBall()
    {
        Vector3 dir = (BallPhysics.instance.t.position - t.position).normalized;
        horizontalInput = dir.x;
        verticalInput = dir.z;


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
