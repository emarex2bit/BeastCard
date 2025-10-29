using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{

    protected TableOrg tableOrganizer;
    public bool isPlayer = false;
    [SerializeField]
    private Card cardToDrop;

    [SerializeField]
    protected Animator animator;

    [SerializeField]
    protected BeastManager manager;

    [SerializeField]
    protected List<Card> cardsObj;

    protected List<Card> cardsLogic;

    [SerializeField]
    public Transform HandTrasform { get; private set; }
    [SerializeField]
    private Transform RightHandTrasform;

    private Camera mainCamera;

    private Card cardDropped;

    private List<SOB_Card> deck;

    private int countCardToStart = 0;


    // Start is called before the first frame update
    void Start()
    {
        cardsLogic = new List<Card>();
        deck = new List<SOB_Card> ();
        HandTrasform = gameObject.transform.Find("Visual/Root/Hips/Spine_01/Spine_02/Spine_03/Clavicle_L/Shoulder_L/Elbow_L/Hand_L");
        RightHandTrasform = gameObject.transform.Find("Visual/Root/Hips/Spine_01/Spine_02/Spine_03/Clavicle_R/Shoulder_R/Elbow_R/Hand_R");
        cardToDrop.SetParent(RightHandTrasform);
        mainCamera = Camera.main;
        foreach (var card in cardsObj)
        {
            cardsLogic.Add(card);
            card.SetParent(HandTrasform);
        }
    }

    public void SetManager(BeastManager manager)
    {
        this.manager = manager;
    }


    public virtual IEnumerator OnTurn()
    {
        
        mainCamera.gameObject.GetComponent<RotateCamera>().SetOffCursoreLockState();//Per visualizzare il mouse
        //Setup delle carte
        foreach (var card in cardsLogic)
        {
            var cardT = cardsObj[cardsObj.IndexOf(card)];
            cardT.SetPlayer(this);
        }
        yield return new WaitForSeconds(0.5f);

        
        //Attivata la possibilita' di selezionare una carta
        foreach (var card in cardsLogic)
        {
            var cardT = cardsObj[cardsObj.IndexOf(card)];
            cardT.ActiveSelection();                    //In questo modo le carte sono notificate che possono essere selezionate dal giocatore
        }
        animator.SetTrigger("OnSelection");
        //Triggerata l'animazione di selezione sul player corrente

    }

    //Questo metodo si occupa solo di triggerare l'animazione per avvicinarsi alla carta sopra cui e' il mouse
    public void OnCardHover(Card card)
    {
        animator.ResetTrigger("Card0");
        animator.ResetTrigger("Card1");
        animator.ResetTrigger("Card2");
        int indexOfCard = cardsObj.IndexOf(card);
        string trigger = "Card0";
        if(indexOfCard != -1)
        {
            if(indexOfCard == 1)
            {
                trigger = "Card1";
            }
            else if(indexOfCard == 2)
            {
                trigger = "Card2";
            }
        }
        animator.SetTrigger(trigger);
    }

    //Metodo eseguito quando una carta viene selezionata
    public IEnumerator CardSelected(Card cardSelected)
    {
        cardsLogic.Remove(cardSelected);
        cardDropped = cardSelected;
        foreach (var card in cardsObj)
        {
            card.DeactiveSelection();
        }
        cardDropped.transform.localScale = Vector3.zero;
        cardToDrop.SetCardInfo(cardSelected.cardInfo);
        cardToDrop.gameObject.SetActive(true);
        StartCoroutine(DropCard(cardDropped.cardInfo));
        yield return new WaitForSeconds(0.9f);
        cardToDrop.gameObject.SetActive(false);
        
    }

    //Per eseguire l'animazione per lasciare una carta e per darla al manager
    protected IEnumerator DropCard(SOB_Card cardDropped)
    {
        if(isPlayer)
            mainCamera.gameObject.GetComponent<RotateCamera>().SetOnCursorLockState();
        animator.SetTrigger("Drop");
        yield return new WaitForSeconds(1f);
        manager.CardDropped(cardDropped, this);
        
    }

    //Serve all'inizio della partita per aggiungere le carte alla mano
    public void SetCard(SOB_Card sOB_Card)
    {
        if(countCardToStart < 3)
        {
            cardsObj[countCardToStart].SetCardInfo(sOB_Card);
            cardsObj[countCardToStart].gameObject.SetActive(true);
            countCardToStart++;
        }
        
    }

    public void SetAnimator(Animator animator)
    {
        this.animator = animator;
    }

    public void AddCardsToDeck(List<SOB_Card> cards)
    {
        deck.AddRange(cards);
    }

    public int GetPoints()
    {
        int points = 0;
        foreach (var card in deck)
        {
            points += card.cartValue;
        }
        return points;
    }

    //Serve per sostituire e riabilitare la carta lasciata precedentemente
    public void AddCardToHand(SOB_Card sOB_Card)
    {
        cardDropped.SetCardInfo(sOB_Card);
        //cardDropped.SetCardVisible();
        cardsLogic.Add(cardDropped);
        cardDropped.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
    }

    public IEnumerator WinRoundAnimation()
    {
        foreach (var item in cardsObj)
        {
            item.SetCardInvisible();
        }
        
        animator.SetTrigger("RoundWin");
        yield return new WaitForSeconds(2.5f);
        foreach (var item in cardsObj)
        {
            Card card = cardsLogic.Find(x => x == item);
            if (card != null)
                item.SetCardVisible();
        }
    }

    public void DoWin()
    {
        
        animator.SetTrigger("Win");
    }

    public void OnGameEnd()
    {
        if(isPlayer)
        {
            mainCamera.gameObject.GetComponent<RotateCamera>().SetOffCursoreLockState();
        }
    }

    public void SetTableOrganizer(TableOrg tableOrganizer)
    {
        this.tableOrganizer = tableOrganizer;
    }
}
