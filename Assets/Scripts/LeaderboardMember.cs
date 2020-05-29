using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardMember : MonoBehaviour
{
    [SerializeField] private Text playerNameText;
    [SerializeField] private Text playerScore;

    public Text PlayerNameText => playerNameText;
    public Text PlayerScore => playerScore;
}
