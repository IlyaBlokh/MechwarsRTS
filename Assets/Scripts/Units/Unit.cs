using Mirror;
using UnityEngine;

namespace Units
{
    public class Unit : NetworkBehaviour
    {
        [SerializeField]
        private GameObject _selectionUI;

        #region Client
        [Client]
        public void OnSelect()
        {
            if (!hasAuthority) return;
            _selectionUI.SetActive(true);
        }

        [Client]
        public void OnDeselect()
        {
            if (!hasAuthority) return;
            _selectionUI.SetActive(false);
        }
        #endregion
    }
}