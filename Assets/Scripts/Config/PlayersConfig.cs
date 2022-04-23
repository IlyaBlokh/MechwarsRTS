using UnityEngine;

namespace Config
{
    [CreateAssetMenu(fileName = "PlayersConfig", menuName = "ScriptableObjects/PlayersConfig", order = 1)]
    public class PlayersConfig : ScriptableObject
    {
        [SerializeField] private Color[] teamColors;

        public Color[] TeamColors { get => teamColors; }
    }
}