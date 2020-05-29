using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using PlayFab.Json;

public class PlayFabStatsDataManager : Singleton<PlayFabStatsDataManager>
{
    [SerializeField] private GameObject leadeboardPanel;
    [SerializeField] private GameObject leadeboardMemberPrefab;
    [SerializeField] private Transform membersGroup;
    private int playerScore;
    private GameManager m_GameManager;
    private GameManager GM => m_GameManager != null ?
    m_GameManager : m_GameManager = GameManager.Instance;

    public void GetPlayerScoreFromCloud()
    {
        GetStats();
    }

    public void SetAndUpdateStats(int value)
    {
        playerScore = value;
        StartCloudUpdatePlayerStats();
    }

    public void OnGetLeaderboards()
    {
        var reqLeaderboard = new GetLeaderboardRequest { StartPosition = 0, StatisticName = "PlayerScore", MaxResultsCount = 10 };
        PlayFabClientAPI.GetLeaderboard(reqLeaderboard, GetLeaderboard, ErrorLeaderboard);
    }

    private void GetLeaderboard(GetLeaderboardResult result)
    {
        leadeboardPanel.SetActive(true);
        foreach (var player in result.Leaderboard)
        {
            var tempMember = Instantiate(leadeboardMemberPrefab, membersGroup);
            var lmemb = tempMember.GetComponent<LeaderboardMember>();
            lmemb.PlayerNameText.text = player.DisplayName;
            lmemb.PlayerScore.text = player.StatValue.ToString();
        }
    }

    private void ErrorLeaderboard(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }

    public void OnCloseLeaderboard()
    {
        leadeboardPanel.SetActive(false);
        for (int i = membersGroup.childCount - 1; i >= 0; i--)
        {
            Destroy(membersGroup.GetChild(i).gameObject);
        }
    }

    /* Unsafe method to send stat
    private void SetStats()
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            // request.Statistics is a list, so multiple StatisticUpdate objects can be defined if required.
            Statistics = new List<StatisticUpdate> {
                new StatisticUpdate { StatisticName = "PlayerScore", Value = playerScore }
            }
        },
        result => { Debug.Log("User statistics updated"); },
        error => { Debug.LogError(error.GenerateErrorReport()); });
    } */

    private void GetStats()
    {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            OnGetStats,
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

    private void OnGetStats(GetPlayerStatisticsResult result)
    {
        Debug.Log("Received the following Statistics:");
        foreach (var eachStat in result.Statistics)  //result.Statistics - list of all stats elements
        {
            Debug.Log($"Statistic ( {eachStat.StatisticName} ): {eachStat.Value})");
            switch (eachStat.StatisticName)
            {
                case "PlayerScore":
                    playerScore = eachStat.Value;
                    GM.SetScore(playerScore);
                    break;
            }
        }
    }

    // Build the request object and access the API
    private void StartCloudUpdatePlayerStats()
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "UpdatePlayerStats", // Arbitrary function name (must exist in your uploaded cloud.js file)
            FunctionParameter = new { PlayerScore = playerScore }, // The parameter provided to your function
            GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
        }, OnCloudUpdateStats, OnErrorShared);
    }

    private static void OnCloudUpdateStats(ExecuteCloudScriptResult result)
    {
        // Cloud Script returns arbitrary results, so you have to evaluate them one step and one parameter at a time
        Debug.Log(PlayFab.PfEditor.Json.JsonWrapper.SerializeObject(result.FunctionResult));
        JsonObject jsonResult = (JsonObject)result.FunctionResult;
        object messageValue;
        jsonResult.TryGetValue("messageValue", out messageValue); // note how "messageValue" directly corresponds to the JSON values set in Cloud Script
        Debug.Log((string)messageValue);
    }

    private static void OnErrorShared(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }
}
