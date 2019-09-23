using UnityEngine;
using System.Collections;

public class VersionManager : MonoBehaviour
{
    public static VersionManager instance;

    const string url = "https://onedrive.live.com/download?cid=38B96754D4E79B3C&resid=38B96754D4E79B3C%21947&authkey=AAMCABpEmzr3r7I";

    void Awake()
    {
        instance = this;
        CheckGameVersion();
    }

    public void CheckGameVersion()
    {
        StartCoroutine(GetTextFromWWW());
    }

    IEnumerator GetTextFromWWW()
    {
        WWW www = new WWW(url);

        yield return www;

        if (www.error != null)
        {
            //FailedToGetFile();
            yield return new WaitForSeconds(1);
            StartCoroutine(GetTextFromWWW());
        }
        else if (www.text == "" || www.text[0] != 'v')
        {
            //FailedToGetFile();
        }
        else
        {
            string[] str = DivideStringToLines(www.text);

            PlayerPrefs.SetFloat("screenYpos", float.Parse(str[1]));
            PlayerPrefs.SetFloat("carScale", float.Parse(str[2]));

            PlayerPrefs.SetFloat("view1", float.Parse(str[3]));
            PlayerPrefs.SetFloat("view2", float.Parse(str[4]));
            PlayerPrefs.SetFloat("view3", float.Parse(str[5]));
            PlayerPrefs.SetFloat("view4", float.Parse(str[6]));

            Debug.Log("Data recieved !");
        }
    }

    string[] DivideStringToLines(string str)
    {
        string[] lines = str.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);
        return lines;
    }
}