using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroSceneManager : MonoBehaviour
{
    public Text introText;

    static string codename;

    SceneManagement sceneManagement;

    public GameObject[] codenameTextSpace;
    private void Start()
    {
        StartCoroutine(IntroTexting());

        sceneManagement = GetComponent<SceneManagement>();
    }

    IEnumerator IntroTexting()
    {
        introText.text = "";
        string nextText = "우리는 불가능에 가까운 어쩌면 불가능한\n임무를 가능하게 만드는 특수 팀입니다.";

        introText.DOText(nextText, 3f);

        yield return new WaitForSeconds(4f);

        introText.text = "";
        nextText = "당신은 전투에도 능하고 은신 능력을 가지고 있어\n우리들이 판단하기에 우리 팀의 적임자라고 생각합니다.";

        introText.DOText(nextText, 4f);

        yield return new WaitForSeconds(5f);

        introText.text = "";
        nextText = "당신이 우리와 함께 임무를 수행하면\n세계 안보를 수호하는 영웅이 되실 수 있습니다.";

        introText.DOText(nextText, 3f);

        yield return new WaitForSeconds(4f);


        introText.text = "";
        nextText = "우리와 함께 임무 수행을 수락하신다면,\n당신의 코드 명을 말해주십시오.";

        introText.DOText(nextText, 2f);

        yield return new WaitForSeconds(2f);

        for (int i = 0; i < codenameTextSpace.Length; i++)
        {
            codenameTextSpace[i].SetActive(true);
        }

        
    }
    
    public void SetCodename()
    {
        string txt = codenameTextSpace[0].transform.GetChild(2).GetComponent<Text>().text;

        if (txt == "")
        {
            return;
        }
        else
        {
            codename = txt;
            sceneManagement.LoadScene(2);
        }
    }

}