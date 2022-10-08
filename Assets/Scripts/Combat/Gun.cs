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
        private GameObject projectilePrefab;
        [SerializeField]
        private float shotsPerSecond = 2.0f;
        [SerializeField]
        private float fireRange = 10.0f;

        private float lastShootTime;
        private Quaternion projectileRotation;
        private Targetable target;
        public Targetable Target { get => target; set => target = value; }

        #region Server

        [ServerCallback]
        void Update()
        {
            if (CanShoot())
            {
                if (Time.time > lastShootTime + 1.0f / shotsPerSecond)
                {
                    projectileRotation = Quaternion.LookRotation(target.transform.position - projectileSpawnPoint.position);
                    var projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileRotation);
                    NetworkServer.Spawn(projectile, connectionToClient);
                    lastShootTime = Time.time;
                }
            }
        }

        [Server]
        private bool CanShoot()
        {
            if (target == null) { return false; }
            if (Utils.Utils.IsDistanceGreater(projectileSpawnPoint.position, target.GetAimPoint(), fireRange)) { return false; }

            return true;
        }

        #endregion
    }
}