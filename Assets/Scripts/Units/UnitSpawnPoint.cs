using UnityEngine;

namespace Units
{
    public class UnitSpawnPoint : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.8f, 0.2f, 0.2f, 0.7f);
            Gizmos.DrawSphere(transform.position, 2f);
        }
    }
}
