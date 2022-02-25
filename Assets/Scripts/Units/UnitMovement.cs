using Mirror;
using UnityEngine;
using UnityEngine.AI;

namespace Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class UnitMovement : NetworkBehaviour
    {
        private NavMeshAgent navMeshAgent;

        #region Server

        [ServerCallback]
        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        [ServerCallback]
        private void Update()
        {
            if (!navMeshAgent.hasPath) return;
            if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance) return;
            navMeshAgent.ResetPath();
        }

        [Command]
        public void CmdTryMove(Vector3 destination)
        {
            if (!NavMesh.SamplePosition(destination, out NavMeshHit hit, 1f, NavMesh.AllAreas))
                return;
            navMeshAgent.SetDestination(hit.position);
        }
        #endregion
    }
}