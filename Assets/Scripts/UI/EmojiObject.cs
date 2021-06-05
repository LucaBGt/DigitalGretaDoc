using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmojiObject : MonoBehaviour
{
    [SerializeField] Image image;
    Camera cam;

    public void SetSprite(Sprite sprite)
    {
        image.sprite = sprite;
    }

    private void Start()
    {
        cam = Camera.main;
        Destroy(gameObject, 3);
    }

    void Update()
    {
        transform.forward = transform.position - cam.transform.position;
        transform.Translate(Vector3.up * Time.deltaTime);
    }
}
