using UnityEngine;

public class OutlineController : MonoBehaviour
{
    public Material outlineMaterial;  // ������ Outline ��Ƽ����
    private Material[][] originalMaterialsArray;  // ���� ��Ƽ������ ������ �迭
    private Renderer[] renderers;

    void Start()
    {
        // ������Ʈ�� ��� Renderer�� ������
        renderers = GetComponentsInChildren<Renderer>();

        // �� Renderer�� ���� ��Ƽ������ ����
        originalMaterialsArray = new Material[renderers.Length][];
        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterialsArray[i] = renderers[i].materials;
        }

        ApplyOutline();
    }

    // �ܰ��� ��Ƽ������ ���� ��Ƽ����� �Բ� �߰�
    void ApplyOutline()
    {
        foreach (Renderer renderer in renderers)
        {
            Material[] newMaterials = new Material[renderer.materials.Length + 1];
            for (int j = 0; j < renderer.materials.Length; j++)
            {
                Debug.Log(newMaterials.Length);

                newMaterials[j] = renderer.materials[j];  // ���� ��Ƽ���� ����
            }
            newMaterials[renderer.materials.Length] = outlineMaterial;  // �ܰ��� ��Ƽ���� �߰�

            renderer.materials = newMaterials;  // ���ο� ��Ƽ���� �迭 ����
        }
    }

    // �ܰ��� ��Ƽ���� ���� �Լ�
    public void RemoveOutline()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].materials = originalMaterialsArray[i];  // ���� ��Ƽ����� ����
        }
    }
}
        