using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

// V switches between the normal chase camera and a close first-person view.
public class CameraModeController : MonoBehaviour
{
    private CinemachineFollow followCamera;
    private Vector3 thirdPersonOffset;
    private bool firstPerson;

    private void Start()
    {
        followCamera = FindFirstObjectByType<CinemachineFollow>();
        if (followCamera == null) return;

        // Use one safe chase-camera distance instead of inheriting a bad scene offset.
        thirdPersonOffset = new Vector3(0f, 6f, -14f);
        SetFirstPerson(false);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.vKey.wasPressedThisFrame)
        {
            SetFirstPerson(!firstPerson);
        }
    }

    private void SetFirstPerson(bool useFirstPerson)
    {
        firstPerson = useFirstPerson;
        if (followCamera == null) return;

        followCamera.FollowOffset = firstPerson
            // A tiny bit behind the head stops Cinemachine from aiming at the ground.
            ? new Vector3(0f, 4.8f, -2.5f)
            : thirdPersonOffset;
    }
}
