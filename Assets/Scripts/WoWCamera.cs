using UnityEngine;

public class WoWCamera : MonoBehaviour
{

    public Transform target;
    public float targetHeight = 1.7f;
    public float distance = 5.0f;
    public float offsetFromWall = 0.1f;
    public float maxDistance = 20;
    public float minDistance = .6f;
    public float xSpeed = 200.0f;
    public float ySpeed = 200.0f;
    public float targetSpeed = 5.0f;
    public int yMinLimit = -80;
    public int yMaxLimit = 80;
    public int zoomRate = 40;
    public float rotationDampening = 3.0f;
    public float zoomDampening = 5.0f;
    public LayerMask collisionLayers = -1;
    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private float correctedDistance;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        xDeg = angles.x;
        yDeg = angles.y;
        currentDistance = distance;
        desiredDistance = distance;
        correctedDistance = distance;
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;
    }

    void Update()
    {
        if (Input.GetMouseButton(1) && Input.GetMouseButton(0))
        {
            float targetRotationAngle = target.eulerAngles.y;
            float currentRotationAngle = transform.eulerAngles.y;
            xDeg = Mathf.LerpAngle(currentRotationAngle, targetRotationAngle, rotationDampening * Time.deltaTime);
            target.transform.Rotate(0, Input.GetAxis("Mouse X") * xSpeed * 0.02f, 0);
            xDeg += Input.GetAxis("Mouse X") * targetSpeed * 0.02f;
            target.transform.Translate(Vector3.forward * targetSpeed * Time.deltaTime);
        }
    }

    void LateUpdate()
    {
        Vector3 vTargetOffset;
        if (!target)
            return;
        if (Input.GetMouseButton(0))
        {
            xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
        }
        else if (Input.GetMouseButton(1))
        {
            float targetRotationAngle = target.eulerAngles.y;
            float currentRotationAngle = transform.eulerAngles.y;
            xDeg = Mathf.LerpAngle(currentRotationAngle, targetRotationAngle, rotationDampening * Time.deltaTime);
            target.transform.Rotate(0, Input.GetAxis("Mouse X") * xSpeed * 0.02f, 0);
            xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
        }
        else if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            float targetRotationAngle = target.eulerAngles.y;
            float currentRotationAngle = transform.eulerAngles.y;
            xDeg = Mathf.LerpAngle(currentRotationAngle, targetRotationAngle, rotationDampening * Time.deltaTime);
        }
        yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
        Quaternion rotation = Quaternion.Euler(yDeg, xDeg, 0);
        desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
        desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        correctedDistance = desiredDistance;
        vTargetOffset = new Vector3(0, -targetHeight, 0);
        Vector3 position = target.position - (rotation * Vector3.forward * desiredDistance + vTargetOffset);
        RaycastHit collisionHit;
        Vector3 trueTargetPosition = new Vector3(target.position.x, target.position.y + targetHeight, target.position.z);
        bool isCorrected = false;
        if (Physics.Linecast(trueTargetPosition, position, out collisionHit, collisionLayers.value))
        {
            correctedDistance = Vector3.Distance(trueTargetPosition, collisionHit.point) - offsetFromWall;
            isCorrected = true;
        }
        currentDistance = !isCorrected || correctedDistance > currentDistance ? Mathf.Lerp(currentDistance, correctedDistance, Time.deltaTime * zoomDampening) : correctedDistance;
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
        position = target.position - (rotation * Vector3.forward * currentDistance + vTargetOffset);
        transform.rotation = rotation;
        transform.position = position;
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}