using UnityEngine;

public class EnemyVisual : MonoBehaviour
{
    private Transform body;
    private Vector3 bodyStartPosition;
    private Renderer[] eyes;

    private void Awake()
    {
        MeshRenderer oldRenderer = GetComponent<MeshRenderer>();
        if (oldRenderer != null) oldRenderer.enabled = false;

        body = CreatePart("Stalker Body", PrimitiveType.Capsule, Vector3.zero, new Vector3(0.9f, 1.5f, 0.9f), Color.black);
        bodyStartPosition = body.localPosition;

        Transform leftEye = CreatePart("Left Eye", PrimitiveType.Sphere, new Vector3(-0.2f, 0.35f, 0.42f), Vector3.one * 0.18f, Color.red);
        Transform rightEye = CreatePart("Right Eye", PrimitiveType.Sphere, new Vector3(0.2f, 0.35f, 0.42f), Vector3.one * 0.18f, Color.red);
        eyes = new Renderer[] { leftEye.GetComponent<Renderer>(), rightEye.GetComponent<Renderer>() };
    }

    private void Update()
    {
        float bob = Mathf.Sin(Time.time * 7f) * 0.06f;
        body.localPosition = bodyStartPosition + Vector3.up * bob;

        // Higher rounds make the stalker glow more intensely.
        foreach (Renderer eye in eyes)
        {
            if (eye != null) eye.material.color = GameManager.CurrentRound >= 4 ? Color.magenta : Color.red;
        }
        body.localRotation = Quaternion.Euler(Mathf.Sin(Time.time * 7f) * 4f, 0f, 0f);
    }

    private Transform CreatePart(string partName, PrimitiveType shape, Vector3 position, Vector3 scale, Color color)
    {
        GameObject part = GameObject.CreatePrimitive(shape);
        part.name = partName;
        part.transform.SetParent(transform);
        part.transform.localPosition = position;
        part.transform.localScale = scale;

        Collider partCollider = part.GetComponent<Collider>();
        if (partCollider != null) partCollider.enabled = false;

        Renderer partRenderer = part.GetComponent<Renderer>();
        partRenderer.material.color = color;
        return part.transform;
    }
}
