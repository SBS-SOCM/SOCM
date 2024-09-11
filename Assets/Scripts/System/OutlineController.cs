using UnityEngine;

public class OutlineController : MonoBehaviour
{
    public Material outlineMaterial;  // 적용할 Outline 머티리얼
    private Material[][] originalMaterialsArray;  // 원래 머티리얼을 저장할 배열
    private Renderer[] renderers;

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

        ApplyOutline();
    }

    // 외곽선 머티리얼을 기존 머티리얼과 함께 추가
    void ApplyOutline()
    {
        foreach (Renderer renderer in renderers)
        {
            Material[] newMaterials = new Material[renderer.materials.Length + 1];
            for (int j = 0; j < renderer.materials.Length; j++)
            {
                Debug.Log(newMaterials.Length);

                newMaterials[j] = renderer.materials[j];  // 기존 머티리얼 유지
            }
            newMaterials[renderer.materials.Length] = outlineMaterial;  // 외곽선 머티리얼 추가

            renderer.materials = newMaterials;  // 새로운 머티리얼 배열 적용
        }
    }

    // 외곽선 머티리얼 제거 함수
    public void RemoveOutline()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].materials = originalMaterialsArray[i];  // 원래 머티리얼로 복구
        }
    }
}
        