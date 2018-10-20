using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UiInputs : MonoBehaviour {

    //Ce script est chargé de capturer les inputs de chaque joueur connecté. Il est utilisé pour activer les UIs de sélection d'équipe (Joueurs 1 et 2) seulement, 
    //et pour stocker le joystickNumber qui sera envoyé à la scène suivante



    public int joystickNumber;
    private bool isReady;

    [Space(20)]

    private float horizontal;
    private float vertical;
    private float A;
    private float Y;

    private float lastAInput;
    private float lastYInput;
    private float lastATimer;
    private float lastYTimer;


    [Space(20)]


    public Button left;
    public Button right;
    public Toggle t;
    
    [Space(20)]

    public EventSystem eventSystem;

    


    private void Update()
    {
        if (!left && !right && !t && !eventSystem)
        {
            //On ne prendra en compte que le joystickNumber du joueur
            return;
        }

        GetInput();
        ActivateButtons();
    }




    private void GetInput()
    {


        horizontal = Input.GetAxisRaw("LeftJoystickX_P" + joystickNumber);

        vertical = Input.GetAxisRaw("LeftJoystickY_P" + joystickNumber);

        A = Input.GetAxis("A" + joystickNumber);
        Y = Input.GetAxis("Y" + joystickNumber);


        //Si le joueur a confirmé son choix, alors il ne peut plus naviguer ailleurs dans son UI
        if (isReady)
            return;

        if (horizontal > 0f)
        {
            right.Select();
        }
        else if (horizontal < 0f)
        {
            left.Select();
        }




        if (vertical < 0f)
        {
            eventSystem.SetSelectedGameObject(t.gameObject);

        }
        else if (vertical > 0f)
        {
            if (eventSystem.currentSelectedGameObject == t.gameObject)
                eventSystem.SetSelectedGameObject(right.gameObject);
        }



    }

    private void ActivateButtons()
    {

        //On récupère le bouton actuellement sélectionné par le joueur
        if (eventSystem.currentSelectedGameObject)
        {

            Button b = eventSystem.currentSelectedGameObject.GetComponent<Button>();

            //Ce qui suit est une sécurité qui permet d'éviter de parcourir plusiuers équipes de la liste disponible en une seule frame.
            //Une fois que le bouton A est appuyé, on le compare avec la valeur lastAInput, qui correspondant à la dernière valeur de l'input
            //Si les deux valeurs son identiques, alors le bouton ne peut plus être activé. Cela force le joueur à relâcher le bouton A pour réactiver la fonction de l'UI et 
            //l'empêche de parcourir toute la liste en une seule frame.
            if (lastATimer < .01f)
            {
                lastATimer += Time.unscaledDeltaTime;
            }
            else
            {


                if (A == 1f && A != lastAInput)
                {
                    if (b)
                    {
                        b.onClick.Invoke();
                    }
                    else if (t)
                    {
                        t.isOn = !t.isOn;
                    }
                }

                lastATimer = 0f;
                lastAInput = A;
            }
        }


        //Ce qui suit est le même type de sécurité que la précédente. Le bouton Y permet d'annuler la commande du joueur et lui permettre de sélectionner une autre équipe s'il souhaite revenir sur son choix.

        if (lastYTimer < .01f)
        {
            lastYTimer += Time.unscaledDeltaTime;
        }
        else
        {


            if (Y == 1f && Y != lastYInput)
            {
                if (t.isOn)
                {
                    t.isOn = false;
                    eventSystem.SetSelectedGameObject(right.gameObject);
                }
            }

            lastYTimer = 0f;
            lastYInput = Y;



        }
    }

    //Appelé par le script ValidateTeam lorsque le joueur a activé son toggle correspondant. Cela l'empêcher de continuer à naviguer dans le reste de son UI
    public void LockInputsWhenReady(bool b)
    {
        if(isReady != b)
        {
            isReady = b;
        }
    }

}
