using Combat;
using Mirror;
using System;
using UnityEngine;
using UnityEngine.AI;
using Utils;

namespace Buildings
{
    [RequireComponent(typeof(NavMeshObstacle))]
    [RequireComponent(typeof(Damageable))]
    [RequireComponent(typeof(Targetable))]
    [RequireComponent(typeof(TeamColorSetter))]
    public class Building : NetworkBehaviour
    {
        [SerializeField] private Sprite icon;
        [SerializeField] private int id = -1;
        [SerializeField] private int price;
        [SerializeField] private GameObject previewGameobject;
        protected Damageable damageable;

        public static event Action<Building> OnServerBuildingSpawned;
        public static event Action<Building> OnServerBuildingDrop;
        public static event Action<Building> OnAuthorityBuildingSpawned;
        public static event Action<Building> OnAuthorityBuildingDrop;

        public Sprite Icon => icon;
        public int Id => id;
        public int Price => price;
        public GameObject PreviewGameobject => previewGameobject;

        private void Awake() => 
            damageable = GetComponent<Damageable>();

        #region Server
        public override void OnStartServer() => 
            OnServerBuildingSpawned?.Invoke(this);

        public override void OnStopServer() => 
            OnServerBuildingDrop?.Invoke(this);

        #endregion

        #region Client

        public override void OnStartAuthority() => 
            OnAuthorityBuildingSpawned?.Invoke(this);

        public override void OnStopClient()
        {
            if (hasAuthority)
                OnAuthorityBuildingDrop?.Invoke(this);
        }

        #endregion
    }
}