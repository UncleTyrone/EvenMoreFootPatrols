using MelonLoader;
using System.Collections;
using UnityEngine;
using Newtonsoft.Json;
using EvenMoreFootPatrols.Helpers;
using EvenMoreFootPatrols.PatrolConfiguration;
using EvenMoreFootPatrols.PatrolRoutes;
using EvenMoreFootPatrols.PatrolOfficers;


#if MONO
using FishNet;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Managing.Object;
using ScheduleOne.Police;
using ScheduleOne.NPCs;
using ScheduleOne.NPCs.Behaviour;
using ScheduleOne.AvatarFramework;
#else
using Il2CppFishNet;
using Il2CppFishNet.Managing;
using Il2CppFishNet.Object;
using Il2CppFishNet.Managing.Object;
using Il2CppScheduleOne.Police;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.NPCs.Behaviour;
using Il2CppScheduleOne.AvatarFramework;
using Il2CppInterop.Runtime;
#endif


[assembly: MelonInfo(
    typeof(EvenMoreFootPatrols.EvenMoreFootPatrols),
    EvenMoreFootPatrols.BuildInfo.Name,
    EvenMoreFootPatrols.BuildInfo.Version,
    EvenMoreFootPatrols.BuildInfo.Author
)]
[assembly: MelonColor(1, 0, 0, 255)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace EvenMoreFootPatrols
{
    public static class BuildInfo
    {
        public const string Name = "Even More Foot Patrols";
        public const string Description = "Even More Foot Patrols!";
        public const string Author = "UncleTyrone";
        public const string Version = "1.0.3";
    }

    public class EvenMoreFootPatrols : MelonMod
    {
        private static MelonLogger.Instance Logger;

        public override void OnInitializeMelon()
        {
            Logger = LoggerInstance;
            Logger.Msg("Even More Foot Patrols has been initialized!");
            Logger.Debug("This will only show in debug mode");
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            Logger.Debug($"Scene loaded: {sceneName}");
            if (sceneName == "Main")
            {
                Logger.Debug("Main scene loaded, waiting for network");
                MelonCoroutines.Start(Utils.WaitForNetwork(DoNetworkStuff()));
            }
        }

        private IEnumerator DoNetworkStuff()
        {
            var nm = InstanceFinder.NetworkManager;
            if (nm.IsServer && nm.IsClient)
            {
                Logger.Debug("Starting patrols from the host!");
                _officers = UnityEngine.Object.FindObjectsOfType<PoliceOfficer>(true);
                MelonCoroutines.Start(PatrolConfiguration.PatrolConfiguration.LoadPatrolRoutesFromJson(nm));
            }
            else if (!nm.IsServer && !nm.IsClient)
            {
                Logger.Debug("Starting patrols in singleplayer!");
                _officers = UnityEngine.Object.FindObjectsOfType<PoliceOfficer>(true);
                MelonCoroutines.Start(PatrolConfiguration.PatrolConfiguration.LoadPatrolRoutesFromJson(nm));
            }
            else if (nm.IsClient && !nm.IsServer)
                Logger.Debug("Client-only");
            else if (nm.IsServer && !nm.IsClient)
            {
                Logger.Debug("Starting patrols from the server!");
                _officers = UnityEngine.Object.FindObjectsOfType<PoliceOfficer>(true);
                MelonCoroutines.Start(PatrolConfiguration.PatrolConfiguration.LoadPatrolRoutesFromJson(nm));
            }
            yield return null;
        }

        public override void OnUpdate()
        {
            bool keyDown = Input.GetKeyDown(UnityEngine.KeyCode.F6);
            if (keyDown)
            {
                LogPlayerCoordinates();
            }
        }

        private static void LogPlayerCoordinates()
        {
            GameObject gameObject = GameObject.Find("Player_Local");
            bool flag = gameObject != null;
            if (flag)
            {
                Transform transform = gameObject.transform;
                Vector3 position = transform.position;
                MelonLogger.Msg($"{{ \"x\": {position.x}, \"y\": {position.y}, \"z\": {position.z} }}");
            }
            else
            {
                MelonLogger.Msg("Player_Local component is missing, cannot fetch coordinates.");
            }
        }

        public static PoliceOfficer[] _officers;

        [Serializable]
        public class PatrolRouteData
        {
            [JsonProperty("routes")]
            public List<Route> Routes;
        }

        [Serializable]
        public class Route
        {
            [JsonProperty("routeName")]
            public string RouteName;

            [JsonProperty("waypointsWrapper")]
            public WaypointsWrapper WaypointsWrapper;

            [JsonProperty("MinOfficers")]
            public int MinOfficers;

            [JsonProperty("MaxOfficers")]
            public int MaxOfficers;

            [JsonProperty("OfficerMovementSpeedMultiplier")]
            public float OfficerMovementSpeedMultiplier;
        }

        [Serializable]
        public class WaypointsWrapper
        {
            [JsonProperty("waypoints")]
            public List<Waypoint> Waypoints;
        }

        [Serializable]
        public class Waypoint
        {
            [JsonProperty("x")]
            public float X;

            [JsonProperty("y")]
            public float Y;

            [JsonProperty("z")]
            public float Z;
        }
    }
}