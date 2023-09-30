using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Linq;

public class LeaderboardEntry
{
    public string id;
    public int place;
    public string displayName;
    public int score;
    public bool isPlayer = false;

    public LeaderboardEntry(string id, int place, string displayName, int score)
    {
        this.id = id;
        this.place = place;
        this.displayName = displayName;
        this.score = score;
    }
}
public class PlayFabManager : MonoBehaviour
{
    [SerializeField] private LeaderboardManager leaderboard;
    [SerializeField] private NameSubmitWindow nameSubmitWindow;

    private string playersID;
    private int lastUserScore;

    private void Start()
    {
        Login();
    }
    public void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnError);
    }

    public void SendLeaderboard(int score)
    {
        lastUserScore = score;
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "Score",
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnError);
    }

    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "Score",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnError);
    }

    public void GetPlayerLeaderboardPosition()
    {
        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = "Score",
            MaxResultsCount = 1,

        };
        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, OnPlayerLeaderboardGet, OnError);
    }

    public void SubmitName(string name)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = name,
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, OnError);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        playersID = result.PlayFabId;

        string name = null;
        if (result.InfoResultPayload.PlayerProfile != null)
            name = result.InfoResultPayload.PlayerProfile.DisplayName;
        else nameSubmitWindow.gameObject.SetActive(true);
    }

    private void OnError(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }

    private void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
    {
        GetLeaderboard();
    }

    private List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
    private void OnLeaderboardGet(GetLeaderboardResult result)
    {
        bool foundPlayer = false;

        entries.Clear();

        for (int i = 0; i < 10; i++)
        {
            if (i + 1 <= result.Leaderboard.Count)
            {
                var item = result.Leaderboard[i];
                LeaderboardEntry entry = new LeaderboardEntry(item.PlayFabId, item.Position + 1, item.DisplayName, item.StatValue);
                if (item.PlayFabId == playersID)
                {
                    foundPlayer = true;
                    entry.isPlayer = true;
                    if (entry.score < lastUserScore) entry.score = lastUserScore;
                }
                entries.Add(entry);
            }
            else
            {
                LeaderboardEntry entry = new LeaderboardEntry("-1", i + 1, "Empty", 0);
                entries.Add(entry);
            }
        }
        if (foundPlayer)
        {
            leaderboard.PopulateLeaderboards(entries);
        }
        else
        {
            GetPlayerLeaderboardPosition();
        }
    }

    private void OnPlayerLeaderboardGet(GetLeaderboardAroundPlayerResult result)
    {
        var playerEntry = result.Leaderboard[0];
        LeaderboardEntry entry = new LeaderboardEntry(playerEntry.PlayFabId, playerEntry.Position + 1, playerEntry.DisplayName, playerEntry.StatValue);
        if (entry.score < lastUserScore) entry.score = lastUserScore;
        entry.isPlayer = true;
        bool inserted = false;
        for (int i = 0; i < 9; i++)
        {
            var item = entries[i];
            if (entry.place == item.place)
            {
                entries[i] = entry;
                inserted = true;
            }
        }
        if (!inserted) entries[9] = entry;

        leaderboard.PopulateLeaderboards(entries);
    }

    private void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
    {

    }
}