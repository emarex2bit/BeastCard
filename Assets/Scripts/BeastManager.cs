using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BeastManager : MonoBehaviour
{

    #region static properties

    public static int NumPlayers { get; set; }

    #endregion

    #region Other components

    [SerializeField]                        //Questa riga viene messa prima di un campo privato in modo da poter essere visualizzata all'esterno su unity
    private Players_Points_VIsualized playersPoints;

    [SerializeField]
    private PlayersLoader playersLoader;

    [SerializeField]
    private ScoreBoardController scoreBoard;

    [SerializeField]
    private TableOrg tableOrganizer;

    #endregion

    #region private fields

    private int countDownCard = 0;

    [SerializeField]
    private List<Player> players = new List<Player>();

    [SerializeField]
    private Queue<Player> playersQueue;

    #endregion

    #region Properties

    public int IndexStartPlayer { get; private set; }

    #endregion




    //Metodo chiamato da unity appena viene caricato l'oggetto nella scena prima del suo primo frame
    //Di norma ritornerebbe un void, in questo caso pero' ritorna un IEnumerator che
    //mi serve semplicemente per gestire le operazioni con dei ritardi,
    //un po' come i Thread.Sleep ma gestiti in modo piu' semplice
    IEnumerator Start()
    {
        SetupGame();
        
        #region Players instantiation

        players = new List<Player>
        {
            playersLoader.LoadPlayer()
            
        };
        players[0].isPlayer = true;

        yield return new WaitForSeconds(1f); //Appena il flusso del codice arriva a questa riga si interrompe per tot secondi e poi prosegue
        for (int i = 1; i < playersLoader.GetSitsPLaces(); i++)
        {
            players.Add(playersLoader.LoadBotPlayer(i));
            yield return new WaitForSeconds(1f);
        }

        #endregion

        //Setup del tavolo
        tableOrganizer.ShuffleDeck();
        tableOrganizer.TakeFirstCardForBriscola();

        ConstructPlayerQueue();
        yield return new WaitForSeconds(0.5f);

        //Da le carte ai giocatori
        for (int i = 0; i < 3; i++)
        {
            foreach (var item in players)
            {
                SOB_Card cardExtract = null;
                cardExtract = tableOrganizer.TakeCard(item.HandTrasform.position, () => { item.SetCard(cardExtract); });
                yield return new WaitForSeconds(0.5f);
                
            }
        }
        //Inizio del turno
        StartTurn();

    }

    private void SetupGame()
    {
        if(NumPlayers == 2)
        {
            playersLoader.SetupForTwoPlayers();
        }
        IndexStartPlayer = -1;
    }

    private void StartTurn()
    {
        //Prende dalla coda creata un giocatore per fargli iniziare il suo turno
        Player player = playersQueue.Dequeue();
        if (IndexStartPlayer == -1)
        {
            IndexStartPlayer = players.IndexOf(player);
        }
        StartCoroutine(player.OnTurn());
    }

    public void CardDropped(SOB_Card card, Player playerWhoDropped)
    {
        int indexPos = players.IndexOf(playerWhoDropped);
        
        if (indexPos != -1)
        {
            tableOrganizer.DropCardOnTable(indexPos, card);
            StartCoroutine(EndTurn());
        }

    }


    private IEnumerator EndTurn()
    {
        
        if (playersQueue.Count > 0)
        {
            
            StartTurn();
            yield return new WaitForSeconds(1f);
        }
        else
        {
            TableOrg.Win winInfo = tableOrganizer.CalculateVincitaRound(IndexStartPlayer, players.ToArray());
            yield return new WaitForSeconds(1f);
            StartCoroutine( winInfo.player.WinRoundAnimation());
            StartCoroutine(tableOrganizer.DoWinRoundAnimation(winInfo.player.HandTrasform.position));
            winInfo.player.AddCardsToDeck(winInfo.cards);
            if (players.IndexOf(winInfo.player) == 0)
                playersPoints.SetPlayerPoints(winInfo.player);

            yield return new WaitForSeconds(2f);
            
            

            

            
            
            if (tableOrganizer.TableDeck.Count > 0)
            {
                StartCoroutine(StartNewRound(winInfo.player));
            }
            else
            {
                countDownCard++;
                if(countDownCard == 3)
                {
                    foreach (var player in players)
                    {
                        player.OnGameEnd();
                    }
                    StartCoroutine(CalculateWinGamePlayer());
                }
                else
                {
                    StartCoroutine(StartNewRound(winInfo.player));
                }
            }
            
        }
        
        

    }

    private IEnumerator CalculateWinGamePlayer()
    {
        int pointsMax = 0;
        foreach (var player in players)
        {
            int pointsPLayer = player.GetPoints();
            if(pointsPLayer > pointsMax)
            {
                pointsMax = pointsPLayer;
            }
        }
        scoreBoard.StartScoreBoard(players);
        yield return new WaitForSeconds(3f);
        foreach (var player in players)
        {
            if(player.GetPoints() == pointsMax)
            {

                player.DoWin();
            }
        }



    }

    private IEnumerator StartNewRound(Player player)
    {
        IndexStartPlayer = -1;
        
        
        ConstructPlayerQueue(players.IndexOf(player));
        Queue<Player> playersQueue = new Queue<Player>();
        foreach (var playerQ in this.playersQueue)
        {
            playersQueue.Enqueue(playerQ);
        }

        yield return new WaitForSeconds(1f);
        if(tableOrganizer.TableDeck.Count > 0)
            StartCoroutine(GetPlayersCards(playersQueue));
        yield return new WaitForSeconds(1.2f);

        StartTurn();
    }

    private IEnumerator GetPlayersCards(Queue<Player> playersQueue)
    {
        for (int i = 0; i < players.Count; i++)
        {
            Player current = playersQueue.Dequeue();
            SOB_Card cardExtract = null;
            cardExtract = tableOrganizer.TakeCard(current.HandTrasform.position, () => {current.AddCardToHand(cardExtract); });
            
            yield return new WaitForSeconds(0.5f);
            
        }
    }

    private void ConstructPlayerQueue(int index = 0)
    {
        playersQueue = new Queue<Player>();
        for (int i = 0, indexPlayer = index; i < players.Count; i++, indexPlayer++) 
        {
            if(indexPlayer >= players.Count)
            {
                indexPlayer = 0;
            }
            playersQueue.Enqueue(players[indexPlayer]);
        }
    }

    

    public void OnRestartClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnMenuClicked()
    {
        SceneManager.LoadScene(0);
    }

    

    public int GetPlayerIndex(Player player)
    {
        return players.Count - (playersQueue.Count + 1);
    }


    public int GetPlayersCountTotal()
    {
        return players.Count;
    }

    



    

    public void OnOpenSettings()
    {
        Camera.main.GetComponent<RotateCamera>().SetOnCursorLockState();
        Camera.main.GetComponent<RotateCamera>().enabled = true;
    }
    public void OnCloseSettings()
    {
        
        Camera.main.GetComponent<RotateCamera>().SetOffCursoreLockState();
        Camera.main.GetComponent<RotateCamera>().enabled = false;
    }

}
