using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    private bool isMouseHover;
    private Vector3 scaleBorderNeedToSelect = new Vector3(1.75f, 3f, 0.11f);
    private Vector3 scaleBorderNotNeedToSelect = new Vector3(1.6f, 2.85f, 0.12f);
    private Vector3 scaleSelected = new Vector3(0.08f, 0.08f, 0.08f);
    private Vector3 scaleNotSelected = new Vector3(0.07f, 0.07f, 0.07f);
    private Player playerInPosses;

    [SerializeField]
    private GameObject borderSelection;

    [SerializeField]
    private GameObject border;

    private Vector3 startPosition;

    public SOB_Card cardInfo = null;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private bool canBeSelected = false;

    // Start is called before the first frame update
    void Start()
    {
        isMouseHover = false;
        //border.transform.localScale = scaleBorderNotNeedToSelect;
    }


    //"Valorizzare" la carta
    public void SetCardInfo(SOB_Card cardInfo)
    {
        this.cardInfo = cardInfo;
        spriteRenderer.sprite = cardInfo.sprite;
    }

    public void ActiveSelection()
    {
        canBeSelected = true;
        border.transform.localScale = scaleBorderNeedToSelect;
    }

    public void DeactiveSelection()
    {
        canBeSelected = false;
        border.transform.localScale = scaleBorderNotNeedToSelect;
    }

    private void SetBorderOn()
    {
        borderSelection.SetActive(true);
    }
    private void SetBorderOff()
    {
        borderSelection.SetActive(false);
    }

    //Metodo chiamato quando il mouse  clicca su questo oggetto
    private void OnMouseDown()
    {
        if (canBeSelected && playerInPosses != null)
        {
            startPosition = transform.position;
            StartCoroutine(playerInPosses.CardSelected(this));
            
            SetBorderOff();
            isMouseHover = false;
        }
    }

    //Metodo chiamato quando il mouse arriva sopra a questo oggetto
    private void OnMouseEnter()
    {
        if (canBeSelected && !isMouseHover)
        {
            SetBorderOn();
            transform.localScale = scaleSelected;
            isMouseHover = true;
            playerInPosses.OnCardHover(this);
        }
    }

    //Metodo chiamato quando il mouse si toglie da sopra a questo oggetto
    private void OnMouseExit()
    {
        if (canBeSelected)
        {
            isMouseHover = false;
            SetBorderOff();
            transform.localScale = scaleNotSelected;
        }
    }

    public void ResetPosition()
    {
        transform.position = startPosition;
    }

    public void SetPlayer(Player player)
    {
        playerInPosses = player;
    }

    public void SetParent(Transform parentToAttach)
    {   
        transform.SetParent(parentToAttach);
    }

    public void SetCardHeight(float height)
    {
        transform.localPosition = new Vector3(transform.localPosition.x, height, transform.localPosition.z);
    }

    public void SetCardInvisible()
    {
        transform.localScale = Vector3.zero;
    }

    public void SetCardVisible()
    {
        transform.localScale = scaleNotSelected;
    }
}
