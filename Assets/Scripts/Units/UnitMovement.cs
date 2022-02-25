using Combat;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

namespace Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Targeter))]
    public class UnitMovement : NetworkBehaviour
    {
        [SerializeField]
        private float chaseStopDistance;

        private NavMeshAgent navMeshAgent;
        private Targeter targeter;

        #region Server

        [ServerCallback]
        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            targeter = GetComponent<Targeter>();
        }

        [ServerCallback]
        private void Update()
        {
            if (targeter.Target != null)
            {
                if ((transform.position - targeter.Target.transform.position).sqrMagnitude > Mathf.Pow(chaseStopDistance, 2))
                {
                    navMeshAgent.SetDestination(targeter.Target.transform.position);
                }
                else if (navMeshAgent.hasPath)
                {
                    navMeshAgent.ResetPath();
                }
                return;
            }

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