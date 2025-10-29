using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableOrg : MonoBehaviour
{

    private float currentCenterCardsHeight = 0f;

    private int currentIndexInsert = 0;

    public List<Card> tableCards;

    [SerializeField]
    private SOB_Card[] cardsArchive;

    public Stack<SOB_Card> TableDeck { get; private set; }

    [SerializeField]
    private GameObject visualObj;

    [SerializeField]
    private GameObject briscolaObj;

    [SerializeField]
    private GameObject cardPrefab;

    private void Awake()
    {
        TableDeck = new Stack<SOB_Card>();
    }


    //Si occupa di mischiare le carte dentro all'archivio e poi le da in pasto allo stack TableDeck
    public void ShuffleDeck()
    {
        for (int i = 0; i < Random.Range(40, 100); i++)
        {
            int index1 = Random.Range(0, cardsArchive.Length);
            int index2;
            int way = Random.Range(0, 1);
            if (index1 == 0)
            {
                index2 = Random.Range(1, cardsArchive.Length);
            }
            else if (index1 == cardsArchive.Length - 1)
            {
                index2 = Random.Range(0, cardsArchive.Length - 1);
            }
            else if (way == 0)
            {
                index2 = Random.Range(0, index1);
            }
            else
            {
                index2 = Random.Range(index1 + 1, cardsArchive.Length);
            }
            (cardsArchive[index2], cardsArchive[index1]) = (cardsArchive[index1], cardsArchive[index2]);
        }
        for (int i = 0; i < 40; i++)
        {

            TableDeck.Push(cardsArchive[i]);
        }
    }

    //Si occupa di animare le carte via codice e di settare la carta briscolaObj
    //In questo metodo sono state utilizzate le funzionalita' del pacchetto LeanTween,
    //Semplicemente si occupa di applicare movimenti e trasformazioni agli oggetti nella scena
    //I metodi del pacchetto utilizzati iniziano sempre con "Lean"
    public SOB_Card TakeFirstCardForBriscola()
    {
        SOB_Card briscola = cardsArchive[0];

        
        Card briscolaCard = briscolaObj.GetComponent<Card>();
        briscolaCard.SetCardInfo(briscola);
        briscolaObj.transform.LeanMoveY(briscolaObj.transform.position.y + 0.04f, 0.5f);
        briscolaObj.transform.LeanRotateAroundLocal(new Vector3(0, 1, 0), -180, 0.5f).setOnComplete(() =>   //Azioni concatenate che vengono quindi eseguite non appena finisce la precendente
        {
            briscolaObj.transform.LeanMoveY(briscolaObj.transform.position.y - 0.04f, 0.2f).setOnComplete(() =>
            {
                briscolaObj.transform.LeanRotateAroundLocal(new Vector3(0, 0, 1f), -260f, 0.5f);
                briscolaObj.transform.LeanMove(new Vector3(visualObj.transform.position.x + 0.2f, briscolaObj.transform.position.y, visualObj.transform.position.z - 0.2f), 0.5f).setOnComplete(() =>
                {
                    briscolaObj.transform.LeanMove(new Vector3(visualObj.transform.position.x + 0.13f, visualObj.transform.position.y, visualObj.transform.position.z - 0.09f), 0.5f);
                });
                
            });
        });
        return briscola;
    }

    //Gestisce sempre le animazioni per dare le carte ad un giocatore
    //"posTogo" e' la posizione verso cui si dirigera' la carta mentre "delega" e' un metodo che verra' eseguito non appena sara' terminata l'animazione
    public SOB_Card TakeCard(Vector3 posToGo, System.Action delega)
    {
        if(TableDeck.Count == 1)
        {
            Vector3 relaPos = posToGo - briscolaObj.transform.position;
            Quaternion rot = Quaternion.LookRotation(relaPos);

            briscolaObj.transform.LeanRotate(rot.eulerAngles + new Vector3(90, 0, 0), 0.5f).setOnComplete(() =>
            {
                briscolaObj.transform.LeanMoveY(briscolaObj.transform.position.y + 0.09f, 0.3f).setOnComplete(() =>
                {
                    briscolaObj.transform.LeanMove(posToGo, 0.3f).setOnComplete( () =>
                    {
                        briscolaObj.SetActive(false);
                        delega();
                        
                    });
                });
            });
        }
        else
        {
            GameObject cardInstantiate = Instantiate(cardPrefab, new Vector3(visualObj.transform.position.x, visualObj.transform.position.y + 0.06f, visualObj.transform.position.z), visualObj.transform.rotation);
            Vector3 relaPos = posToGo - cardInstantiate.transform.position;
            Quaternion rot = Quaternion.LookRotation(relaPos);

            cardInstantiate.transform.LeanRotate(rot.eulerAngles + new Vector3(90, 0, 0), 0.5f).setOnComplete(() =>
            {
                cardInstantiate.transform.LeanMoveY(cardInstantiate.transform.position.y + 0.09f, 0.3f).setOnComplete(() =>
                {
                    cardInstantiate.transform.LeanMove(posToGo, 0.3f).setDestroyOnComplete(true).setOnComplete(delega);
                });
            });
            if (TableDeck.Count == 2)
            {
                visualObj.transform.localScale = Vector3.zero;
            }
            else
            {
                visualObj.transform.localScale = new Vector3(visualObj.transform.localScale.x, visualObj.transform.localScale.y, visualObj.transform.localScale.z - 0.013f);
            }
        }
        /*
        GameObject cardInstantiate = Instantiate(cardPrefab, new Vector3(visualObj.transform.position.x, visualObj.transform.position.y + 0.06f, visualObj.transform.position.z), visualObj.transform.rotation);
        Vector3 relaPos = posToGo - cardInstantiate.transform.position;
        Quaternion rot = Quaternion.LookRotation(relaPos);

        cardInstantiate.transform.LeanRotate(rot.eulerAngles + new Vector3(90, 0, 0), 0.5f).setOnComplete(() =>
        {
            cardInstantiate.transform.LeanMoveY(cardInstantiate.transform.position.y + 0.09f, 0.3f).setOnComplete(() =>
            {
                cardInstantiate.transform.LeanMove(posToGo, 0.3f).setDestroyOnComplete(true).setOnComplete(delega);
            });
        });
        
        
        if(TableDeck.Count == 2)
        {
            visualObj.transform.localScale = Vector3.zero;
            
        }
        else if(TableDeck.Count == 1)
        {
            briscolaObj.SetActive(false);
        }
        else
        {
            visualObj.transform.localScale = new Vector3(visualObj.transform.localScale.x, visualObj.transform.localScale.y, visualObj.transform.localScale.z - 0.013f);
        }*/
        SOB_Card extraction = TableDeck.Pop();
        return extraction;
    }

    //Si occupa sempre dell'aspetto visivo di quando il round termina pero' questa volta
    public IEnumerator DoWinRoundAnimation(Vector3 posToGo)
    {
        List<GameObject> cards = new List<GameObject>();
        float height = 0;
        for (int i = 0, d = currentIndexInsert; i < 4; i++, d--)
        {
            if(d < 0)
            {
                d = tableCards.Count - 1;
            }
            Vector3 pos = tableCards[d].transform.position;
            GameObject newCard = Instantiate(tableCards[d].gameObject, pos, tableCards[d].transform.rotation);
            cards.Add(newCard);

            newCard.LeanMoveY(pos.y + 0.15f + height, 0.5f);
            Vector3 relaPos = posToGo - newCard.transform.position;
            Quaternion rot = Quaternion.LookRotation(relaPos);
            newCard.transform.LeanRotate(rot.eulerAngles + new Vector3(90, 0, 0), 0.5f);
            //newCard.LeanRotateAroundLocal(new Vector3(0, 1, 0), 180, 0.5f);
            tableCards[d].gameObject.SetActive(false);
            tableCards[d].cardInfo = null;
            height += 0.01f;
            yield return new WaitForSeconds(0.2f);
        }
        foreach (var item in cards)
        {
            item.LeanMove(posToGo, 0.5f).setDestroyOnComplete(true);
            yield return new WaitForSeconds(0.2f);
        }
        currentCenterCardsHeight = 0f;

    }

    public Card[] GetCurrentTableCards()
    {
        return tableCards.ToArray();
    }

    public Card GetTableCardIndex(int index)
    {
        return tableCards[index];
    }

    //Una struct che mi serve per avere delle informazioni impacchettate su chi e' stato a vincere il round e che carte ha vinto
    public struct Win
    {
        public Player player;
        public List<SOB_Card> cards;
    }

    //Ritorna le informazioni sulla vincita 
    public Win CalculateVincitaRound(int indexStartPlayer, Player[] players)
    {
        SOB_Card firstCard = tableCards[indexStartPlayer].GetComponent<Card>().cardInfo;

        CardSeed dominantSeed = firstCard.cardSeed;
        Card winningCard = tableCards[indexStartPlayer].GetComponent<Card>();
        CardSeed briscolaSeed = briscolaObj.GetComponent<Card>().cardInfo.cardSeed;
        List<SOB_Card> cards = new List<SOB_Card>();

        foreach (var card in tableCards)
        {
            if (card != winningCard)
            {

                if (dominantSeed != briscolaSeed)
                {
                    if (card.cardInfo.cardSeed == briscolaSeed)
                    {
                        winningCard = card;
                        dominantSeed = briscolaSeed;
                    }
                }

                if (card.cardInfo.cardSeed == dominantSeed)
                {

                    if (card.cardInfo.cartValue != 0)
                    {
                        if (winningCard.cardInfo.cartValue != 0)
                        {
                            if (winningCard.cardInfo.cartValue < card.cardInfo.cartValue)
                            {
                                winningCard = card;
                            }
                        }
                        else
                        {
                            winningCard = card;
                        }
                    }
                    else if (winningCard.cardInfo.cartValue == 0)
                    {
                        if (winningCard.cardInfo.cardNumber < card.cardInfo.cardNumber)
                        {
                            winningCard = card;
                        }
                    }
                }
            }

            cards.Add(card.cardInfo);
        }
        Player winningPlayer = players[tableCards.IndexOf(winningCard)];



        return new Win { cards = cards, player = winningPlayer };


    }

    //Ritorna la carta che al momento sta vincendo il bancone
    public Card GetCurrentDominantTableCard(int indexStartPlayer)
    {
        Card dominantCard = tableCards[indexStartPlayer];
        Card briscolaCard = briscolaObj.GetComponent<Card>();

        int cardsOnTable = 0;
        foreach (var card in tableCards)
        {
            if (card.cardInfo != null)
            {
                cardsOnTable++;
            }
        }
        if(cardsOnTable == 1)
        {
            return dominantCard;
        }
        Card oldDominantCard = dominantCard;
        foreach (var card in tableCards)
        {
            if (card.cardInfo != null && card != oldDominantCard)
            {
                if (card.cardInfo.cardSeed == briscolaCard.cardInfo.cardSeed && dominantCard.cardInfo.cardSeed != briscolaCard.cardInfo.cardSeed)
                {
                    dominantCard = card;
                }
                else if (card.cardInfo.cardSeed == dominantCard.cardInfo.cardSeed)
                {
                    if (card.cardInfo.cartValue != 0)
                    {
                        if (dominantCard.cardInfo.cartValue != 0)
                        {
                            if (card.cardInfo.cartValue > dominantCard.cardInfo.cartValue)
                            {
                                dominantCard = card;
                            }
                        }
                        else
                        {
                            dominantCard = card;
                        }
                    }
                    else if (dominantCard.cardInfo.cartValue == 0)
                    {
                        if (card.cardInfo.cardNumber > dominantCard.cardInfo.cardNumber)
                        {
                            dominantCard = card;
                        }
                    }
                }
            }
        }
        
       
        /*for (int i = 1; i < cardsOnTable; i++)
        {
            if (tableCards[i].cardInfo != null)
            {
                if (tableCards[i].cardInfo.cardSeed == briscolaCard.cardInfo.cardSeed && dominantCard.cardInfo.cardSeed != briscolaCard.cardInfo.cardSeed)
                {
                    dominantCard = tableCards[i];
                }
                else if (tableCards[i].cardInfo.cardSeed == dominantCard.cardInfo.cardSeed)
                {
                    if (tableCards[i].cardInfo.cartValue != 0)
                    {
                        if (dominantCard.cardInfo.cartValue != 0)
                        {
                            if (tableCards[i].cardInfo.cartValue > dominantCard.cardInfo.cartValue)
                            {
                                dominantCard = tableCards[i];
                            }
                        }
                        else
                        {
                            dominantCard = tableCards[i];
                        }
                    }
                    else if (dominantCard.cardInfo.cartValue == 0)
                    {
                        if (tableCards[i].cardInfo.cardNumber > dominantCard.cardInfo.cardNumber)
                        {
                            dominantCard = tableCards[i];
                        }
                    }
                }
            }
            
        }*/
        return dominantCard;
    }

    //Ritorna i punti che al momento sono sul bancone
    public int GetCurrentTablePoints()
    {
        int tot = 0;
        foreach (var card in tableCards)
        {
            if (card.gameObject.activeInHierarchy)
                tot += card.cardInfo.cartValue;
        }
        return tot;
    }

    //Lascia una carta in base al player che l'ha droppata
    public void DropCardOnTable(int indexPlayer, SOB_Card cardInfo)
    {
        Card cardObj = tableCards[indexPlayer];
        currentIndexInsert = indexPlayer;
        cardObj.SetCardInfo(cardInfo);
        cardObj.gameObject.SetActive(true);
        cardObj.SetCardHeight(currentCenterCardsHeight);
        currentCenterCardsHeight += 0.005f;
    }

    public Card GetBriscola()
    {
        return briscolaObj.GetComponent<Card>();
    }

    //Questo metodo mi serve per risistemare il sistema in caso la partita sia solo di 2 giocatori
    public void SetUpForTwo()
    {
        tableCards.RemoveAt(3);
        tableCards.RemoveAt(1);
    }

}
