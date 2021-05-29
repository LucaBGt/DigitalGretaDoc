using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorInteractionBehaviour : MonoBehaviour
{
    [Header("Door UI")]

    [SerializeField] private CanvasGroup doorUI;
    [SerializeField] private RawImage image;

    stand_json stand;
    Door currentDoor;

    #region Door Interaction Logic

    //Shows the Door UI
    IEnumerator ShowDoor()
    {
        doorUI.blocksRaycasts = true;

        yield return new WaitForSeconds(1.5f);

        //TO REIMPLEMENT
        //LeanTween.alphaCanvas(doorUI, 1, 1.5f);

        doorUI.interactable = true;
    }

    //Hides the Door UI
    public void EndDoor()
    {
        currentDoor.EndDoor();
        doorUI.interactable = false;
        doorUI.blocksRaycasts = false;


        //TO REIMPLEMENT
        //LeanTween.alphaCanvas(doorUI, 0, 1f);

    }

    //Downloads the Vendor's Info JSON from the server
    IEnumerator loadJSON(string URL)
    {
        WWW www = new WWW(URL);
        while (!www.isDone)
        {
            Debug.Log("Download image on progress" + www.progress);
            yield return null;
        }

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log("Download failed");
        }
        else
        {
            Debug.Log("Download succes");
            string JSONString;
            JSONString = www.text;

            stand = JsonUtility.FromJson<stand_json>(JSONString);

            StartCoroutine(setImage(stand.image_url));

        }
    }

    //Set the Preview Image from the link provided by the JSON
    IEnumerator setImage(string URL)
    {
        WWW www = new WWW(URL);
        while (!www.isDone)
        {
            Debug.Log("Download image on progress" + www.progress);
            yield return null;
        }

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log("Download failed");
        }
        else
        {
            Debug.Log("Download succes");
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(www.bytes);
            texture.Apply();


            image.texture = texture;

        }
    }

    #endregion
}
