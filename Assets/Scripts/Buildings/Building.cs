using Combat;
using Mirror;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace Buildings
{
    [RequireComponent(typeof(NavMeshObstacle))]
    [RequireComponent(typeof(Damageable))]
    [RequireComponent(typeof(Targetable))]
    public class Building : NetworkBehaviour
    {
        [SerializeField] private Sprite icon;
        [SerializeField] private int id = -1;
        [SerializeField] private int price;
        protected Damageable damageable;

        public static event Action<Building> OnServerBuildingSpawned;
        public static event Action<Building> OnServerBuildingDrop;
        public static event Action<Building> OnAuthorityBuildingSpawned;
        public static event Action<Building> OnAuthorityBuildingDrop;

        public Sprite Icon { get => icon; }
        public int Id { get => id;  }
        public int Price { get => price; }

        private void Awake()
        {
            damageable = GetComponent<Damageable>();
        }

        #region Server
        public override void OnStartServer()
        {
            OnServerBuildingSpawned?.Invoke(this);
        }

        public override void OnStopServer()
        {
            OnServerBuildingDrop?.Invoke(this);
        }

        #endregion

        #region Client

        public override void OnStartAuthority()
        {
            OnAuthorityBuildingSpawned?.Invoke(this);
        }

        public override void OnStopClient()
        {
            if (hasAuthority)
                OnAuthorityBuildingDrop?.Invoke(this);
        }

        #endregion
    }
}