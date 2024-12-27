using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class PlayerEntry : MonoBehaviour
{
    public ToggleGroup characterSelectToggleGroup;
    public Text playerNameText;
    public Toggle readyToggle;
    public GameObject ready;

    private List<Toggle> selectToggles = new List<Toggle>();

    public Player player;
    public bool IsMine => player == PhotonNetwork.LocalPlayer;

    private bool localReady = false;

    private void Awake()
    {
        foreach (Transform toggleTransform in characterSelectToggleGroup.transform)
        {
            selectToggles.Add(toggleTransform.GetComponent<Toggle>());
        }
        readyToggle.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                localReady = true;
                ready.SetActive(localReady);
            }
            else
            {
                localReady = false;
                ready.SetActive(localReady);
            }
        });
    }

    private void Start()
    {
        Hashtable customProperties = player.CustomProperties;
        if (false == customProperties.ContainsKey("CharacterSelect"))
        {
            customProperties.Add("CharacterSelect", 0);
        }

        if (false == customProperties.ContainsKey("Ready"))
        {
            customProperties.Add("Ready", localReady);
        }

        int select = (int)customProperties["CharacterSelect"];
        selectToggles[select].isOn = true;
        if (IsMine)
        {
            for (int i = 0; i < selectToggles.Count; i++)
            {   //�Ϻη� �͸�޼��忡 ������ ĸ���ϱ� ���� ���� ������ ���� ����
                int index = i;

                selectToggles[i].onValueChanged.AddListener(
                    isOn =>
                    {
                        if (isOn)
                        {
                            Hashtable customProperties = player.CustomProperties;
                            customProperties["CharacterSelect"] = index;
                            player.SetCustomProperties(customProperties);
                        }
                    }
                );
            }

            readyToggle.onValueChanged.AddListener(
                isOn =>
                {
                    if (isOn)
                    {
                        Hashtable customProperties = player.CustomProperties;
                        customProperties["Ready"] = localReady;
                        player.SetCustomProperties(customProperties);
                    }
                    else
                    {
                        Hashtable customProperties = player.CustomProperties;
                        customProperties["Ready"] = localReady;
                        player.SetCustomProperties(customProperties);
                    }
                });
        }
        else
        {
            for (int i = 0; i < selectToggles.Count; i++)
            {
                selectToggles[i].interactable = false;
            }
        }

        if ((bool)player.CustomProperties["Ready"] == true)
        {
            ready.SetActive(true);
        }
    }

    public void SetSelection(int select)
    {
        if (IsMine) return;
        selectToggles[select].isOn = true;
    }

    public void SetReady(bool ready)
    {
        if (IsMine) return;
        readyToggle.isOn = ready;
    }
}

