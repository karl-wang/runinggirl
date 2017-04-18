using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
    public GameObject trackObj;
    public Transform trackContainer;
    public Transform roleTransform;
    public int cacheCount = 5;
    public GameObject[] tracks;
    private int curTrackIndex = 0;

    private GameObject newTrack(int index)
    {
        Vector3 pos = tracks[index - 1].GetComponent<Track>().end.transform.position;
        return Instantiate(trackObj, pos, Quaternion.identity, trackContainer);
    }

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < cacheCount; i++)
        {
            if (tracks[i]) continue;
            tracks[i] = newTrack(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (tracks[1].GetComponent<Track>().isFinish) {
            Destroy(tracks[0]);
            for (var i = 1; i < cacheCount; i++)
            {
                tracks[i - 1] = tracks[i];
            }
            tracks[cacheCount - 1] = newTrack(cacheCount - 1);
        }
    }
}
