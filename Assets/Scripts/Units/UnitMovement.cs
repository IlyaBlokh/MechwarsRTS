using Combat;
using Mirror;
using Networking;
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

        public override void OnStartServer()
        {
            GameLoopController.OnServerGameOver += HandleServerGameOver;
        }

        public override void OnStopServer()
        {
            GameLoopController.OnServerGameOver -= HandleServerGameOver;
        }

        [Server]
        private void HandleServerGameOver()
        {
            navMeshAgent.ResetPath();
        }

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
                if (Utils.Utils.IsDistanceGreater(transform.position, targeter.Target.transform.position, chaseStopDistance))
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

        [Server]
        public void ServerTryMove(Vector3 destination)
        {
            if (!NavMesh.SamplePosition(destination, out NavMeshHit hit, 1f, NavMesh.AllAreas))
                return;
            targeter.ClearTarget();
            navMeshAgent.SetDestination(hit.position);
        }

        [Server]
        public Vector3 ServerFindAvailableDestination(Vector3 origin, float range)
        {
            Vector3 result;
            int maxAttempts = 100;
            for (int i = 0; i< maxAttempts; i++)
            {
                result = origin + Random.insideUnitSphere * range;
                result.y = origin.y;
                if (NavMesh.SamplePosition(result, out NavMeshHit hit, 1f, NavMesh.AllAreas))
                {
                    return result;
                }
            }
            Debug.LogWarning("Couldn't find available destination on navmesh");
            return origin;
        }

        [Command]
        public void CmdTryMove(Vector3 destination)
        {
            ServerTryMove(destination);
        }
        #endregion
    }
}