using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class JsonRankState
{
    public List<Rank> Ranks = new List<Rank>();

    [Serializable]
    public struct Rank
    {
        public string nickname;
        public uint score;
    }

    public void Sort()
    {
        this.Ranks.Sort(delegate (JsonRankState.Rank A, JsonRankState.Rank B)
        {
            if (A.score < B.score) return 1;
            else if (A.score > B.score) return -1;
            else return 0;
        });
    }

    public void RankUpdate(Rank compareRank)
    {
        if(this.Ranks.Count >= 100)
        {

            JsonRankState.Rank lastRank = this.Ranks[this.Ranks.Count - 1];
            if (compareRank.score < lastRank.score) return;
            
            int i = 0;
            bool hasUser = false;

            foreach (JsonRankState.Rank rank in this.Ranks)
            {
                if (rank.nickname == compareRank.nickname)
                {
                    hasUser = true;
                    if (rank.score < compareRank.score)
                    {
                        this.Ranks.RemoveAt(i);
                        this.Ranks.Add(compareRank);
                        this.Sort();
                        break;
                    }
                }
                i++;
            }

            if (!hasUser)
            {
                this.Ranks.RemoveAt(this.Ranks.Count - 1);
                this.Ranks.Add(compareRank);
                this.Sort();
            }

        }
        else
        {
            int i = 0;
            bool hasUser = false;

            foreach (JsonRankState.Rank rank in this.Ranks)
            {
                if(rank.nickname == compareRank.nickname)
                {
                    hasUser = true;
                    if (rank.score < compareRank.score)
                    {
                        this.Ranks.RemoveAt(i);
                        this.Ranks.Add(compareRank);
                        this.Sort();
                        break;
                    }
                }
                i++;
            }

            if (!hasUser)
            {
                this.Ranks.Add(compareRank);
                this.Sort();
            }
        }
    }
}
