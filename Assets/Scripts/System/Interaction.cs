using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
    public GameObject player;
    public Inventory inventory;

    public float detectionRadius = 5f;  // 감지 반경
    public float detectionAngle = 45f;  // 감지 각도
    public LayerMask targetLayerMask;   // 9번 레이어를 포함하는 레이어 마스크
    public Color gizmoColor = Color.green;  // 기즈모 색상 설정

    Collider[] hits;

    public Image interactionImage;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.StartsWith("Ingame"))
        {
            player = GameObject.Find("PlayerArmature");
        }
    }

    private void Update()
    {
        CheckInteraction();

        if (Input.GetKeyDown(KeyCode.E))
        {
            SendInteraction();
        }
    }

    public void SendInteraction()
    {
        Collider[] hits = Physics.OverlapSphere(player.transform.position, detectionRadius, targetLayerMask);

        foreach (Collider hit in hits)
        {
            // 오브젝트의 방향 벡터를 계산
            Vector3 directionToTarget = hit.transform.transform.position - player.transform.position;
            directionToTarget.Normalize();

            // 오브젝트와 플레이어의 앞방향 벡터 간의 각도를 계산
            float angleToTarget = Vector3.Angle(player.transform.forward, directionToTarget);

            // 감지 각도 내에 있는지 확인
            if (angleToTarget <= detectionAngle)
            {
                ReceiveInteraction(hits[0]);
                
            }
        }
    }

    public void CheckInteraction()
    {
        bool isInteractive = false;

        Collider[] hits = Physics.OverlapSphere(player.transform.position, detectionRadius, targetLayerMask);

        foreach (Collider hit in hits)
        {
            // 오브젝트의 방향 벡터를 계산
            Vector3 directionToTarget = hit.transform.transform.position - player.transform.position;
            directionToTarget.Normalize();

            // 오브젝트와 플레이어의 앞방향 벡터 간의 각도를 계산
            float angleToTarget = Vector3.Angle(player.transform.forward, directionToTarget);

            // 감지 각도 내에 있는지 확인
            if (angleToTarget <= detectionAngle)
            {
                isInteractive = true;
            }
        }

        if (isInteractive)
        {
            interactionImage.gameObject.SetActive(true);
        }

        else
        {
            interactionImage.gameObject.SetActive(false);
        }
    }

    public void ReceiveInteraction(Collider interactionObject)
    {
        string interactionName = interactionObject.name;

        Debug.Log("상호작용 : "+ interactionName);

        switch (interactionName)
        {
            case "StartItem":
                Inventory.Item item1 = new Inventory.Item("Knife");
                inventory.GetItem(item1);

                Inventory.Item item2 = new Inventory.Item("Pistol");
                inventory.GetItem(item2);

                Inventory.Item item3 = new Inventory.Item("Coin");
                inventory.GetItem(item3);
                break;

            case "Map":
                interactionObject.GetComponent<Clue>().ClueOn();
                break;

            case "Labber":
                interactionObject.GetComponent<Labber>().InteractionOn();
                break;
        }
    }

     // 기즈모를 그려주는 메서드
    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        // 플레이어 위치를 기준으로 원뿔을 그리기 위해 루프를 사용
        Vector3 forward = player.transform.forward * detectionRadius;
        Vector3 start = player.transform.position;

        for (float i = -detectionAngle; i <= detectionAngle; i += 5.0f) // 5도씩 간격을 둬서 원뿔형 범위를 그립니다.
        {
            // 각도에 따른 방향을 계산
            Vector3 direction = Quaternion.Euler(0, i, 0) * forward;
            Gizmos.DrawRay(start, direction);
        }
        
        // 감지 반경을 시각화하기 위해 구를 그림
        Gizmos.DrawWireSphere(player.transform.position, detectionRadius);
    }
}
