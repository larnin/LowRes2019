using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class CheckpointManager : MonoBehaviour
{
    string saveSpawnName = "Spawn";

    public static CheckpointManager instance { get; private set; }

    List<CheckpointInteractable> m_checkpoints = new List<CheckpointInteractable>();

    string m_currentSpawnName;

    private void Awake()
    {
        if (instance != null)
            Debug.LogError("More than one Checkpoint Manager instancied");
        instance = this;
    }

    private void Start()
    {
        m_currentSpawnName = PlayerPrefs.GetString(saveSpawnName, "");
        DOVirtual.DelayedCall(0.1f, MovePlayerToSpawn);
    }

    public void RegisterCkeckpoint(CheckpointInteractable checkpoint)
    {
        var it = m_checkpoints.Find(x => { return x.GetID() == checkpoint.GetID(); });
        if(it != null)
        {
            Debug.LogError("Two checkpoints have the same ID : " + checkpoint.GetID());
        }
        m_checkpoints.Add(checkpoint);
    }

    public void MovePlayerToSpawn()
    {
        if (m_currentSpawnName == "")
            return;

        var it = m_checkpoints.Find(x => { return x.GetID() == m_currentSpawnName; });
        if (it == null)
        {
            Debug.Log("Cannot find the saved checkpoint " + m_currentSpawnName);
            return;
        }

        it.EnablePoint(true);

        var pTransform = PlayerControler.instance.transform;
        pTransform.position = new Vector3(it.transform.position.x, it.transform.position.y, pTransform.position.z);

        Event<CameraInstantMoveEvent>.Broadcast(new CameraInstantMoveEvent(pTransform.position));
    }

    public void ResetSpawn()
    {
        m_currentSpawnName = "";
        PlayerPrefs.SetString(saveSpawnName, "");
    }

    public void SetCurrentCheckpoint(string name)
    {
        var oldPoint = m_checkpoints.Find(x => { return x.GetID() == m_currentSpawnName; });
        if (oldPoint != null)
            oldPoint.EnablePoint(false);

        m_currentSpawnName = name;
        PlayerPrefs.SetString(saveSpawnName, m_currentSpawnName);
    }
}
