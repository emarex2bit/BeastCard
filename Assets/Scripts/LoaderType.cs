using UnityEngine;
using UnityEngine.SceneManagement;

public class LoaderType : MonoBehaviour
{

    public void LoadLobby(Mode mode)
    {
        BeastManager.NumPlayers = (int)mode;
        SceneManager.LoadScene("GameScene");
    }
}

public enum Mode
{
    two = 2, 
    four = 4
}
