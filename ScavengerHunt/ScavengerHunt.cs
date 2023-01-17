using OWML.Common;
using OWML.ModHelper;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ScavengerHunt
{
    public class ScavengerHunt : ModBehaviour
    {
        public AssetBundle assets;

        GameObject orb;
        List<GameObject> positionalIndicators;
        NomaiInterfaceOrb orbInterface;
        bool searching;
        bool inGame;
        int indicatorIndex = 0;
        float graceTimer = 0f;
        

        private void Awake()
        {
            // You won't be able to access OWML's mod helper in Awake.
            // So you probably don't want to do anything here.
            // Use Start() instead.
            positionalIndicators = new List<GameObject>();
        }

        private void Start()
        {
            // Starting here, you'll have access to OWML's mod helper.
            ModHelper.Console.WriteLine($"My mod {nameof(ScavengerHunt)} is loaded!", MessageType.Success);

            assets = ModHelper.Assets.LoadBundle("Models/scavengerhuntassets");


            // Example of accessing game code.
            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                if (loadScene != OWScene.SolarSystem)
                {
                    inGame = false;
                    return;
                }
                ModHelper.Console.WriteLine("Loaded into solar system!", MessageType.Success);
                LoadIn();
                inGame = true;
            };
        }

        private void Update()
        {
            if (!inGame) return;
            if (searching)
            {
                GameObject TLRI = GameObject.Find("Interactibles_TimeLoopRing_Hidden");
                if (TLRI == null)
                {
                    if (graceTimer > 10f)
                    {
                        ModHelper.Console.WriteLine("Failed to locate the orb parent object within the allotted time.", MessageType.Warning);
                        searching = false;
                    }
                    graceTimer += Time.deltaTime;
                }
                else
                {
                    ModHelper.Console.WriteLine("Found the orb parent: " + TLRI.name, MessageType.Success);
                    
                    orb = TLRI.transform.Find("CoreContainmentInterface/Prefab_NOM_InterfaceOrb").gameObject;
                    if (orb != null)
                    {
                        ModHelper.Console.WriteLine("Found the orb", MessageType.Info);
                    }
                    else
                    {
                        ModHelper.Console.WriteLine("Couldn't find it...", MessageType.Error);
                        searching = false;
                        return;
                    }
                    orbInterface = orb.GetComponent<NomaiInterfaceOrb>();
                    orbInterface.AddLock();
                    GameObject orbLock = Instantiate(assets.LoadAsset<GameObject>("Orb Lock"), orb.transform);
                    orbLock.transform.position = orb.transform.position;
                    orbLock.transform.localScale = Vector3.one * 0.55f;
                    ModHelper.Console.WriteLine("Locked the orb!", MessageType.Success);

                    searching = false;
                }
            }

            if (Keyboard.current[Key.J].wasPressedThisFrame)
            {
                GrabObjectPosition();
            }
        }

        private void GrabObjectPosition()
        {
            Transform player = Locator.GetPlayerCamera().transform;
            Transform playerBody = Locator.GetPlayerBody().transform;
            LayerMask mask = LayerMask.GetMask("Default", "IgnoreSun");
            Physics.Raycast(player.position, player.TransformDirection(Vector3.forward), out RaycastHit hit, 1000f, mask); //Ignore layer 8!
            Collider collider = hit.collider;
            Vector3 relativePos = collider.transform.InverseTransformPoint(player.position);

            string hitname = hit.collider.name;
            GameObject go = hit.collider.gameObject;
            while (go.transform.parent != null)
            {
                hitname = go.transform.parent.name + "/" + hitname;
                go = go.transform.parent.gameObject;
            }
            
            if (positionalIndicators.Count < 5)
            {
                GameObject indicator;
                indicator = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                indicator.GetComponent<MeshRenderer>().material = assets.LoadAsset<Material>("Uncollected");
                indicator.GetComponent<CapsuleCollider>().enabled = false;
                indicator.transform.localScale = Vector3.one * 0.5f;
                positionalIndicators.Add(indicator);
                ModHelper.Console.WriteLine("Created new indicator", MessageType.Info);
            }
            positionalIndicators[indicatorIndex].transform.position = hit.point;
            positionalIndicators[indicatorIndex].transform.rotation = playerBody.rotation;
            positionalIndicators[indicatorIndex].transform.parent = hit.collider.transform;

            ModHelper.Console.WriteLine("Collider found: " + hitname + ", \nPosition: " + relativePos.ToString() + ", \nRotation: " + positionalIndicators[indicatorIndex].transform.localEulerAngles);

            indicatorIndex++;
            if (indicatorIndex >= 5) indicatorIndex = 0;
        }

        private void LoadIn()
        {
            searching = true;
            GameObject atp = GameObject.Find("TimeLoopRing_Body");
            ModHelper.Console.WriteLine(atp == null ? "Could not find it yet" : "Located ATP: " + atp.name, MessageType.Info);

            GameObject mask = new GameObject();
            mask.transform.parent = atp.transform.Find("Geo_TimeLoopRing/BatchedGroup/BatchedMeshColliders_0");
            mask.transform.localPosition = new Vector3(-23.4f, 11.4f, 0f);
            mask.transform.localEulerAngles = new Vector3(64f, 270f, 180f);
            LockManager lm = mask.AddComponent<LockManager>();
            lm.main = this;
        }
    }
}