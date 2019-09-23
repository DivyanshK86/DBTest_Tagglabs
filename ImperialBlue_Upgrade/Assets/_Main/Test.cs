using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System;
using GoogleARCore.Examples.Common;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GoogleARCore;

/*
 * https://github.com/ChrisMaire/unity-native-sharing
 */

public class Test : MonoBehaviour {
	public string ScreenshotName;
    public GameObject cameraButton;
    Texture2D image;

    public GameObject SaveAndCancelButtons;
    public GameObject ShareAndCancelButtons;
    [Space]
    public GameObject portraiFrame;
    public GameObject landscapeFrame;

    public PointcloudVisualizer pointcloudVisualizer;

    public RawImage previewImage;
    public ARCoreSessionConfig aRCoreSessionConfig;
    public Text focusModeText;

    string screenShotPath;
    string msgTxt;

    private void Awake()
    {
        SaveAndCancelButtons.SetActive(false);
        ShareAndCancelButtons.SetActive(false);

        portraiFrame.SetActive(false);
        landscapeFrame.SetActive(false);

        previewImage.gameObject.SetActive(false);


        pointcloudVisualizer.enabled = true;

        Screen.orientation = ScreenOrientation.AutoRotation;
    }

    public void ShareScreenshotWithText(string text)
    {
        StartCoroutine(TurnOffTheButton(text));
    }

    IEnumerator TurnOffTheButton(String text)
    {
        cameraButton.SetActive(false);
        pointcloudVisualizer.enabled = false;


        if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
        {
            portraiFrame.SetActive(true);
        }
        else
        {
            landscapeFrame.SetActive(true);
        }

        yield return new WaitForSeconds(0.1f);

        ScreenshotName = UnityEngine.Random.Range(10000, 99999).ToString() + ".png";

        screenShotPath = Application.persistentDataPath + "/" + ScreenshotName;
        if (File.Exists(screenShotPath)) File.Delete(screenShotPath);
        ScreenCapture.CaptureScreenshot(ScreenshotName);

        image = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        image.Apply();

        previewImage.texture = image;
        previewImage.gameObject.SetActive(true);

        byte[] bytes = image.EncodeToPNG();
        var dirPath = Application.persistentDataPath + "/";
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        string fileName = "AR_" + DateTime.Now.ToString().Replace("/", "-") + ".png";
        File.WriteAllBytes(dirPath + fileName, bytes);

        msgTxt = text;

        yield return new WaitForSeconds(0.1f);

        portraiFrame.SetActive(false);
        landscapeFrame.SetActive(false);

        ShowSaveAndCanceOption();
    }

    void ShowSaveAndCanceOption()
    {
        SaveAndCancelButtons.SetActive(true);
    }

    public void _SaveToGallery()
    {

        NativeGallery.SaveImageToGallery(image, "AR_", DateTime.Now.ToString().Replace("/", "-"));

        SaveAndCancelButtons.SetActive(false);
        ShareAndCancelButtons.SetActive(true);
    }

    public void _CancelSave()
    {
        SaveAndCancelButtons.SetActive(false);
        cameraButton.SetActive(true);
        previewImage.gameObject.SetActive(false);


        portraiFrame.SetActive(false);
        landscapeFrame.SetActive(false);
        pointcloudVisualizer.enabled = true;

    }

    void ShowShareAndCancelOption()
    {
        ShareAndCancelButtons.SetActive(true);
    }

    public void _ShareButton()
    {
        StartCoroutine(delayedShare(screenShotPath, msgTxt));
        ShareAndCancelButtons.SetActive(false);
    }

    public void _CancelShareButton()
    {
        ShareAndCancelButtons.SetActive(false);
        cameraButton.SetActive(true);
        previewImage.gameObject.SetActive(false);


        portraiFrame.SetActive(false);
        landscapeFrame.SetActive(false);
        pointcloudVisualizer.enabled = true;

    }

    //CaptureScreenshot runs asynchronously, so you'll need to either capture the screenshot early and wait a fixed time
    //for it to save, or set a unique image name and check if the file has been created yet before sharing.
    IEnumerator delayedShare(string screenShotPath, string text)
    {
        while(!File.Exists(screenShotPath)) {
    	    yield return new WaitForSeconds(.05f);
        }

		NativeShare.Share(text, screenShotPath, "", "", "image/png", true, "");
        yield return new WaitForSeconds(0.1f);
        cameraButton.SetActive(true);
        previewImage.gameObject.SetActive(false);


        portraiFrame.SetActive(false);
        landscapeFrame.SetActive(false);
        pointcloudVisualizer.enabled = true;
    }

    //---------- Helper Variables ----------//
    private float width
    {
        get
        {
            return Screen.width;
        }
    }

    private float height
    {
        get
        {
            return Screen.height;
        }
    }


	//---------- Screenshot ----------//
	public void Screenshot()
	{
		// Short way
		StartCoroutine(GetScreenshot());
	}

    //---------- Get Screenshot ----------//
    public IEnumerator GetScreenshot()
    {
        yield return new WaitForEndOfFrame();

        // Get Screenshot
        Texture2D screenshot;
        screenshot = new Texture2D((int)width, (int)height, TextureFormat.ARGB32, false);
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);
        screenshot.Apply();

        // Save Screenshot
        Save_Screenshot(screenshot);
    }

    //---------- Save Screenshot ----------//
    private void Save_Screenshot(Texture2D screenshot)
    {
        string screenShotPath = Application.persistentDataPath + "/" + DateTime.Now.ToString("dd-MM-yyyy_HH:mm:ss") + "_" + ScreenshotName;
        File.WriteAllBytes(screenShotPath, screenshot.EncodeToPNG());

        // Native Share
        StartCoroutine(DelayedShare_Image(screenShotPath));
    }

    //---------- Clear Saved Screenshots ----------//
    public void Clear_SavedScreenShots()
    {
        string path = Application.persistentDataPath;

        DirectoryInfo dir = new DirectoryInfo(path);
        FileInfo[] info = dir.GetFiles("*.png");

        foreach (FileInfo f in info)
        {
            File.Delete(f.FullName);
        }
    }

    //---------- Delayed Share ----------//
    private IEnumerator DelayedShare_Image(string screenShotPath)
    {
        while (!File.Exists(screenShotPath))
        {
            yield return new WaitForSeconds(.05f);
        }

        // Share
        NativeShare_Image(screenShotPath);
    }

    //---------- Native Share ----------//
    private void NativeShare_Image(string screenShotPath)
    {
        string text = "";
        string subject = "";
        string url = "";
        string title = "Select sharing app";

#if UNITY_ANDROID

        subject = "Test subject.";
        text = "Test text";
#endif

#if UNITY_IOS
        subject = "Test subject.";
        text = "Test text";
#endif

		// Share
        NativeShare.Share(text, screenShotPath, url, subject, "image/png", true, title);
    }

    public void _ResetButton()
    {
        GoogleARCore.Examples.ObjectManipulation.AndyPlacementManipulator.objectCreated = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void _ToggleCameraMode()
    {
        if(aRCoreSessionConfig.CameraFocusMode == CameraFocusMode.Auto)
        {
            aRCoreSessionConfig.CameraFocusMode = CameraFocusMode.Fixed;
            focusModeText.text = "Fixed";
        }
        else
        {
            aRCoreSessionConfig.CameraFocusMode = CameraFocusMode.Auto;
            focusModeText.text = "Auto Focus";
        }
    }
}

