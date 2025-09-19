using MelonLoader;
using MelonLoader.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;
using static EvenMoreFootPatrols.EvenMoreFootPatrols;
using EvenMoreFootPatrols.PatrolRoutes;
using EvenMoreFootPatrols.PatrolOfficers;


#if MONO
using FishNet.Managing;
using ScheduleOne.NPCs.Behaviour;
#else
using Il2CppFishNet.Managing;
using Il2CppScheduleOne.NPCs.Behaviour;
#endif


namespace EvenMoreFootPatrols.PatrolConfiguration
{
    public class PatrolConfiguration
    {
        public static IEnumerator LoadPatrolRoutesFromJson(NetworkManager manager)
        {
            string path = Path.Combine(MelonEnvironment.UserDataDirectory, "routes.json");
            if (!File.Exists(path))
            {
                MelonLogger.Msg("JSON file not found at path: " + path);
                yield break;
            }

            string json = File.ReadAllText(path);

            PatrolRouteData patrolRouteData = null;
            try
            {
                var settings = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };
                patrolRouteData = JsonConvert.DeserializeObject<PatrolRouteData>(json, settings);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Failed to deserialize JSON: {ex}");
                yield break;
            }

            if (patrolRouteData?.Routes == null || patrolRouteData.Routes.Count == 0)
            {
                MelonLogger.Msg("No routes found in JSON data.");
                yield break;
            }

            float delayBetweenRoutes = 5f;

            foreach (Route route in patrolRouteData.Routes)
            {
                List<Vector3> waypoints = new List<Vector3>();
                foreach (Waypoint waypoint in route.WaypointsWrapper.Waypoints)
                {
                    waypoints.Add(new Vector3(waypoint.X, waypoint.Y, waypoint.Z));
                }

                PatrolGroup group = PatrolRoutes.PatrolRoutes.CreatePatrolGroup(route.RouteName, waypoints);

                int officerCount = UnityEngine.Random.Range(route.MinOfficers, route.MaxOfficers + 1);
                for (int i = 0; i < officerCount; i++)
                {
                    string officerName = $"{route.RouteName}_Officer{i + 1}";
                    PatrolOfficers.PatrolOfficers.spawnOfficer(manager, officerName, group, route.OfficerMovementSpeedMultiplier, true);
                    yield return new WaitForSeconds(0.5f);
                }

                MelonLogger.Msg($"{route.RouteName} route created successfully with {officerCount} officers");

                yield return new WaitForSeconds(delayBetweenRoutes);
            }
        }
    }
}