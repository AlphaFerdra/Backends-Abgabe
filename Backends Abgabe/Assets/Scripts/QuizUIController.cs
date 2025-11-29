using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class QuizUIController : MonoBehaviour
{
    public static QuizUIController QZUIInstance;

    [Header("UI References")]
    [SerializeField] private TMP_Text questionText;
    public Button[] answerButtons;

    [Header("Screens")]
    [SerializeField] private GameObject quizScreen;
    [SerializeField] private GameObject resultScreen;
    [SerializeField] private TMP_Text resultText;

    private Question currentQuestion;

    private void Awake()
    {
        QZUIInstance = this;

        for (int i = 0; i < answerButtons.Length; i++)
        {    
            int index = i;
            answerButtons[i].onClick.AddListener(() => SubmitAnswer(index));
        }
    }

    public void DisplayQuestion(Question q)
    {
        currentQuestion = q;

        quizScreen.SetActive(true);
        resultScreen.SetActive(false);

        questionText.text = q.QuestionText;

        answerButtons[0].GetComponentInChildren<TMP_Text>().text = q.Answer1Text;
        answerButtons[1].GetComponentInChildren<TMP_Text>().text = q.Answer2Text;
        answerButtons[2].GetComponentInChildren<TMP_Text>().text = q.Answer3Text;
        answerButtons[3].GetComponentInChildren<TMP_Text>().text = q.Answer4Text;
        
        foreach (Button btn in answerButtons)
            btn.interactable = true;

        ResetButtonColors();
    }


    public void SubmitAnswer(int answerIndex)
    {
        // Send answer to server
        QuizGameController.QZInstance.SubmitAnswerServerRpc(answerIndex, NetworkManager.Singleton.LocalClientId);
        foreach(Button answerButton in answerButtons)
        {
            answerButton.interactable = false;
        }
    }

    public void ShowAnswerResult(int chosenIndex, bool correct)
    {
        ResetButtonColors();

        
        if(currentQuestion.rightAnswer != RightAnswerEnum.Answer5)
        {
            int right = (int)currentQuestion.rightAnswer;
            answerButtons[right].image.color = Color.green;
        }
        

        if (!correct)
        {
            Debug.Log(chosenIndex);
            answerButtons[chosenIndex].image.color = Color.red;
        }
    }


    private void ResetButtonColors()
    {
        foreach (var btn in answerButtons)
            btn.image.color = Color.white;
    }

    public void ShowResultsScreen()
    {
        quizScreen.SetActive(false);
        resultScreen.SetActive(true);

        // Get local player's score
        ulong me = NetworkManager.Singleton.LocalClientId;
        int score = QuizPlayerState.Players[me].correctAnswers.Value;

        resultText.text = "Your Score is: " + score;
    }
}
