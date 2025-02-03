using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class DraggableObject1 : NetworkBehaviour
{
    private Vector3 offset;
    private float zCoordinate;
    public float defaultGravity;
    private Rigidbody2D rb;
    private Vector3 lastPosition;
    private Vector3 velocity; // Velocity of the object during dragging.
    private bool isDragging = false;
    private Collider2D objectCollider; // Reference to the object's collider.

    [SerializeField] private float smoothSpeed = 10f; // Speed for interpolation.

    private Vector3 targetPosition; // Target position for interpolation.
    private Quaternion targetRotation; // Target rotation for interpolation.
    private Vector3 targetVelocity; // Target velocity for interpolation.
    private Vector3 previousPosition; // Position from the frame before last.

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        objectCollider = GetComponent<Collider2D>(); // Get the collider component.
        defaultGravity = rb.gravityScale;

        // Initialize target position, rotation, and velocity.
        targetPosition = transform.position;
        targetRotation = transform.rotation;
        targetVelocity = Vector3.zero;
    }

    void Update()
    {
        if (!IsOwner)
        {
            // Smoothly interpolate position, rotation, and velocity for non-owners.
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
            return;
        }

        if (isDragging)
        {
            Vector3 newPosition = GetMouseWorldPosition() + offset;

            // Shift the positions to track changes over time.
            previousPosition = lastPosition;
            lastPosition = newPosition;

            // Move the object to the new position.
            transform.position = newPosition;

            // Send updated data to the server.
            UpdateTransformAndVelocityServerRpc(newPosition, transform.rotation, Vector3.zero);
        }

    }

    void OnMouseDown()
    {
        if (!IsOwner)
        {
            // Request ownership of the object.
            RequestOwnershipServerRpc();
        }
        StartDragging();
    }

    void OnMouseDrag()
    {
        if (!isDragging || !IsOwner) return;

        // Move the object to follow the mouse, maintaining the offset.
        Vector3 newPosition = GetMouseWorldPosition() + offset;
        transform.position = newPosition;

        // Calculate velocity based on change in position over time.
        velocity = (newPosition - lastPosition) / Time.deltaTime;
        lastPosition = newPosition;

        // Send the updated position, rotation, and velocity to the server.
        UpdateTransformAndVelocityServerRpc(newPosition, transform.rotation, velocity);
    }



    void OnMouseUp()
    {
        if (!IsOwner) return;

        StopDragging();
        velocity = (lastPosition - previousPosition) / Time.deltaTime;
        UpdateVelocityServerRpc(velocity);
    }



    private void StopDragging()
    {
        rb.gravityScale = defaultGravity;
        rb.isKinematic = false;

        isDragging = false;
        objectCollider.enabled = true;

        targetVelocity = Vector3.zero;

        StopDraggingServerRpc();
        StartCoroutine(DelayedOwnershipTransfer()); // Delayed transfer
    }

    IEnumerator DelayedOwnershipTransfer()
    {
        yield return new WaitForSeconds(0.1f); // Slight delay
        TransferOwnershipToHostServerRpc();
    }



    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoordinate;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void StartDragging()
    {
        Debug.Log("[Client] Starting drag.");
        offset = transform.position - GetMouseWorldPosition();
        lastPosition = transform.position; // Store the last position for velocity calculation.
        isDragging = true;

        rb.gravityScale = 0; // Disable gravity while dragging.
        rb.isKinematic = true; // Prevent physics interactions while dragging.

        // Disable the collider to prevent self-collisions.
        objectCollider.enabled = false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestOwnershipServerRpc(ServerRpcParams rpcParams = default)
    {
        // Transfer ownership to the client requesting it.
        GetComponent<NetworkObject>().ChangeOwnership(rpcParams.Receive.SenderClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void TransferOwnershipToHostServerRpc()
    {
        // Transfer ownership back to the host (server).
        GetComponent<NetworkObject>().ChangeOwnership(NetworkManager.ServerClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateTransformAndVelocityServerRpc(Vector3 newPosition, Quaternion newRotation, Vector3 newVelocity)
    {
        // Update the target position, rotation, and velocity on the server and broadcast it to all clients.
        targetPosition = newPosition;
        targetRotation = newRotation;
        targetVelocity = newVelocity;
        UpdateTransformAndVelocityClientRpc(newPosition, newRotation, newVelocity);
    }

    [ClientRpc]
    private void UpdateTransformAndVelocityClientRpc(Vector3 newPosition, Quaternion newRotation, Vector3 newVelocity)
    {
        if (!IsOwner)
        {
            // Update the target position, rotation, and velocity for non-owners.
            targetPosition = newPosition;
            targetRotation = newRotation;
            targetVelocity = newVelocity;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateVelocityServerRpc(Vector3 newVelocity)
    {
        // Update the velocity on the server and broadcast it to all clients.
        targetVelocity = newVelocity;
        rb.velocity = newVelocity; // Update the Rigidbody2D's velocity on the server.
        UpdateVelocityClientRpc(newVelocity);
    }

    [ClientRpc]
    private void UpdateVelocityClientRpc(Vector3 newVelocity)
    {
        if (!IsOwner)
        {
            // Update the velocity for non-owners.
            targetVelocity = newVelocity;
            rb.velocity = newVelocity; // Update the Rigidbody2D's velocity for non-owners.
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void StopDraggingServerRpc()
    {
        // Reset gravity and kinematic state on the server.
        rb.gravityScale = defaultGravity;
        rb.isKinematic = false;

        // Re-enable the collider on the server.
        objectCollider.enabled = true;

        // Broadcast the gravity reset and velocity to all clients.
        StopDraggingClientRpc();
    }

    [ClientRpc]
    private void StopDraggingClientRpc()
    {
        if (!IsOwner)
        {
            // Reset gravity and kinematic state for non-owners.
            rb.gravityScale = defaultGravity;
            rb.isKinematic = false;

            // Re-enable the collider for non-owners.
            objectCollider.enabled = true;
        }
    }
}