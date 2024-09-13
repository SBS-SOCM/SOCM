using UnityEngine;

public class OutlineController : MonoBehaviour
{
    public Material outlineMaterial;  // 외곽선 머티리얼
    private Material[][] originalMaterialsArray;
    private Renderer[] renderers;

    public bool isOn;

    void Start()
    {
        // 오브젝트의 모든 Renderer를 가져옴
        renderers = GetComponentsInChildren<Renderer>();

        // 각 Renderer의 원래 머티리얼을 저장
        originalMaterialsArray = new Material[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterialsArray[i] = renderers[i].materials;
        }
    }

    // 외곽선 추가 (기존 머티리얼 유지)
    public void ApplyOutline()
    {
        if (isOn)
        {
            return;
        }

        foreach (Renderer renderer in renderers)
        {
            Material[] newMaterials = new Material[renderer.materials.Length + 1];
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                newMaterials[i] = renderer.materials[i];  // 기존 머티리얼 유지
            }

            newMaterials[renderer.materials.Length] = outlineMaterial;  // 외곽선 머티리얼 추가
            renderer.materials = newMaterials;
        }
        isOn = true;
    }

    // 외곽선 제거 함수
    public void RemoveOutline()
    {
        if (!isOn)
        {
            return;
        }
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].materials = originalMaterialsArray[i];  // 원래 머티리얼로 복구
        }

        isOn = false;
    }
}
