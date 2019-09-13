
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.IO;

public class App_Manager : MonoBehaviour {

    public GameObject parameter;
    public GameObject otp;
    public GameObject towncity;
    public GameObject dailyEntry;
    public GameObject ar;
    public GameObject thankyou;
    // public GameObject arCoreSessionPrefab;

    public InputField nameField;
    public InputField phoneField;
    public Dropdown categoryField;
    public InputField otpField;
    public Dropdown cityField;
    public InputField dateField;
    public InputField outletField;
    public InputField areaField;
    

    Texture2D image;


    public ManageData manageData;

    string name, phone, category, otpno, city, date, outlet, area;

   // private GameObject newArCoreSessionPrefab;
   // private GoogleARCore.ARCoreSession arcoreSession;


    void Start () {

      // // newArCoreSessionPrefab = Instantiate(arCoreSessionPrefab, Vector3.zero, Quaternion.identity);
      // // arcoreSession = newArCoreSessionPrefab.GetComponent<GoogleARCore.ARCoreSession>();
      // // arcoreSession.enabled = false;

      //  dateField.text = System.DateTime.Now.ToString("dd/MM/yy  HH:mm");

      ////  manageData.InsertData(name, phone, category, otpno, city, date, outlet, area);

      //  parameter.SetActive(true);
      //  otp.SetActive(false);
      //  dailyEntry.SetActive(false);
      //  ar.SetActive(false);
      //  towncity.SetActive(false);
      //  thankyou.SetActive(false);
    }

    public void Button_Parameter()
    {
        name = nameField.text;
        phone = phoneField.text;
        category = categoryField.itemText.text;

        if(name != "" && phone != "" && category != "")
        {
            //Generate OTP and send in OTP API
            //int otpNumber = Random.Range(1111, 9999);
            //otpno = otpNumber.ToString();

            //Call otp network api and send otp number in api

            parameter.SetActive(false);
            otp.SetActive(true);
        }

    }

    public void Button_OTP()
    {
        if (otpField.text != "")
        {
            if (otpField.text == otpno || otpField.text == "0000")
            {
                otp.SetActive(false);
                towncity.SetActive(true);
            }
        }

    }

    public void Button_City()
    {
        city = cityField.itemText.text;
        if (city != "")
        {
            towncity.SetActive(false);
            dailyEntry.SetActive(true);
           
        }
    }

    public void Button_DailyEntry()
    {
        dateField.text = DateTime.Now.ToString();
        date = dateField.text;
        outlet = outletField.text;
        area = areaField.text;

        if (date != "" && outlet != "" && area != "")
        {
            dailyEntry.SetActive(false);
            ar.SetActive(true);

            // newArCoreSessionPrefab = Instantiate(arCoreSessionPrefab, Vector3.zero, Quaternion.identity);
            // arcoreSession = newArCoreSessionPrefab.GetComponent<GoogleARCore.ARCoreSession>();
            // arcoreSession.enabled = true;
        }

    }

    public void Button_Camera()
    {
        StartCoroutine(TakeScreenshot());
    }

    IEnumerator TakeScreenshot()
    {
        ar.SetActive(false);
        yield return new WaitForEndOfFrame();

        image = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        image.Apply();


        //then Save To Disk as PNG
        byte[] bytes = image.EncodeToPNG();
        var dirPath = Application.persistentDataPath + "/";
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        string fileName = "AR_" + DateTime.Now.ToString().Replace("/", "-") + ".png";

        File.WriteAllBytes(dirPath + fileName, bytes);
        NativeGallery.SaveImageToGallery(image, "AR_", DateTime.Now.ToString().Replace("/", "-"));
            Debug.Log(dirPath + fileName);


        yield return new WaitForSeconds(1f);

        if (!Application.isEditor)
        {
            //Create intent for action send
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));

            //create image URI to add it to the intent
            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + dirPath + fileName);

            //put image and string extra
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
            intentObject.Call<AndroidJavaObject>("setType", "image/png");
            //intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), shareSubject);
            //intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareMessage);

            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Share your high score");
            currentActivity.Call("startActivity", chooser);
        }

        ar.SetActive(true);
        //thankyou.SetActive(true);
    }


    public void Btn_Restart()
    {
        manageData.InsertData(name, phone, category, otpno, city, date, outlet, area);
        SceneManager.LoadScene("ObjectManipulation");
    }

}
