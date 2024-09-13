using UnityEngine;

public class OutlineController : MonoBehaviour
{
    public Material outlineMaterial;  // �ܰ��� ��Ƽ����
    private Material[][] originalMaterialsArray;
    private Renderer[] renderers;

    public bool isOn;

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
    }

    // �ܰ��� �߰� (���� ��Ƽ���� ����)
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
                newMaterials[i] = renderer.materials[i];  // ���� ��Ƽ���� ����
            }

            newMaterials[renderer.materials.Length] = outlineMaterial;  // �ܰ��� ��Ƽ���� �߰�
            renderer.materials = newMaterials;
        }
        isOn = true;
    }

    // �ܰ��� ���� �Լ�
    public void RemoveOutline()
    {
        if (!isOn)
        {
            return;
        }
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].materials = originalMaterialsArray[i];  // ���� ��Ƽ����� ����
        }

        isOn = false;
    }
}
