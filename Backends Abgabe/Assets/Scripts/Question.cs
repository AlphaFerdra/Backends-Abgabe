
 using UnityEngine;

[CreateAssetMenu(fileName = "Question", menuName = "Scriptable Objects/Question")]
public class Question : ScriptableObject
{
    public string QuestionText;

    public string Answer1Text;
    public string Answer2Text;
    public string Answer3Text;
    public string Answer4Text;

    public RightAnswerEnum rightAnswer;
}
 
