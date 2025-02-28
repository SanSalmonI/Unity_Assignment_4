using UnityEngine;


public class CompassPointer : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The arrow object that should rotate away from the waypoint.")]
    public Transform arrow;

    [Tooltip("The waypoint or target the arrow should face away from.")]
    public Transform waypoint;

    void Update()
    {
        if (arrow == null || waypoint == null)
            return;

       
        Vector3 direction = arrow.position - waypoint.position;

        
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            arrow.rotation = targetRotation;
        }
    }
}
