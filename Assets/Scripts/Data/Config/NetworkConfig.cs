using UnityEngine;

namespace Data.Config
{
    [CreateAssetMenu(fileName = "NetworkConfig", menuName = "ScriptableObjects/NetworkConfig", order = 1)]
    public class NetworkConfig : ScriptableObject
    {
        [SerializeField] private bool useSteam = false;
        public bool UseSteam { get => useSteam; }
    }
}