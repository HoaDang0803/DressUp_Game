using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class ImageLoader : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform container;
    public string galleryFolder = "GalleryTest";

    public void LoadImagesFromGallery()
    {
        if (!Screenshot.instance.hasPermission)
        {
            return;
        }
        
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        string folderPath = Path.Combine("/storage/emulated/0/DCIM/", galleryFolder);

        if (Directory.Exists(folderPath))
        {
            Debug.Log($"Scanning folder: {folderPath}");

            // Lấy danh sách file PNG trong thư mục
            string[] imagePaths = Directory.GetFiles(folderPath, "*.png");

            foreach (string path in imagePaths)
            {
                Debug.Log("Loading image: " + path);

                // Tải ảnh từ thư viện
                Texture2D texture = NativeGallery.LoadImageAtPath(path);
                if (texture != null)
                {
                    Debug.Log("Image loaded successfully!");

                    // Hiển thị ảnh trong giao diện
                    CreateButtonWithImage(texture, path);
                }
                else
                {
                    Debug.LogWarning("Failed to load image from: " + path);
                }
            }
        }
        else
        {
            Debug.LogWarning($"Gallery folder does not exist: {folderPath}");
        }
    }

    private void CreateButtonWithImage(Texture2D texture, string imagePath)
    {
        // Tạo một nút mới từ prefab
        GameObject button = Instantiate(buttonPrefab, container);

        // Gắn texture vào Image của nút
        Image buttonImage = button.GetComponentInChildren<Image>();
        if (buttonImage != null)
        {
            buttonImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
    }

}
