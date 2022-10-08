using UnityEngine;

namespace Data.Config
{
    [CreateAssetMenu(fileName = "PlayersConfig", menuName = "ScriptableObjects/PlayersConfig", order = 1)]
    public class PlayersConfig : ScriptableObject
    {
        [SerializeField] private ColorId[] playersColors;
        public ColorId[] PlayersColors => playersColors;
    }
}