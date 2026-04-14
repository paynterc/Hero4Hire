using UnityEngine;

public class CameraFollowSweep : MonoBehaviour
{
    [Header("Follow")]
    public Transform target;
    public Vector3 offset    = new Vector3(0f, 12f, -8f);
    public float followSpeed = 10f;

    [Header("Occlusion Sweep")]
    [Tooltip("Layers treated as obstacles when checking line of sight to the player")]
    public LayerMask occlusionLayers;
    [Tooltip("Maximum sweep speed in degrees per second")]
    public float sweepSpeed        = 90f;
    [Tooltip("How quickly the sweep accelerates and decelerates (degrees/sec²)")]
    public float sweepAcceleration = 180f;

    // ── Runtime state ─────────────────────────────────────────────────────

    private float orbitAngle    = 0f;   // current horizontal rotation around player
    private float sweepVelocity = 0f;   // degrees/sec, signed
    private int   sweepDir      = 1;    // +1 = right, -1 = left
    private bool  wasOccluded   = false;

    // ── Update ────────────────────────────────────────────────────────────

    void LateUpdate()
    {
        if (target == null) return;

        bool occluded = IsOccluded(orbitAngle);

        if (occluded)
        {
            // Choose the faster direction when occlusion first begins
            if (!wasOccluded)
                sweepDir = ChooseSweepDirection();

            // Accelerate toward max sweep speed in chosen direction
            sweepVelocity = Mathf.MoveTowards(
                sweepVelocity,
                sweepSpeed * sweepDir,
                sweepAcceleration * Time.deltaTime);
        }
        else
        {
            // Decelerate smoothly to a stop at the new clear angle
            sweepVelocity = Mathf.MoveTowards(
                sweepVelocity,
                0f,
                sweepAcceleration * Time.deltaTime);
        }

        wasOccluded  = occluded;
        orbitAngle  += sweepVelocity * Time.deltaTime;

        // Smooth follow to desired position
        Vector3 desiredPosition = OrbitPosition(orbitAngle);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up);
    }

    // ── Helpers ───────────────────────────────────────────────────────────

    // Picks the sweep direction that finds clear space sooner
    int ChooseSweepDirection()
    {
        float peek      = 20f;
        bool rightClear = !IsOccluded(orbitAngle + peek);
        bool leftClear  = !IsOccluded(orbitAngle - peek);

        if (rightClear && !leftClear) return  1;
        if (leftClear  && !rightClear) return -1;
        return 1; // both blocked or both clear — go right
    }

    // Checks whether an obstacle blocks line of sight at a given orbit angle
    bool IsOccluded(float angle)
    {
        Vector3 camPos = OrbitPosition(angle);
        Vector3 from   = target.position + Vector3.up;
        Vector3 dir    = camPos - from;

        return Physics.Raycast(from, dir.normalized, dir.magnitude, occlusionLayers);
    }

    // World position of the camera at a given orbit angle
    Vector3 OrbitPosition(float angle)
    {
        Vector3 horizontal = new Vector3(offset.x, 0f, offset.z);
        float   radius     = horizontal.magnitude;
        Vector3 baseDir    = radius > 0.001f ? horizontal.normalized : Vector3.back;

        Vector3 orbitDir = Quaternion.Euler(0f, angle, 0f) * baseDir;
        return target.position + orbitDir * radius + Vector3.up * offset.y;
    }
}
