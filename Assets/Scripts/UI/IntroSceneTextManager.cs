using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroSceneTextManager : MonoBehaviour
{
    public Text introText;

    SceneManagement sceneManagement;

    private void Start()
    {
        StartCoroutine(IntroTexting());

        sceneManagement = GetComponent<SceneManagement>();
    }

    IEnumerator IntroTexting()
    {
        introText.text = "";
        string nextText = "�츮�� �Ұ��ɿ� ����� ��¼�� �Ұ�����\n�ӹ��� �����ϰ� ����� Ư�� ���Դϴ�.";

        introText.DOText(nextText, 3f);

        yield return new WaitForSeconds(4f);

        introText.text = "";
        nextText = "����� �������� ���ϰ� ���� �ɷ��� ������ �־�\n�츮���� �Ǵ��ϱ⿡ �츮 ���� �����ڶ�� �����մϴ�.";

        introText.DOText(nextText, 4f);

        yield return new WaitForSeconds(5f);

        introText.text = "";
        nextText = "����� �츮�� �Բ� �ӹ��� �����ϸ�\n���� �Ⱥ��� ��ȣ�ϴ� ������ �ǽ� �� �ֽ��ϴ�.";

        introText.DOText(nextText, 3f);

        yield return new WaitForSeconds(4f);


        introText.text = "";
        nextText = "�츮�� �Բ� �ӹ� ������ �����ϽŴٸ�,\n����� �ڵ� ���� �����ֽʽÿ�.";

        introText.DOText(nextText, 2f);

        yield return new WaitForSeconds(3f);

        sceneManagement.LoadScene(3);
    }

}
