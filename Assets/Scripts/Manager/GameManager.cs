using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private GameObject player;
    [SerializeField] private float respawnTime;

    private float respawnTimeStart;

    private bool respawn;

    private CinemachineVirtualCamera _cinemachineVirtualCamera;

    private void Start()
    {
        _cinemachineVirtualCamera = GameObject.Find("Player Camera").GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        CheckRespawn();
    }

    public void Respawn()
    {
        respawnTimeStart = Time.time;
        respawn = true;
        
    }

    private void CheckRespawn()
    {
        if (Time.time >= respawnTimeStart + respawnTime && respawn)
        {
            var playerTemp = Instantiate(player, respawnPoint);
            _cinemachineVirtualCamera.m_Follow = playerTemp.transform;
            respawn = false;
        }
    }
}
