using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] private LeaderboardNode nodePrefab;
    [SerializeField] private Color playerNodeColor;

    public void PopulateLeaderboards(List<LeaderboardEntry> entries)
    {
        foreach (Transform item in transform)
        {
            Destroy(item.gameObject);
        }

        entries = entries.OrderByDescending(entry => entry.score).ToList();

        if (entries.Count > 0)
        {
            for (int i = 0; i < 10; i++)
            {
                if (i != 9) entries[i].place = i + 1;
                else
                {
                    if (entries[i].place < 10) entries[i].place = 10;
                }
                var newNode = Instantiate(nodePrefab, transform);
                newNode.SetupNode(entries[i].place.ToString(), entries[i].displayName, entries[i].score.ToString());
                if (entries[i].isPlayer)
                {
                    newNode.ChangeTextColor(playerNodeColor);
                }
                if (i % 2 != 0) newNode.GetComponent<Image>().enabled = false;
            }
        }
    }

}