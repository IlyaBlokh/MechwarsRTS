using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class UnitMovement : NetworkBehaviour
    {
        private NavMeshAgent _navMeshAgent;
        private Camera _mainCamera;

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        #region Server
        [Command]
        private void CmdMovePlayer(Vector3 destination)
        {
            if (!NavMesh.SamplePosition(destination, out NavMeshHit hit, 1f, NavMesh.AllAreas)) return;

            _navMeshAgent.SetDestination(hit.position);
        }
        #endregion

        #region Client

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            if (!hasAuthority) return;

            if (!Mouse.current.rightButton.wasPressedThisFrame) return;

            Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) return;

            CmdMovePlayer(hit.point);
        }

        #endregion
    }
}