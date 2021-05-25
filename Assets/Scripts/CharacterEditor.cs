using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///WARNING:
/// Large parts of this script are commented out since the "Character Creation"-Feature has been disbaled

namespace QuickStart
{
    public class CharacterEditor : MonoBehaviour
    {
        public PlayerScript player;

        public GameObject canvas;

        /*und Die Die 

                public Material material;

                public int currentHatIndex = 1;

                public int currentWingIndex = 1;


                public List<GameObject> Hats;

                public List<GameObject> Wings;
        */

        private void Awake()
        {
            /*
            foreach (GameObject g in Hats)
            {
                g.SetActive(false);
                currentHatIndex = 0;
            }
            foreach (GameObject g in Wings)
            {
                g.SetActive(false);
                currentWingIndex = 0;
            }*/
        }

        public void SetName(string n)
        {
            player.tempName = n;
        }

        /*
                public void SetColor(Color c)
                {
                    player.tempColor = c;
                    material.color = c;
                }

        */

        public void Open()
        {
            canvas.SetActive(true);
        }

        public void Close()
        {
            canvas.SetActive(false);
        }

        /*
                public void MoveHats(bool left)
                {

                    Debug.Log("Changing Hat");


                                foreach (GameObject hat in Hats)
                                {
                                    hat.SetActive(false);
                                }

                                if (left)
                                {
                                    if (currentHatIndex - 1 < 0)
                                    {
                                        currentHatIndex = Hats.Count - 1;
                                    }
                                    else
                                    {
                                        currentHatIndex--;
                                    }
                                }
                                else
                                {
                                    if (currentHatIndex + 1 >= Hats.Count)
                                    {
                                        currentHatIndex = 0;
                                    }
                                    else
                                    {
                                        currentHatIndex++;
                                    }
                                }
                                Hats[currentHatIndex].SetActive(true);
                                player.tempHat = currentHatIndex;

                                */
    }

    /*
            public void MoveWings(bool left)
            {

                Debug.Log("Changing Wings");

                foreach (GameObject wing in Wings)
                {
                    wing.SetActive(false);
                }

                if (left)
                {
                    if (currentWingIndex - 1 < 0)
                    {
                        currentWingIndex = Wings.Count - 1;
                    }
                    else
                    {
                        currentWingIndex--;
                    }
                }
                else
                {
                    if (currentWingIndex + 1 >= Wings.Count)
                    {
                        currentWingIndex = 0;
                    }
                    else
                    {
                        currentWingIndex++;
                    }
                }
                Wings[currentWingIndex].SetActive(true);
                player.tempWings = currentWingIndex;
            }*/
}