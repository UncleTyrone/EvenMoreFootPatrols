using MelonLoader;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using EvenMoreFootPatrols;


#if MONO
using FishNet.Managing;
using FishNet.Managing.Object;
using FishNet.Object;
using ScheduleOne.AvatarFramework;
using ScheduleOne.NPCs;
using ScheduleOne.NPCs.Behaviour;
using ScheduleOne.Police;
#else
using Il2CppFishNet.Managing;
using Il2CppFishNet.Managing.Object;
using Il2CppFishNet.Object;
using Il2CppScheduleOne.AvatarFramework;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.NPCs.Behaviour;
using Il2CppScheduleOne.Police;
#endif


namespace EvenMoreFootPatrols.PatrolOfficers
{
    public class PatrolOfficers
    {
        private static AvatarSettings getRandomOfficerAvatarSettings()
        {
            PoliceOfficer policeOfficer = EvenMoreFootPatrols._officers[UnityEngine.Random.Range(0, EvenMoreFootPatrols._officers.Length)];
            return policeOfficer.Avatar.CurrentSettings;
        }
        public static void spawnOfficer(NetworkManager manager, string name, PatrolGroup group, float movementSpeedMult, bool warpToStart)
        {
            PrefabObjects spawnablePrefabs = manager.SpawnablePrefabs;
            NetworkObject networkObject = new NetworkObject();
            for (int i = 0; i < spawnablePrefabs.GetObjectCount(); i++)
            {
                NetworkObject @object = spawnablePrefabs.GetObject(true, i);
                bool flag = @object.gameObject == null;
                if (!flag)
                {
                    bool flag2 = @object.gameObject.name == "PoliceNPC";
                    if (flag2)
                    {
                        networkObject = @object;
                    }
                }
            }
            NetworkObject networkObject2 = UnityEngine.Object.Instantiate<NetworkObject>(networkObject);
            AvatarSettings randomOfficerAvatarSettings = getRandomOfficerAvatarSettings();
            networkObject2.gameObject.name = name;
            NPCMovement component = networkObject2.gameObject.GetComponent<NPCMovement>();
            component.MoveSpeedMultiplier = movementSpeedMult;
            PoliceOfficer component2 = networkObject2.gameObject.GetComponent<PoliceOfficer>();
            component2.Avatar.SetSkinColor(randomOfficerAvatarSettings.SkinColor);
            component2.Avatar.ApplyBodyLayerSettings(randomOfficerAvatarSettings, -1);
            component2.Avatar.ApplyBodySettings(randomOfficerAvatarSettings);
            component2.Avatar.ApplyEyeBallSettings(randomOfficerAvatarSettings);
            component2.Avatar.ApplyAccessorySettings(randomOfficerAvatarSettings);
            component2.Avatar.ApplyEyeLidColorSettings(randomOfficerAvatarSettings);
            component2.Avatar.ApplyEyeLidSettings(randomOfficerAvatarSettings);
            component2.Avatar.ApplyEyebrowSettings(randomOfficerAvatarSettings);
            component2.Avatar.ApplyFaceLayerSettings(randomOfficerAvatarSettings);
            component2.Avatar.ApplyHairColorSettings(randomOfficerAvatarSettings);
            component2.Avatar.ApplyHairSettings(randomOfficerAvatarSettings);
            component2.Avatar.ApplyShapeKeys(randomOfficerAvatarSettings.Gender, randomOfficerAvatarSettings.Weight, false);
            component2.StartFootPatrol(group, warpToStart);
            MelonCoroutines.Start(ActivateNetworkObjectAfterDelay(networkObject2, 0.3f));
            MelonCoroutines.Start(SpawnNetworkObjectAfterDelay(manager, networkObject2, 0.6f));
        }

        private static IEnumerator ActivateNetworkObjectAfterDelay(NetworkObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            bool flag = obj != null;
            if (flag)
            {
                obj.gameObject.SetActive(true);
            }
            else
            {
                MelonLogger.Msg("Object to activate is null.");
            }
            yield break;
        }

        private static IEnumerator SpawnNetworkObjectAfterDelay(NetworkManager manager, NetworkObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            bool flag = obj != null;
            if (flag)
            {
                manager.ServerManager.Spawn(obj, null, default(Scene));
            }
            else
            {
                MelonLogger.Msg("Object to spawn is null.");
            }
            yield break;
        }
    }
}