using System;
using UnityEngine;

namespace GameCycle
{
    public class Gamemode : MonoBehaviour
    {
        public static float GeneDropRate { get; set; }
        public static float DifficultyMultiplier { get; private set; }
        
        private static GamemodeType mode = GamemodeType.Default;

        public static GamemodeType Mode
        {
            get => mode;
            set
            {
                switch (mode)
                {
                    case GamemodeType.Easy:
                        DifficultyMultiplier = 0.75f;
                        GeneDropRate = 0.7f;
                        break;
                    case GamemodeType.Default:
                        DifficultyMultiplier = 1f;
                        GeneDropRate = 0.85f;
                        break;
                    case GamemodeType.Hardcore:
                        DifficultyMultiplier = 1.5f;
                        GeneDropRate = 1f;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                mode = value;
            }
        }

        private void Awake()
        {
            Mode = GamemodeType.Default;
        }
    }

    public enum GamemodeType
    {
        Easy = 0,
        Default = 1,
        Hardcore = 2,
    }
}