using MelonLoader;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using EvenMoreFootPatrols;


#if MONO
using ScheduleOne.NPCs.Behaviour;
#else
using Il2CppScheduleOne.NPCs.Behaviour;
#endif


namespace EvenMoreFootPatrols.PatrolRoutes
{
    public class PatrolRoutes
    {
        public static PatrolGroup CreatePatrolGroup(string routeName, List<Vector3> waypoints)
        {
            GameObject gameObject = GameObject.Find("PatrolRoutes");
            bool flag = gameObject == null;
            PatrolGroup result;
            if (flag)
            {
                MelonLogger.Msg("Patrol group object not found");
                result = null;
            }
            else
            {
                GameObject patrolRouteObject = new GameObject(routeName);
                FootPatrolRoute footPatrolRoute = patrolRouteObject.AddComponent<FootPatrolRoute>();
                footPatrolRoute.RouteName = routeName;
                footPatrolRoute.StartWaypointIndex = 0;
                patrolRouteObject.transform.position = waypoints[0];
                patrolRouteObject.transform.SetParent(gameObject.transform);
                List<GameObject> list = new List<GameObject>();
                foreach (Vector3 position in waypoints)
                {
                    GameObject gameObject2 = new GameObject("Waypoint");
                    gameObject2.transform.position = position;
                    gameObject2.transform.SetParent(patrolRouteObject.transform);
                    list.Add(gameObject2);
                }
                Transform[] source = patrolRouteObject.GetComponentsInChildren<Transform>();
                footPatrolRoute.Waypoints = (from t in source
                                             where t != patrolRouteObject.transform
                                             select t).ToArray<Transform>();
                footPatrolRoute.UpdateWaypoints();
                PatrolGroup patrolGroup = new PatrolGroup(footPatrolRoute);
                result = patrolGroup;
            }
            return result;
        }
    }
}