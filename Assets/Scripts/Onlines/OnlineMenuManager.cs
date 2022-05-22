using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class OnlineMenuManager : MonoBehaviourPunCallbacks
{
    // if I press a button It will start Matching
    // Random Matching
    // if a room is there it will connect, otherwise it will create a room and connect

    [SerializeField] GameObject loadingAnim;
    [SerializeField] GameObject loadingText;
    [SerializeField] GameObject matchingButton;
    bool inRoom;
    bool isMatching;


    public void OnMatchingButton()
    {
        loadingAnim.SetActive(true);
        loadingText.SetActive(true);
        matchingButton.SetActive(false);
        // PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
        PhotonNetwork.ConnectUsingSettings();
    }

    // マスターサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnConnectedToMaster()
    {
        // Random Matching
        PhotonNetwork.JoinRandomRoom();


        // "Room"という名前のルームに参加する（ルームが存在しなければ作成して参加する）
        //PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
    }

    // ゲームサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnJoinedRoom()
    {
        inRoom = true;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // CreateRoom has a callback OnjoinedRoom()
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 2 } , TypedLobby.Default);

    }

    // if There is 2 person in the room the scene will change
    private void Update()
    {
        if (isMatching)
        {
            return;
        }
        if (inRoom)
        {
            if (PhotonNetwork.CurrentRoom.MaxPlayers == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                isMatching = true;
                loadingAnim.SetActive(false);
                loadingText.SetActive(false);
                SceneManager.LoadScene("Game");

            }
        }
        
    }
}
