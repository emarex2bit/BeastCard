using System.Collections.Generic;
using UnityEngine;

public class PlayersLoader : MonoBehaviour
{
    [SerializeField]
    private BeastManager manager;

    [SerializeField]
    private List<GameObject> sitsPositions;

    [SerializeField]
    private GameObject player_prefab;

    [SerializeField]
    private GameObject bot_prefab;

    [SerializeField]
    private RuntimeAnimatorController animatorControllerPlayers;

    [SerializeField]
    private GameObject[] charactersPrefabs;

    [SerializeField]
    private TableOrg tableOrganizer;


    //Carica il modello 3d del giocatore in maniera randomica, ed i settaggi base che deve avere il player
    public Player LoadPlayer()
    {
        
        GameObject playerObj = Instantiate(player_prefab, sitsPositions[0].transform.position, sitsPositions[0].transform.rotation);
        Player player = playerObj.GetComponent<Player>();
        playerObj.name = "Player";

        //Faccio questa cosa perche' c'e' un problema con la telecamera con questi 3 prefabs,
        //praticamente il cappello del modello sta in mezzo alla visuale,
        //non ho altri modi per mettere a posto
        int indexPrefab = Random.Range(0, charactersPrefabs.Length);
        while(indexPrefab == 36 || indexPrefab == 37 || indexPrefab == 38)
        {
            indexPrefab = Random.Range(0, charactersPrefabs.Length);
        }
        GameObject visual = Instantiate(charactersPrefabs[indexPrefab], sitsPositions[0].transform.position, sitsPositions[0].transform.rotation, player.transform);
        visual.name = "Visual";


        visual.GetComponent<Animator>().runtimeAnimatorController = animatorControllerPlayers;
        player.SetAnimator(visual.GetComponent<Animator>());
        player.SetManager(manager);
        player.SetTableOrganizer(tableOrganizer);
        return player;
    }

    //Carica il modello 3d del giocatore Bot in maniera randomica, ed i settaggi base che deve avere il player_Bot
    public Player_Bot LoadBotPlayer(int indexPos)
    {
        GameObject botObj = Instantiate(bot_prefab, sitsPositions[indexPos].transform.position, sitsPositions[indexPos].transform.rotation);
        Player_Bot player_Bot = botObj.GetComponent<Player_Bot>();
        player_Bot.InitializeBot(Player_Bot.Difficulty.NORMAL);
        botObj.name = $"Bot_{indexPos}";
        player_Bot.SetManager(manager);
        GameObject visual_Bot = Instantiate(charactersPrefabs[Random.Range(0, charactersPrefabs.Length)], sitsPositions[indexPos].transform.position, sitsPositions[indexPos].transform.rotation, player_Bot.transform);
        visual_Bot.GetComponent<Animator>().runtimeAnimatorController = animatorControllerPlayers;
        visual_Bot.name = "Visual";
        player_Bot.SetAnimator(visual_Bot.GetComponent<Animator>());
        player_Bot.SetTableOrganizer(tableOrganizer);
        return player_Bot;
    } 

    public int GetSitsPLaces()
    {
        return sitsPositions.Count;
    }

    //Questo metodo mi serve per risistemare il sistema in caso la partita sia solo di 2 giocatori
    public void SetupForTwoPlayers()
    {
        tableOrganizer.SetUpForTwo();
        sitsPositions.RemoveAt(3);
        sitsPositions.RemoveAt(1);
        
    }

}
