using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class QuizPlayerState : NetworkBehaviour
{
    public static Dictionary<ulong, QuizPlayerState> Players = new Dictionary<ulong, QuizPlayerState>();

    public NetworkVariable<int> correctAnswers =
        new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        Players[OwnerClientId] = this;
    }

    public void AddCorrectAnswer()
    {
        correctAnswers.Value++;
    }
}
