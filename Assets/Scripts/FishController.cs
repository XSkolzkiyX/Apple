using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FishController : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private float acceleration = 20;
    [SerializeField] private float maxSpeed = 3;
    [SerializeField] private float rotationSpeed = 2;
    [SerializeField] private float attackDistance = 2;
    [Space(10)]

    [Header("Waypoints")]
    [SerializeField] private float wayPointsRange = 25;
    [SerializeField] private float waypointSelectionDelay = 4;
    private float lastTimeWaypointSelected;
    private Vector3 currentWayPoint;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        SelectWayPoint();
    }

    private void FixedUpdate()
    {
        Rotate();
        rb.AddForce(transform.forward * acceleration);
        rb.velocity = new Vector3(
            Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed),
            Mathf.Clamp(rb.velocity.y, -maxSpeed, maxSpeed),
            Mathf.Clamp(rb.velocity.z, -maxSpeed, maxSpeed));
        if ((Time.time > lastTimeWaypointSelected + waypointSelectionDelay) ||
            Vector3.Distance(transform.position, currentWayPoint) < attackDistance) 
            SelectWayPoint();
    }

    private void Rotate()
    {
        Quaternion targetRotation = Quaternion.LookRotation(currentWayPoint - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void SelectWayPoint()
    {
        currentWayPoint = transform.position + new Vector3(
        /*X = */ Random.Range(-wayPointsRange, wayPointsRange),
        /*Y = */ Random.Range(-wayPointsRange, wayPointsRange),
        /*Z = */ Random.Range(-wayPointsRange, wayPointsRange));

        lastTimeWaypointSelected = Time.time;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(currentWayPoint, 1);
    }
}
