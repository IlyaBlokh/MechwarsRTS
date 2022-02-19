using Mirror;
using UnityEngine;

namespace Units
{
    [RequireComponent(typeof(UnitMovement))]
    public class Unit : NetworkBehaviour
    {
        [SerializeField]
        private GameObject _selectionUI;

        private UnitMovement _unitMovement;

        public UnitMovement GetUnitMovement { get => _unitMovement; }

        private void Awake()
        {
            _unitMovement = GetComponent<UnitMovement>();
        }

        #region Client
        [Client]
        public void Select()
        {
            if (!hasAuthority) return;
            _selectionUI.SetActive(true);
        }

        [Client]
        public void Deselect()
        {
            if (!hasAuthority) return;
            _selectionUI.SetActive(false);
        }
        #endregion
    }
}