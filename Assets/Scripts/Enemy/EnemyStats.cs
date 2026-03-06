using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyDifficulty
{
    Easy,
    Hard,
    Default
}

public static class EnemyStats
{
    public static readonly Dictionary<EnemyDifficulty, float> SpeedByDifficulty = new Dictionary<EnemyDifficulty, float>
    {
        { EnemyDifficulty.Easy, 3f },
        { EnemyDifficulty.Hard, 7f },
        { EnemyDifficulty.Default, 5f }
    };
}