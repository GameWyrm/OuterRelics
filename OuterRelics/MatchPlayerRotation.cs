using System.Collections;
using UnityEngine;

namespace OuterRelics
{
    public class MatchPlayerRotation : MonoBehaviour
    {
        GameObject player = Locator.GetPlayerBody().gameObject;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (PlayerState.InZeroG())
            {
                transform.rotation = player.transform.rotation;
            }
        }
    }
}