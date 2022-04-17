﻿using Buildings;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(fileName = "BuildingsConfig", menuName = "ScriptableObjects/BuildingsConfig", order = 1)]
    public class BuildingsConfig : ScriptableObject
    {
        [SerializeField] private List<Building> buildings;
        public List<Building> Buildings { get => buildings; }
    }
}