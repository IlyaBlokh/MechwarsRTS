using Mirror;
using System;
using UnityEngine;

namespace Units
{
    [RequireComponent(typeof(UnitMovement))]
    public class Unit : NetworkBehaviour
    {
        [SerializeField]
        private GameObject selectionUI;

        private UnitMovement unitMovement;

        public UnitMovement GetUnitMovement { get => unitMovement; }

        public static event Action<Unit> OnServerUnitSpawned;
        public static event Action<Unit> OnServerUnitDrop;
        public static event Action<Unit> OnAuthorityUnitSpawned;
        public static event Action<Unit> OnAuthorityUnitDrop;

        private void Awake()
        {
            unitMovement = GetComponent<UnitMovement>();
        }

        #region Server
        public override void OnStartServer()
        {
            base.OnStartServer();
            OnServerUnitSpawned?.Invoke(this);
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            OnServerUnitDrop?.Invoke(this);
        }

        #endregion

        #region Client

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (hasAuthority && isClientOnly)
                OnAuthorityUnitSpawned?.Invoke(this);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            if (hasAuthority && isClientOnly)
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