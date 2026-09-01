using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Builds a simple red-and-black boss environment at the start of the boss round.
public class BossWorldController : MonoBehaviour
{
    public static BossWorldController Instance { get; private set; }

    private readonly Color bossFog = new Color(0.18f, 0.01f, 0.01f);
    private bool bossWorldActive;
    private Light bossLight;

    private void Awake()
    {
        Instance = this;
    }

    public void EnterBossWorld()
    {
        if (bossWorldActive) return;
        bossWorldActive = true;

        RenderSettings.fog = true;
        RenderSettings.fogColor = bossFog;
        RenderSettings.fogDensity = 0.018f;
        RenderSettings.ambientLight = new Color(0.35f, 0.03f, 0.03f);
        ApplyImportedBossSkybox();

        CreateBossLight();
        CreateLavaCracks();
    }

    private void CreateBossLight()
    {
        GameObject lightObject = new GameObject("Boss World Red Light");
        bossLight = lightObject.AddComponent<Light>();
        bossLight.type = LightType.Directional;
        bossLight.color = new Color(1f, 0.12f, 0.02f);
        bossLight.intensity = 1.35f;
        lightObject.transform.rotation = Quaternion.Euler(50f, -25f, 0f);
    }

    private void ApplyImportedBossSkybox()
    {
#if UNITY_EDITOR
        Material galaxySkybox = AssetDatabase.LoadAssetAtPath<Material>(
            "Assets/GalaxyFire/Material/GalaxyFireMaterial.mat");
        if (galaxySkybox != null) RenderSettings.skybox = galaxySkybox;
#endif
    }

    private void CreateLavaCracks()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        if (shader == null) return;

        Material lavaMaterial = null;
#if UNITY_EDITOR
        lavaMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Shaders/Lava/Lava.mat");
#endif
        if (lavaMaterial == null) lavaMaterial = new Material(shader);
        lavaMaterial.color = new Color(1f, 0.08f, 0.005f);
        lavaMaterial.EnableKeyword("_EMISSION");
        lavaMaterial.SetColor("_EmissionColor", new Color(2.8f, 0.08f, 0.005f));

        for (int i = 0; i < 9; i++)
        {
            GameObject crack = GameObject.CreatePrimitive(PrimitiveType.Cube);
            crack.name = "Boss Lava Crack";
            crack.transform.position = new Vector3(i % 2 == 0 ? -3.8f : 3.8f, 0.04f, 28f + i * 12f);
            crack.transform.localScale = new Vector3(0.22f, 0.05f, 7f);
            crack.transform.rotation = Quaternion.Euler(0f, i % 2 == 0 ? 12f : -12f, 0f);
            crack.GetComponent<Renderer>().material = lavaMaterial;
            Destroy(crack.GetComponent<Collider>());
        }
    }
}
