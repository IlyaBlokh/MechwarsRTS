using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public class Gun : NetworkBehaviour
    {
        [SerializeField]
        private Transform projectileSpawnPoint;
        [SerializeField]
        private Transform projectilePrefab;
        [SerializeField]
        private float shotsPerSecond = 2.0f;
        [SerializeField]
        private float fireRange = 10.0f;


        private Targetable target;
        public Targetable Target { get => target; set => target = value; }

        [ServerCallback]
        void Update()
        {

        }
    }
}