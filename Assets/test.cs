using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StreamEncryption.DecryptAssetBundle(Application.streamingAssetsPath + "/assets/cube.ab", "Cube",(prefab)=>
        {
            var obj = GameObject.Instantiate<GameObject>(prefab as GameObject);
            obj.transform.localPosition = new Vector3(0, 1, 0);
        });

        StartCoroutine(StreamEncryption.AsyncLoad(Application.streamingAssetsPath + "/assets/cube.ab", "Cube", (prefab) =>
        {
            var obj = GameObject.Instantiate<GameObject>(prefab as GameObject);
            obj.transform.localPosition = new Vector3(0, -1, 0);
        }));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
