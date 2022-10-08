using System.Collections.Generic;
using Buildings;
using UnityEngine;

namespace Data.Config
{
    [CreateAssetMenu(fileName = "BuildingsConfig", menuName = "ScriptableObjects/BuildingsConfig", order = 1)]
    public class BuildingsConfig : ScriptableObject
    {
        [SerializeField] private List<Building> buildings;
        [SerializeField] private LayerMask buildingLockedLayers;
        [SerializeField] private float buildRange;
        public List<Building> Buildings { get => buildings; }
        public LayerMask BuildingLockedLayers { get => buildingLockedLayers; }
        public float BuildRange { get => buildRange; }
    }
}