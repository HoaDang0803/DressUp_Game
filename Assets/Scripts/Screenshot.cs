using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using TMPro;

public class Screenshot : MonoBehaviour
{
    public static Screenshot instance;
    public GameObject UIDressUp;
    public GameObject model;
    public GameObject galleryPanel;
    public bool hasPermission = false;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    async void Start()
    {
        await RequestPermissionAsynchronously(NativeGallery.PermissionType.Write, NativeGallery.MediaType.Image);
    }

    private IEnumerator ScreenShot()
    {
        yield return new WaitForEndOfFrame();
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);

        texture.Apply();
        string name = "Outfit " + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";

        // byte[] bytes = texture.EncodeToPNG();
        // File.WriteAllBytes(Application.dataPath + "/Resources/Gallery/" + name, bytes);

        //Mobile
        NativeGallery.SaveImageToGallery(texture, "GalleryTest", name);

        Destroy(texture);
        model.transform.DOMoveX(-1.52f, 1f);
        UIDressUp.SetActive(true);
    }

    private async Task RequestPermissionAsynchronously(NativeGallery.PermissionType permissionType, NativeGallery.MediaType mediaTypes)
    {
        NativeGallery.Permission permission = await NativeGallery.RequestPermissionAsync(permissionType, mediaTypes);
        if (NativeGallery.CheckPermission(NativeGallery.PermissionType.Write, NativeGallery.MediaType.Image) == NativeGallery.Permission.Denied)
        {
            hasPermission = false;
        }
        else if (NativeGallery.CheckPermission(NativeGallery.PermissionType.Write, NativeGallery.MediaType.Image) == NativeGallery.Permission.Granted)
        {
            hasPermission = true;
        }
        Debug.Log(permission.ToString());
    }

    public void TakeScreenshot()
    {
        if (!hasPermission)
        {
            return;
        }
        else
        {
            UIDressUp.SetActive(false);
            model.transform.position = new Vector3(0, 0, 0);
            StartCoroutine(ScreenShot());
        }

    }

    public void OpenGallery()
    {
        galleryPanel.SetActive(true);
    }
}
