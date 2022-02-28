using Combat;
using Mirror;
using System;
using UnityEngine;

namespace Units
{
    [RequireComponent(typeof(UnitMovement))]
    [RequireComponent(typeof(Targeter))]
    [RequireComponent(typeof(Damageable))]
    public class Unit : NetworkBehaviour, IDestructible
    {
        [SerializeField]
        private GameObject selectionUI;

        private UnitMovement unitMovement;
        private Targeter unitTargeter;
        private Damageable damageable;

        public UnitMovement GetUnitMovement { get => unitMovement; }
        public Targeter GetUnitTargeter { get => unitTargeter; }

        public static event Action<Unit> OnServerUnitSpawned;
        public static event Action<Unit> OnServerUnitDrop;
        public static event Action<Unit> OnAuthorityUnitSpawned;
        public static event Action<Unit> OnAuthorityUnitDrop;

        private void Awake()
        {
            unitMovement = GetComponent<UnitMovement>();
            unitTargeter = GetComponent<Targeter>();
            damageable = GetComponent<Damageable>();
        }

        #region Server
        public override void OnStartServer()
        {
            OnServerUnitSpawned?.Invoke(this);
            damageable.OnServerDestruct += HandleDestruction;
        }

        public override void OnStopServer()
        {
            OnServerUnitDrop?.Invoke(this);
            damageable.OnServerDestruct -= HandleDestruction;
        }

        [Server]
        public void HandleDestruction()
        {
            NetworkServer.Destroy(gameObject);
        }

        #endregion

        #region Client

        public override void OnStartAuthority()
        {
            OnAuthorityUnitSpawned?.Invoke(this);
        }

        public override void OnStopClient()
        {
            if (hasAuthority)
                OnAuthorityUnitDrop?.Invoke(this);
        }

        [Client]
        public void Select()
        {
            if (!hasAuthority) return;
            selectionUI.SetActive(true);
        }

        [Client]
        public void Deselect()
        {
            if (!hasAuthority) return;
            selectionUI.SetActive(false);
        }
        #endregion
    }
}