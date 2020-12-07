﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();

    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;

    public GameObject localUnitPrefab;
    public GameObject [] unitPrefabs;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destrying object!");
            Destroy(this);
        }
    }
    public void SpawnPlayer(int _id, string _username, Vector3 _position, Quaternion _rotation)
    {
        GameObject _player;
        if(_id == ClientConnection.instance.myId)
        {
            _player = Instantiate(localPlayerPrefab, _position, _rotation);
            GameObject.Find("MainCamera").GetComponent<Camera>().enabled = false;
            _player.GetComponentInChildren<Camera>().enabled = true;
        }
        else
        {
            _player = Instantiate(playerPrefab, _position, _rotation);
        }

        _player.GetComponent<PlayerManager>().id = _id;
        _player.GetComponent<PlayerManager>().username = _username;
        _player.GetComponent<PlayerManager>().units = new GameObject[20];
        players.Add(_id, _player.GetComponent<PlayerManager>());
    }

    public void SpawnUnit(int _id, int _unitId, int _health, Vector3 _position, Quaternion _rotation)
    {
        GameObject _unit;
        if (_id == ClientConnection.instance.myId)
        {
            _unit = Instantiate(localUnitPrefab, _position, _rotation);
            _unit.GetComponent<ObjectInfo>().isLocalPlayerUnit = true;
        }
        else
        {
            _unit = Instantiate(GetUnitPrefab(_id), _position, _rotation);
        }

        _unit.GetComponent<ObjectInfo>().id = _unitId;
        _unit.GetComponent<ObjectInfo>().currentHealth = _health;
        _unit.GetComponent<ObjectInfo>().maxHealth = _health;
        players[_id].GetComponent<PlayerManager>().units[_unitId] = _unit;
        _unit.transform.parent = players[_id].GetComponent<PlayerManager>().gameObject.transform;
    }

    public GameObject GetUnitPrefab(int _playerId)
    {
        switch(_playerId)
        {
            case 2: return unitPrefabs[0];
            case 3: return unitPrefabs[1];
            case 4: return unitPrefabs[2];
            default: return unitPrefabs[0];
        }
    }

}
