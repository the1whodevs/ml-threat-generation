using System.Collections;
using Mirror;
using Unity.Mathematics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f;

    [SerializeField] private float backwardsAngle = -35.0f;
    [SerializeField] private float rightAngle = 35.0f;

    [SerializeField] private float rotLerpSpeed = 2.0f;

    [SerializeField] private Transform movementRotator;
    
    private float horizontal = 0.0f;
    private float vertical = 0.0f;

    private float lerpT = 0.0f;

    private Vector3 movement;
    private Vector3 previousTargetRotVector3;
    private Vector3 targetRotVector3;
    
    private Quaternion targetRot;
    
    private readonly WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

    private void Start()
    {
        targetRotVector3 = Vector3.zero;
        targetRot = Quaternion.identity;
        previousTargetRotVector3 = Vector3.zero;

        StartCoroutine(RotationLerp());
    }

    public void SetMovementRotator(Transform t)
    {
        movementRotator = t;
    }

    public void ResetRotation()
    {
        if (targetRot.Equals(quaternion.identity)) return;
        
        targetRotVector3 = Vector3.zero;
        targetRot = Quaternion.identity;
        lerpT = 0.0f;
    }

    public bool HandleMovement(float v, float h, bool useMoveRotations)
    {
        vertical = v;
        horizontal = h;

        if (useMoveRotations)
        {
            previousTargetRotVector3 = targetRotVector3;
        
            if (v < 0.0f) targetRotVector3.x = backwardsAngle;
            else if (v > 0.0f) targetRotVector3.x = -backwardsAngle;
            else targetRotVector3.x = 0;
        
            if (h > 0.0f) targetRotVector3.z = rightAngle;
            else if (h < 0.0f) targetRotVector3.z = -rightAngle;
            else targetRotVector3.z = 0;

            if (!previousTargetRotVector3.Equals(targetRotVector3))
            {
                targetRot = Quaternion.Euler(targetRotVector3);
                lerpT = 0.0f;
            }
        }

        movement = transform.forward * vertical + transform.right * horizontal;
        movement.Normalize();

        movement.y = 0;
        
        transform.position += movement * (moveSpeed * Time.deltaTime);

        return Mathf.Abs(v) + Mathf.Abs(h) > 0.0f;
    }

    private IEnumerator RotationLerp()
    {
        while (Application.isPlaying)
        {
            while (lerpT <= 1.0)
            {
                lerpT += Time.deltaTime * rotLerpSpeed;
                
                movementRotator.localRotation = Quaternion.Lerp(movementRotator.localRotation, targetRot, lerpT);
                
                yield return waitForEndOfFrame;
            }

            yield return waitForEndOfFrame;
        }
    }
}
