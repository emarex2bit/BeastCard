using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    private Button firstModeButton;
    private Button secondModeButton;
    private Button exitButton;
    private Button charactersButton;

    private LoaderType loaderType;
    // Start is called before the first frame update
    private void Awake()
    {
        loaderType = gameObject.GetComponent<LoaderType>();
    }
    void Start()
    {
        
        var root = GetComponent<UIDocument>().rootVisualElement;

        firstModeButton = root.Q<Button>("1v1-Button");
        secondModeButton = root.Q<Button>("4-Button");
        exitButton = root.Q<Button>("Exit-Button");
        charactersButton = root.Q<Button>("Character-Button");

        firstModeButton.clicked += FirstModeClicked;
        secondModeButton.clicked += SecondModeClicked;
        exitButton.clicked += ExitButton_clicked;
        //charactersButton.clicked += CharactersButton_clicked;
    }

    private void CharactersButton_clicked()
    {
        
    }

    private void ExitButton_clicked()
    {
        Application.Quit();
    }

    private void FirstModeClicked()
    {
        loaderType.LoadLobby(Mode.two);
    }
    private void SecondModeClicked()
    {
        loaderType.LoadLobby(Mode.four);
    }
}
