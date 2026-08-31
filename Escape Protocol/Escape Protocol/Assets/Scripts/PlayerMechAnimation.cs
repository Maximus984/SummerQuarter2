using UnityEngine;

// Small procedural walk animation for the existing mech model.
// It keeps the project simple and does not require a separate Animator setup.
public class PlayerMechAnimation : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 9f;
    [SerializeField] private float legSwing = 12f;
    [SerializeField] private float bodyBob = 0.06f;

    private Transform[] legs;
    private Quaternion[] restRotations;
    private Transform body;
    private Vector3 bodyRestPosition;
    private PlayerController player;

    private void Start()
    {
        player = GetComponent<PlayerController>();
        body = FindChild(transform, "SK_ISO_Mech");

        legs = new Transform[]
        {
            FindChild(transform, "FrontLeg1_L"),
            FindChild(transform, "FrontLeg1_R"),
            FindChild(transform, "MiddleLeg1_L"),
            FindChild(transform, "MiddleLeg1_R"),
            FindChild(transform, "BackLeg1_L"),
            FindChild(transform, "BackLeg1_R")
        };

        restRotations = new Quaternion[legs.Length];
        for (int i = 0; i < legs.Length; i++)
        {
            if (legs[i] != null) restRotations[i] = legs[i].localRotation;
        }

        if (body != null) bodyRestPosition = body.localPosition;
    }

    private void Update()
    {
        if (legs == null) return;

        float running = player != null && player.CurrentHealth > 0 ? 1f : 0f;
        float cycle = Mathf.Sin(Time.time * walkSpeed) * running;

        for (int i = 0; i < legs.Length; i++)
        {
            if (legs[i] == null) continue;

            float side = i % 2 == 0 ? 1f : -1f;
            legs[i].localRotation = restRotations[i] * Quaternion.Euler(cycle * legSwing * side, 0f, 0f);
        }

        if (body != null)
        {
            body.localPosition = bodyRestPosition + Vector3.up * Mathf.Abs(cycle) * bodyBob;
        }
    }

    private Transform FindChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName) return child;

            Transform result = FindChild(child, childName);
            if (result != null) return result;
        }

        return null;
    }
}
