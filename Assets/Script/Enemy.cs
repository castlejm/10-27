﻿using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviourPunCallbacks, IPunObservable
{
    NavMeshAgent pathfinder;
    private Transform tr;
    Transform target;
    float distance, distance0, distance1, distance2, distance3;
    GameObject[] robolist;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();

    }

    private void FindRobo()
    {
        //target = GameObject.FindGameObjectWithTag("ROBO").transform;

        //가까운놈으로 타겟 초기화
        if (distance == distance0)
            target = robolist[0].transform;
        else if (distance == distance1)
            target = robolist[1].transform;
        else if (distance == distance2)
            target = robolist[2].transform;
        else
            target = null;

        StartCoroutine(UpdatePath());
    }




    void Update()
    {
        robolist = GameObject.FindGameObjectsWithTag("ROBO");

        if (GameObject.FindGameObjectWithTag("ROBO"))
        {
            distance0 = Vector3.Distance(robolist[0].transform.position, transform.position);
            distance1 = Vector3.Distance(robolist[1].transform.position, transform.position);
            distance2 = Vector3.Distance(robolist[2].transform.position, transform.position);
            //distance3 = Vector3.Distance(robolist[3].transform.position, transform.position);
        }


        distance = Mathf.Min(distance0, distance1, distance2);//, distance3);


        if (distance <= 10.0f)
        {
            pathfinder = GetComponent<NavMeshAgent>();

            FindRobo();

        }
        else
        {
            target = null;
        }

        if (!photonView.IsMine)
        {
            if ((tr.position - currPos).sqrMagnitude >= 10.0f * 10.0f)
            {
                tr.position = currPos;
                tr.rotation = currRot;
            }
            else
            {
                tr.position = Vector3.Lerp(tr.position, currPos, Time.deltaTime * 10.0f);
                tr.rotation = Quaternion.Slerp(tr.rotation, currRot, Time.deltaTime * 10.0f);
            }
        }

    }

    IEnumerator UpdatePath()
    {
        float refreshRate = 1;
        while (target != null)
        {
            Vector3 targetPosition = new Vector3(target.position.x, -20, target.position.z);
            pathfinder.SetDestination(targetPosition);
            yield return new WaitForSeconds(refreshRate);
        }
    }

    private Vector3 currPos;    // 실시간으로 전송하고 받는 변수
    private Quaternion currRot; // 실시간으로 전송하고 받는 변수
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) //데이터를 계속 전송만
        {
            stream.SendNext(tr.position);   //내 위치값을 보낸다
            stream.SendNext(tr.rotation);   //내 회전값을 보낸다
        }
        else
        {
            //stream.ReceiveNext()는 오브젝트 타입이라  currPos에 맞게 vector3로 변경해준다.
            currPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();
        }
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("ROBO"))
        {
            Destroy(gameObject);
        }
    }

}