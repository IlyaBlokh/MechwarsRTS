using UnityEngine;

namespace Data.Config
{
  [CreateAssetMenu(fileName = "PlayersColors", menuName = "ScriptableObjects/PlayersColors", order = 1)]
  public class PlayersColors : ScriptableObject
  {
    [SerializeField] private TeamColor[] teamColors;
    public TeamColor[] TeamColors => teamColors;
  }
}