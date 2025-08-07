using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FishController : MonoBehaviour
{
    public FishData data;
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
        rb.AddForce(transform.forward * data.acceleration);
        rb.velocity = new Vector3(
            Mathf.Clamp(rb.velocity.x, -data.maxSpeed, data.maxSpeed),
            Mathf.Clamp(rb.velocity.y, -data.maxSpeed, data.maxSpeed),
            Mathf.Clamp(rb.velocity.z, -data.maxSpeed, data.maxSpeed));
        if ((Time.time > lastTimeWaypointSelected + data.waypointSelectionDelay) ||
            Vector3.Distance(transform.position, currentWayPoint) < data.attackDistance) 
            SelectWayPoint();
    }

    private void Rotate()
    {
        Quaternion targetRotation = Quaternion.LookRotation(currentWayPoint - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, data.rotationSpeed * Time.deltaTime);
    }

    private void SelectWayPoint()
    {
        currentWayPoint = transform.position + new Vector3(
        /*X = */ Random.Range(-data.wayPointsRange, data.wayPointsRange),
        /*Y = */ Random.Range(-data.wayPointsRange, data.wayPointsRange),
        /*Z = */ Random.Range(-data.wayPointsRange, data.wayPointsRange));

        lastTimeWaypointSelected = Time.time;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(currentWayPoint, 1);
    }
}
