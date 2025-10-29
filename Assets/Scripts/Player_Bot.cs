
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Bot : Player
{
    
    private Difficulty difficulty;

    public void InitializeBot(Difficulty difficulty)
    {
        this.difficulty = difficulty;
    }

    public override IEnumerator OnTurn()
    {
        foreach (var card in cardsLogic)
        {
            var cardT = cardsObj[cardsObj.IndexOf(card)];
            cardT.SetCardVisible();
        }
        yield return new WaitForSeconds(1f);
        animator.SetTrigger("OnSelection");
        StartCoroutine(Attendi());
        

    }


    //Questo metodo si occupa di decidere la carta che sara' lasciata sul bancone in base alla difficolta' ed in caso a determinate situazioni,
    //in poche parole e' l'intelligenza dei Bot
    public IEnumerator Attendi()
    {
        animator.ResetTrigger("Card0");
        animator.ResetTrigger("Card1");
        animator.ResetTrigger("Card2");
        yield return new WaitForSeconds(0.5f);
        if (cardsLogic.Count == 1)
        {
            //Caso in cui e' rimasta una sola carta in mano
            int index = cardsObj.IndexOf(cardsLogic[0]);
            if (index == 0)
            {
                animator.SetTrigger("Card0");
            }
            else if (index == 1)
            {
                animator.SetTrigger("Card1");
            }
            else
            {
                animator.SetTrigger("Card2");
            }
            yield return new WaitForSeconds(Random.Range(1f, 2f));
            StartCoroutine(CardSelected(cardsObj[index]));
        }
        else
        {
            switch (difficulty)
            {
                case Difficulty.EASY:
                    int randomIndex = 0;
                    for (int i = 0; i < Random.Range(0, 1); i++)
                    {
                        yield return new WaitForSeconds(Random.Range(0.5f, 1f));
                        randomIndex = Random.Range(0, cardsLogic.Count);
                        randomIndex = cardsObj.IndexOf(cardsLogic[randomIndex]);
                        if (randomIndex == 0)
                        {
                            animator.SetTrigger("Card0");
                        }
                        else if (randomIndex == 1)
                        {
                            animator.SetTrigger("Card1");
                        }
                        else
                        {
                            animator.SetTrigger("Card2");
                        }
                        yield return new WaitForSeconds(Random.Range(0.5f, 1f));
                    }
                    StartCoroutine(CardSelected(cardsObj[randomIndex]));
                    break;
                case Difficulty.NORMAL:
                    int indexPlayer = manager.GetPlayerIndex(this);
                    for (int i = 0; i < Random.Range(0, 1); i++)
                    {
                        //yield return new WaitForSeconds(Random.Range(0.5f, 1f));
                        randomIndex = Random.Range(0, cardsLogic.Count);
                        randomIndex = cardsObj.IndexOf(cardsLogic[randomIndex]);
                        if (randomIndex == 0)
                        {
                            animator.SetTrigger("Card0");
                        }
                        else if (randomIndex == 1)
                        {
                            animator.SetTrigger("Card1");
                        }
                        else
                        {
                            animator.SetTrigger("Card2");
                        }
                        yield return new WaitForSeconds(0.5f);
                    }
                    Card cardToDrop;
                    if (indexPlayer == 0)
                    {
                        cardToDrop = CalculateMinCard();
                        
                    }
                    else if(indexPlayer == manager.GetPlayersCountTotal() - 1)
                    {
                        cardToDrop = EvaluatePossibilitiesToTake(false);

                    }
                    else
                    {
                        cardToDrop = EvaluatePossibilitiesToTake(true);
                    }
                    yield return new WaitForSeconds(0.5f);
                    int index = cardsObj.IndexOf(cardToDrop);
                    if (index == 0)
                    {
                        animator.SetTrigger("Card0");
                    }
                    else if (index == 1)
                    {
                        animator.SetTrigger("Card1");
                    }
                    else
                    {
                        animator.SetTrigger("Card2");
                    }
                    yield return new WaitForSeconds(0.5f);
                    StartCoroutine(CardSelected(cardToDrop));


                    break;
                case Difficulty.HARD:
                    //Alla fine non mi sono messo ad implementare una difficola' maggiore

                    break;
            }
        }

        


    }

    //Si occupa di decidere la carta da lasciare in base alle carte che ha in mano,
    //alle carte che sono attualmente sul bancone ed in che momento del round si trova a giocare
    private Card EvaluatePossibilitiesToTake(bool min)
    {
        Card cardToDrop = null;

        Card dominantTableCard = tableOrganizer.GetCurrentDominantTableCard(manager.IndexStartPlayer);
        if (tableOrganizer.GetCurrentTablePoints() > 0)
        {
            Card dominantCardInHand = null;
            
            CardSeed briscolaSeed = tableOrganizer.GetBriscola().cardInfo.cardSeed;
            foreach (var cardInHand in cardsLogic)
            {
                Card winCard = WinCardBetweenTwo(dominantTableCard, cardInHand, briscolaSeed);
                if (winCard == cardInHand)
                {
                    if (dominantCardInHand == null)
                    {
                        dominantCardInHand = winCard;
                    }
                    else
                    {
                        Card minCard;
                        if (min)
                            minCard = MinBetweenTwoCard(dominantCardInHand, cardInHand, briscolaSeed);
                        else
                            minCard = MaxBetweenTwo(dominantCardInHand, cardInHand, briscolaSeed);
                        if (minCard == cardInHand)
                        {
                            dominantCardInHand = cardInHand;
                        }
                    }
                }
            }
            cardToDrop = dominantCardInHand;
        }
        else
        {
            cardToDrop = IHaveInHandSameSeedWithPoints(dominantTableCard);
        }
        
        if(cardToDrop == null)
        {
            cardToDrop = CalculateMinCard();
        }
        return cardToDrop;
    }

    //Ritorna la carta con valore minore tra due carte
    private Card MinBetweenTwoCard(Card card0, Card card1, CardSeed briscola)
    {
        Card minCard = card0;
        if(card0.cardInfo.cardSeed == briscola)
        {
            if(card1.cardInfo.cardSeed != briscola)
            {
                return card1;
            }
        }
        else if(card1.cardInfo.cardSeed == briscola)
        {
            return card0;
        }

        if(card0.cardInfo.cartValue != 0)
        {
            if(card1.cardInfo.cartValue != 0)
            {
                if(card1.cardInfo.cartValue < card0.cardInfo.cartValue)
                {
                    minCard = card1;
                }
            }
            else
            {
                minCard = card1;
            }
        }
        else if(card1.cardInfo.cartValue == 0)
        {
            if(card1.cardInfo.cardNumber < card0.cardInfo.cardNumber)
            {
                minCard = card1;
            }
        }

        return minCard;
    }

    //Ritorna la carta con valore maggiore tra due carte
    private Card MaxBetweenTwo(Card card0, Card card1, CardSeed briscola)
    {
        Card maxCard = card0;
        if(card0.cardInfo.cardSeed == briscola)
        {
            if(card1.cardInfo.cardSeed != briscola)
            {
                return card1;
            }
        }
        else if(card1.cardInfo.cardSeed == briscola)
        {
            return card0;
        }

        if(card0.cardInfo.cartValue != 0)
        {
            if(card1.cardInfo.cartValue != 0)
            {
                if(card1.cardInfo.cartValue > card0.cardInfo.cartValue)
                {
                    maxCard = card1;
                }
            }
            
        }
        else if(card1.cardInfo.cartValue != 0)
        {
            maxCard = card1;
            
        }
        else
        {
            if (card1.cardInfo.cardNumber > card0.cardInfo.cardNumber)
            {
                maxCard = card1;
            }
        }

        return maxCard;
    }

    //Ritorna la carta vincente tra due carte
    private Card WinCardBetweenTwo(Card card0, Card card1, CardSeed briscola)
    {
        Card winningCard = card0;
        if(card0.cardInfo.cardSeed == briscola)
        {
            if(card1.cardInfo.cardSeed == briscola)
            {
                if(card0.cardInfo.cartValue != 0)
                {
                    if(card1.cardInfo.cartValue != 0)
                    {
                        if(card0.cardInfo.cartValue > card1.cardInfo.cartValue)
                        {
                            winningCard = card0;
                        }
                        else
                        {
                            winningCard = card1;
                        }
                    }
                    else
                    {
                        winningCard = card0;
                    }
                }
                else if(card0.cardInfo.cartValue != 0)
                {
                    winningCard = card0;
                }
                else
                {
                    if(card0.cardInfo.cardNumber > card1.cardInfo.cardNumber)
                    {
                        winningCard = card0;
                    }
                    else
                    {
                        winningCard = card1;
                    }
                }
            }
            else
            {
                winningCard = card0;
            }
        }else if(card1.cardInfo.cardSeed == briscola)
        {
            winningCard = card1;
        }
        else if(card0.cardInfo.cardSeed == card1.cardInfo.cardSeed)
        {
            if(card0.cardInfo.cartValue != 0)
            {
                if(card1.cardInfo.cartValue != 0)
                {
                    if(card0.cardInfo.cartValue > card1.cardInfo.cartValue)
                    {
                        winningCard = card0;
                    }
                    else
                    {
                        winningCard = card1;
                    }
                }
                else
                {
                    winningCard = card0;
                }
            }
            else if(card1.cardInfo.cartValue != 0)
            {
                winningCard = card1;
            }
            else if(card0.cardInfo.cardNumber < card1.cardInfo.cardNumber)
            {
                winningCard = card1;
            }
        }
        return winningCard;
    }

    //Ritorna la carta che ha valore minore in mano
    private Card CalculateMinCard()
    {
        Card minCard = null;
        CardSeed briscolaSeed = tableOrganizer.GetBriscola().cardInfo.cardSeed;
        foreach (var card in cardsLogic)
        {
            if(card.cardInfo.cardSeed != briscolaSeed)
            {
                if(card.cardInfo.cartValue == 0)
                {
                    if(minCard == null)
                    {
                        minCard = card;
                    }
                    else if(minCard.cardInfo.cardNumber > card.cardInfo.cardNumber)
                    {
                        minCard = card;
                    }
                    else if(card.cardInfo.cartValue < 10)
                    {
                        minCard = card;
                    }
                    
                }
            }
        }
        
        if (minCard == null)
            foreach (var card in cardsLogic)
            {
                if (card.cardInfo.cardSeed == briscolaSeed)
                {
                    if (card.cardInfo.cartValue == 0)
                    {
                        if (minCard == null)
                        {
                            minCard = card;
                        }
                        else if (minCard.cardInfo.cardNumber > card.cardInfo.cardNumber)
                        {
                            minCard = card;
                        }

                    }
                    else if(card.cardInfo.cartValue < 10)
                    {
                        minCard = card;
                    }
                }
            }
        if (minCard == null)
            foreach (var card in cardsLogic)
            {
                if (card.cardInfo.cardSeed != briscolaSeed)
                {
                    if(minCard == null)
                    {
                        minCard = card;
                    }
                    else if(minCard.cardInfo.cartValue > card.cardInfo.cartValue)
                    {
                        minCard = card;
                    }
                }
            }
        if (minCard == null)
            foreach (var card in cardsLogic)
            {
                if (minCard == null)
                {
                    minCard = card;
                }
                else if (minCard.cardInfo.cartValue > card.cardInfo.cartValue)
                {
                    minCard = card;
                }
            }
        return minCard;
    }


    //Ritorna in caso la carta che ha lo stesso seme,in mano, di una carta
    private Card IHaveInHandSameSeedWithPoints(Card cardTable)
    {
        Card cardToDrop = null;
        foreach (var card in cardsLogic)
        {
            if(card.cardInfo.cardSeed == cardTable.cardInfo.cardSeed)
            {
                if(card.cardInfo.cartValue > cardTable.cardInfo.cartValue)
                {
                    if(cardToDrop == null)
                        cardToDrop = card;  
                    else if(card.cardInfo.cartValue > cardToDrop.cardInfo.cartValue)
                    {
                        cardToDrop = card;
                    }
                        
                }
            }
        }
        return cardToDrop;
    }

    public enum Difficulty
    {
        EASY = 0,
        NORMAL = 1,
        HARD = 2
    }
}
