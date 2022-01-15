//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using UnityEngine;

namespace DADemo_Sample_Utils
{
    public class AADisableChecker : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            bool active = false;
            if (QualitySettings.antiAliasing > 0)
            {
                active = true;
            }
            gameObject.SetActive(active);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
