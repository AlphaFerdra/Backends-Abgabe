using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class QuizGameController : NetworkBehaviour
{
    public static QuizGameController QZInstance;

    public static QuizUIController QZUIInstance;

    [Header("Questions (Assign in Inspector)")]
    [SerializeField] private List<Question> questionPool;


    private List<Question> remainingQuestions;
    private Question currentQuestion;

    private NetworkVariable<int> currentQuestionIndex =
        new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Awake()
    {
        QZInstance = this;
    }


    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {            
            remainingQuestions = new List<Question>(questionPool);
        }

        currentQuestionIndex.OnValueChanged += OnQuestionIndexChanged;
    }


    private void OnQuestionIndexChanged(int oldValue, int newValue)
    {
        if (newValue == -1) return;

        // Client retrieves question from controller
        currentQuestion = questionPool[newValue];
    }

    // SERVER picks next question and notifies clients
    public void SendNextQuestion() 
    {
        if (!IsServer)
            return;

        if (remainingQuestions.Count == 0)
        {
            ShowResultsToAllClientsClientRpc();
            return;
        }

        int index = Random.Range(0, remainingQuestions.Count);
        Question chosen = remainingQuestions[index];
        remainingQuestions.RemoveAt(index);

        currentQuestion = chosen;

        int mainIndex = questionPool.IndexOf(chosen);
        currentQuestionIndex.Value = mainIndex;

        ShowQuestionClientRpc(mainIndex);
    }


    // Called by clients when they answer
    [ServerRpc(RequireOwnership = false)]
    public void SubmitAnswerServerRpc(int answerIndex, ulong sender)
    {
        QuizPlayerState player = QuizPlayerState.Players[sender];

        bool correct = (answerIndex == (int)currentQuestion.rightAnswer);
        if (correct)
            player.AddCorrectAnswer();

        ShowAnswerFeedbackClientRpc(sender, answerIndex, correct);

        Invoke(nameof(SendNextQuestion), 2.0f);
    }


    public void SendFirstQuestion()
    {
        Debug.Log("In firstQuestionMethod");

        foreach (Button answerbutton in QuizUIController.QZUIInstance.answerButtons)
        {
            answerbutton.interactable = true;
        }
        if (!IsServer)
            return;

        if (remainingQuestions.Count == 0)
        {
            ShowResultsToAllClientsClientRpc();
            return;
        }

        Question chosen = remainingQuestions[0];
        remainingQuestions.RemoveAt(0);

        currentQuestion = chosen;

        currentQuestionIndex.Value = 0;

        ShowQuestionClientRpc(0);
    }

        

    [ClientRpc]
    private void ShowQuestionClientRpc(int questionIndex)
    {
        QuizUIController.QZUIInstance.DisplayQuestion(questionPool[questionIndex]);
        Debug.Log("Question should be on screen");
    }

    [ClientRpc]
    private void ShowAnswerFeedbackClientRpc(ulong playerId, int answerIndex, bool correct)
    {
        QuizUIController.QZUIInstance.ShowAnswerResult(answerIndex, correct);
    }

    [ClientRpc]
    private void ShowResultsToAllClientsClientRpc()
    {
        QuizUIController.QZUIInstance.ShowResultsScreen();
    }
}
