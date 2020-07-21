﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;

public class RoomData :  MonoBehaviourPun
{

    public string roomName = "";
    public int playerCount = 0;
    public int maxPlayer = 0;

    [System.NonSerialized]
    public Text roomDataTxt;

    void Awake()
    {
        roomDataTxt = GetComponentInChildren<Text>();
    }

    public void UpdateInfo()
    {
        roomDataTxt.text = string.Format(" {0} [{1}/{2}]"
                                        , roomName
                                        , playerCount.ToString("00")
                                        , maxPlayer);
    }
}
