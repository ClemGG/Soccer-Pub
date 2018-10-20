using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSystem : MonoBehaviour {

    [Space(10)]
    [Header("Analog input values :")]
    [Space(10)]

    [SerializeField] protected float rotationSpeed;
    [SerializeField] protected float moveSpeed;

    [SerializeField] protected float aInput;
    [SerializeField] protected float bInput;
    [SerializeField] protected float xInput;
    [SerializeField] protected float yInput;

    protected int bumperInput;
    protected int pauseInput;
    protected int lastPauseInput;
    private float pauseTimer;


    protected float horizontalInput;
    protected float verticalInput;

    [Space(10)]

    public Transform t;
    public Transform meshToRotate;
    protected Rigidbody rb;
    protected Vector3 moveDir;
    [HideInInspector] public Vector3 startPos;
    [HideInInspector] public Quaternion startRot;

    [HideInInspector] public MeshRenderer rend;
    [HideInInspector] public MeshFilter filter;


    [Space(10)]
    [Header("FX :")]
    [Space(10)]

    [SerializeField] protected GameObject shootAudio;
    [SerializeField] protected GameObject hitAudio;
    [SerializeField] protected GameObject dashAudio;
    [SerializeField] protected GameObject specialMoveAudio;
    [SerializeField] protected GameObject superDashParticles;
    [SerializeField] protected GameObject dashParticles;

    protected GameObject stunParticles;
    protected Text playerNumberText;




    [Space(10)]
    [Header("Ball Physics : ")]
    [Space(10)]

    public bool hasBall = false;
    public Transform shotPoint;

    [SerializeField] protected float forceRecul;
    [SerializeField] protected Vector3 forcePasse;
    [SerializeField] protected Vector3 forceTir;
    [SerializeField] protected Vector3 forceLobbe;






    [Space(10)]
    [Header("IA : ")]
    [Space(10)]

    public int ID;
    public int IDinTeam;
    public bool isControlledByPlayer;
    [SerializeField] protected LayerMask whatIsPlayer;

    [Space(10)]

    public bool canMove = true;
    public bool isStunned = false;
    [SerializeField] protected bool isDashing = false;
    public bool isInvincible = false;
    [SerializeField] protected float stunDuration;
    [SerializeField] protected float invincibilityDuration;
    [SerializeField] protected float dashSpeed;
    [SerializeField] protected float dashDuration;
    [SerializeField] protected float delayBetweenDashes;
    protected float stunTimer;
    protected float invincibilityTimer;
    protected float dashTimer;
    protected float dashDelayTimer;


    protected RaycastHit ligneDeTir;
    protected Transform targetAlly;



    [Space(10)]
    [Header("Special Move : ")]
    [Space(10)]
    
    [SerializeField] protected bool useSpecialMove = false;
    public bool isUsingSpecialMove = false;
    [SerializeField] protected float specialMoveSpeed;
    [SerializeField] protected float specialMoveDuration;
    protected float specialMoveTimer;



    [Space(10)]
    [Header("Multiplayer : ")]
    [Space(10)]

    public int joystickNumber;
    public List<PlayerSystem> teamOfThisPlayer;

    [SerializeField] private float swapDelay;
    private float swapTimer;



    
    void Awake () {

        //isControlledByPlayer = joystickNumber > 0;

        dashTimer = dashDuration;
        dashDelayTimer = delayBetweenDashes;

        t = transform;
        meshToRotate = t.GetChild(0);
        startPos = t.position;
        rb = GetComponent<Rigidbody>();
        
        rend = meshToRotate.GetComponent<MeshRenderer>();
        filter = meshToRotate.GetComponent<MeshFilter>();

        stunParticles = t.GetChild(2).gameObject;
        playerNumberText = t.GetChild(3).GetChild(0).GetComponent<Text>();
        UpdatePlayerNumberUI();


    }


    
    void Update () {

        GetInput();
        
        if (isControlledByPlayer && ScoreManager.instance.matchHasStarted)
        {
            SwapPlayers();
            PauseGame();
        }

    }


    void FixedUpdate()
    {
        ControllerMovement();
    }








    private void ControllerMovement()
    {
        



        if (!canMove)
            return;

        if (!isUsingSpecialMove)
        {
            if (isStunned)
                StunPlayer();

            if (isInvincible)
                MakePlayerInvincible();

            if (!isDashing && !isStunned)
            {
                Move();
                Shoot();
            }


            if (!isStunned)
                Dash();
        }
        else
        {
            if (!isStunned && !isDashing)
            {
                SpecialMove();
            }
        }

        

        

    }






    protected virtual void GetInput()
    {
        if (joystickNumber == 0)
        {
            horizontalInput =  verticalInput = aInput = bInput = xInput = yInput = bumperInput = 0;
            return;
        }

        horizontalInput = Input.GetAxis("LeftJoystickX_P" + joystickNumber);
        verticalInput = Input.GetAxis("LeftJoystickY_P" + joystickNumber);

        aInput = Input.GetAxis("A" + joystickNumber);
        bInput = Input.GetAxis("B" + joystickNumber);
        xInput = Input.GetAxis("X" + joystickNumber);
        yInput = Input.GetAxis("Y" + joystickNumber);

        bumperInput = (int)Input.GetAxisRaw("Bumpers_P" + joystickNumber);

        pauseInput = (int)Input.GetAxisRaw("XBox_P" + joystickNumber);

        if (bumperInput > 0f && !isUsingSpecialMove && PlayerManager.instance.GetTeamJaugeFull(ID % 2) && !isStunned)
        {
            specialMoveTimer = 0f;
            isUsingSpecialMove = true;
            useSpecialMove = true;
        }
    }

    private void SwapPlayers()
    {


        if (swapTimer < swapDelay)
        {
            swapTimer += Time.deltaTime;
        }
        else
        {
            if (bumperInput >= 0f)
            {
                return;
            }


            swapTimer = 0f;


            //Si l'on veut que le changement de joueur se fasse en fonction de l'ordre dans la liste de joueurs

            //int y = -bumperInput;
            //int nextPlayerInTeam = IDinTeam + y;



            //for (int i = 0; i < teamOfThisPlayer.Count; i++)
            //{
            //    int index = nextPlayerInTeam + i;



            //    if (index == teamOfThisPlayer.Count)
            //    {
            //        index = 0;
            //        i = 0;
            //    }
            //    else if (index == -1)
            //    {
            //        index = teamOfThisPlayer.Count - 1;
            //        i = 0;
            //    }


            //    if (teamOfThisPlayer[index].joystickNumber == 0)
            //    {
            //        teamOfThisPlayer[index].isControlledByPlayer = true;
            //        teamOfThisPlayer[index].joystickNumber = joystickNumber;
            //        isControlledByPlayer = false;
            //        joystickNumber = 0;


            //        UpdatePlayerNumberUI();
            //        teamOfThisPlayer[index].UpdatePlayerNumberUI();
            //        PlayerManager.instance.UpdateCameraTargets();

            //        break;
            //    }

            //}





            //Si l'on veut que le changement de joueur se fasse en fonction de la distance des joueurs par rapport à la balle


            float smallestDst = Mathf.Infinity;
            int index = 0;

            for (int i = 0; i < teamOfThisPlayer.Count; i++)
            {

                float f = Vector3.Distance(BallPhysics.instance.t.position, teamOfThisPlayer[i].t.position);

                if (f < smallestDst && teamOfThisPlayer[i].ID != ID && !teamOfThisPlayer[i].isStunned && !teamOfThisPlayer[i].isControlledByPlayer)
                {
                    smallestDst = f;
                    index = i;
                }
                
            }


            teamOfThisPlayer[index].isControlledByPlayer = true;
            teamOfThisPlayer[index].joystickNumber = joystickNumber;
            isControlledByPlayer = false;
            joystickNumber = 0;


            UpdatePlayerNumberUI();
            teamOfThisPlayer[index].UpdatePlayerNumberUI();
            PlayerManager.instance.UpdateCameraTargets();

        }
    }



    public void UpdatePlayerNumberUI()
    {
        switch (joystickNumber)
        {
            case 0:
                playerNumberText.enabled = false;
                return;
            default:
                playerNumberText.enabled = true;
                break;
        }
        switch (joystickNumber)
        {
            case 1:
                playerNumberText.color = Color.cyan;
                break;
            case 2:
                playerNumberText.color = Color.red;
                break;
            case 3:
                playerNumberText.color = Color.yellow;
                break;
            case 4:
                playerNumberText.color = Color.green;
                break;
            default:
                break;
        }

        playerNumberText.text = "P" + joystickNumber.ToString();
    }

    private void PauseGame()
    {
        if(pauseTimer < .1f)
        {
            pauseTimer += Time.unscaledDeltaTime;
        }
        else
        {
            if (pauseInput == 1 && pauseInput != lastPauseInput)
                Pause.instance.PauseGame();

            pauseTimer = 0f;
            lastPauseInput = pauseInput;
        }
    }







    private void StunPlayer()
    {
        //Contrôle la durée de paralysie du joueur


        UpdateStunFX(true);

        if (stunTimer < stunDuration)
        {
            stunTimer += Time.deltaTime;
        }
        else
        {
            isStunned = false;
            isInvincible = true;
            stunTimer = 0f;
        }
    }

    private void MakePlayerInvincible()
    {
        //Offre au joueur un petit temps d'invincibilité afin de lui permettre d'éviter les coups et de reprendre le contrôle

        UpdateStunFX(false);

        if (invincibilityTimer < invincibilityDuration)
        {
            invincibilityTimer += Time.deltaTime;
        }
        else
        {
            isInvincible = false;
            stunTimer = 0f;

            
            Color col = rend.sharedMaterial.color;
            col = new Color(col.r, col.g, col.b, 1f);
            rend.sharedMaterial.color = col;
        }
    }

    public void UpdateStunFX(bool enableFX)
    {
        stunParticles.SetActive(enableFX);
    }

    internal void DisableDashFX()
    {
        dashParticles.SetActive(false);
        superDashParticles.SetActive(false);

        dashTimer = 0f;
        specialMoveTimer = 0f;
        isDashing = false;
        isUsingSpecialMove = false;
    }










    private void SpecialMove()
    {
        if (useSpecialMove)
        {
            PlayerManager.instance.DecreaseJauge(ID % 2);
            moveDir = meshToRotate.forward;
            useSpecialMove = false;

            specialMoveAudio.SetActive(false);
            specialMoveAudio.SetActive(true);
            superDashParticles.SetActive(false);
            superDashParticles.SetActive(true);
        }

        if (specialMoveTimer < specialMoveDuration)
        {
            specialMoveTimer += Time.deltaTime;
            t.Translate(moveDir * dashSpeed * Time.deltaTime);
        }
        else
        {
            specialMoveTimer = 0f;
            isUsingSpecialMove = false;
            superDashParticles.SetActive(false);
        }
    }



    private void Move()
    {
        moveDir = new Vector3(horizontalInput, 0f, verticalInput);

        if (moveDir != Vector3.zero)
            meshToRotate.rotation = Quaternion.LookRotation(moveDir);


        t.Translate(moveDir * moveSpeed * Time.deltaTime);
    }

    private void Shoot()
    {
        Physics.Raycast(t.position, meshToRotate.forward, out ligneDeTir, 100f, whatIsPlayer, QueryTriggerInteraction.Collide);

        if (ligneDeTir.collider)
        {
            PlayerSystem playerHit = ligneDeTir.collider.transform.parent.GetComponent<PlayerSystem>();
            if (playerHit && !targetAlly)
            {
                if (ID % 2 == playerHit.ID % 2)  //Vérifie que le joueur touché par le rayon est de la même équipe
                {
                    targetAlly = ligneDeTir.collider.transform;
                }
            }
        }
        else
        {
            targetAlly = null;
        }



        if (!hasBall)
            return;




            //Debug.Log(ligneDeTir.collider.transform.parent.name);
            if (aInput > 0f)
            {
                if (targetAlly)
                {
                    //Si le rayon touche un joueur, on fait une simple passe
                    Vector3 tirDirection = targetAlly.position - t.position;   //print(tirDirection);
                    Quaternion origin = shotPoint.rotation;
                    shotPoint.rotation = Quaternion.LookRotation(tirDirection);

                    BallPhysics.instance.DetachFromPlayer(this, forcePasse);

                    shotPoint.rotation = origin;
                }
                else
                {
                    //Sinon on tire tout droit
                    BallPhysics.instance.DetachFromPlayer(this, forceTir);
                }


                shootAudio.SetActive(false);
                shootAudio.SetActive(true);

            }
    


        else if (bInput > 0f)
        {
            //Tir droit
            BallPhysics.instance.DetachFromPlayer(this, forceTir);

            shootAudio.SetActive(false);
            shootAudio.SetActive(true);

        }

        else if (yInput > 0f)
        {
            //Tir lobbé
            BallPhysics.instance.DetachFromPlayer(this, forceLobbe);

            shootAudio.SetActive(false);
            shootAudio.SetActive(true);
        }


    }

    private void Dash()
    {
        if (hasBall || isInvincible || isStunned)
        {
            dashParticles.SetActive(false);

            isDashing = false;
            dashDelayTimer = 0f;
            return;
        }

        if (dashDelayTimer >= delayBetweenDashes)
        {
            //Contrôle la durée d'un dash
            if (xInput > 0f && !isDashing)
            {
                moveDir = new Vector3(horizontalInput, 0f, verticalInput);

                if (moveDir != Vector3.zero)
                {
                    isDashing = true;
                    dashDelayTimer = 0f;

                    dashAudio.SetActive(false);
                    dashAudio.SetActive(true);
                    dashParticles.SetActive(false);
                    dashParticles.SetActive(true);
                }
            }
        }
        else
        {
            if (!isDashing)
                dashDelayTimer += Time.deltaTime;
        }


        if (isDashing)
        {
            if (dashTimer > 0)
            {
                t.Translate(moveDir.normalized * dashSpeed * Time.deltaTime);
                //print(moveDir.normalized);

                dashTimer -= Time.fixedDeltaTime;
            }
            else
            {
                isDashing = false;
                dashTimer = dashDuration;

                dashParticles.SetActive(false);
            }
        }
    }


    public void ResetInputs()
    {
        horizontalInput = 0f;
        verticalInput = 0f;
    }










    protected virtual void OnCollisionEnter(Collision col)
    {
        

        //Permet de ramasser la balle si elle est au sol
        if (col.gameObject.tag == "Entity/Ball" && !BallPhysics.instance.isHeldByPlayer && !isStunned && !isInvincible)
        {
            BallPhysics.instance.AttachToPlayer(this);
        }

        ApplyDashMalus(col);
    }

    protected virtual void OnCollisionStay(Collision col)
    {
        //Permet de ramasser la balle si elle est au sol
        if (col.gameObject.tag == "Entity/Ball" && !BallPhysics.instance.isHeldByPlayer && !isStunned && !isInvincible)
        {
            BallPhysics.instance.AttachToPlayer(this);
        }

        ApplyDashMalus(col);
    }






    private void ApplyDashMalus(Collision col)
    {

        if (col.gameObject.tag == "Entity/Player")
        {
            PlayerSystem playerHit = col.gameObject.GetComponent<PlayerSystem>();





            //Si le joueur touché est déjà stunned ou qu'il est dans notre équipe, alors on ne lui appliquera pas de malus
            if (playerHit.isStunned || playerHit.isInvincible || ID % 2 == playerHit.ID % 2)
                return;

            Vector3 knockback = col.contacts[0].point - t.position;

            //Si on utilise le special move et que l'ennemi ne l'utilise pas, alors il sera stunned lui aussi
            if (isUsingSpecialMove)
            {
                if (!playerHit.isUsingSpecialMove)
                {
                    if (playerHit.hasBall)
                    {
                        BallPhysics.instance.DetachFromPlayer(playerHit, knockback.normalized * forceRecul);
                    }
                    playerHit.isStunned = true;

                    hitAudio.SetActive(false);
                    hitAudio.SetActive(true);
                }
                return;
            }



            if (!isDashing || playerHit.isDashing || playerHit.isUsingSpecialMove)
                return;


            if (playerHit.hasBall)
            {
                BallPhysics.instance.DetachFromPlayer(playerHit, knockback.normalized * forceRecul);
                PlayerManager.instance.IncreaseJauge(ID % 2);
            }

            playerHit.isStunned = true;

            dashParticles.SetActive(false);
            hitAudio.SetActive(false);
            hitAudio.SetActive(true);
        }
    }



    protected virtual void OnDrawGizmos()
    {
        if (t == null || meshToRotate == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(t.position, moveDir);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(meshToRotate.position, meshToRotate.forward);


        if (ligneDeTir.collider)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(t.position, meshToRotate.forward * 100f);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(t.position, meshToRotate.forward * 100f);
        }


        if (targetAlly && hasBall)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(t.position, targetAlly.position);
        }

    }
}
