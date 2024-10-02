using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YuchiGames.POM.Shared;
using YuchiGames.POM.Shared.DataObjects;
using YuchiGames.POM.Shared.Utils;
using static Il2Cpp.CubeGenerator;

namespace YuchiGames.POM.Client.Managers
{
    
    public class GroupSyncerComponent : MonoBehaviour
    {
        public GroupSyncerComponent(IntPtr ptr) : base(ptr) { }

        public int HostID = Network.ID;
        public ObjectUID UID = Network.NextObjectUID();
        float _timePastSinceUpdate = 0f;

        public RigidbodyManager RBM = null!;

        bool _unloaded = false;

        public void Unload()
        {
            _unloaded = true;
            Network.Send(new GroupSetHostMessage() { GroupID = UID, NewHostID = -1 });
        }

        public void Start()
        {
            if (Network.ID < 0)
            {
                GameObject.Destroy(this);
                return;
            }

            Network.SyncedObjects[UID] = this;

            RBM = GetComponent<RigidbodyManager>();
            gameObject.name += $"[SYNCED {UID}]";

            if (Network.IsConnected)
                InserAwaitForUpdateObjectSorted(this);
        }

        public void OnDestroy()
        {
            if (Network.ID < 0)
                return;
            if (!_unloaded)
                Network.Send(new GroupDestroyedMessage
                    {
                        GroupID = UID,
                    }
                );
            Network.SyncedObjects.Remove(UID);
        }
        
        public void Update()
        {
            if (RBM == null)
            {
                RBM = GetComponent<RigidbodyManager>();
                Log.Warning($"RigidbodyManager of GO {gameObject.name} didn't exist!");
                return;
            }
                

            /*_timePass += Time.deltaTime;
            if (_timePass < NeedTime) return;
            _timePass %= NeedTime;

            if (HostID != Network.ID) return;

            Network.Send(new GroupUpdateMessage()
            {
                GroupData = Network.GetGroupValues(RBM),
                GroupUID = UID,
            });*/
        }

        class SyncerTask
        {
            public float TimePasses = 0f;
            public float TimeBetweenUpdates;
            public Action<IEnumerable<GroupSyncerComponent>> ActualUpdate = delegate { };
        }

        // Radius of objects, that will be synced with other players
        public static float WorldUpdateDistance { get; set; }
        public static float WorldQuickUpdateDistance { get; set; } // FETCH THAT FROM SERVER!!

        private static LinkedList<(GroupSyncerComponent component, float distance)> s_awaitsForUpdateObjects = new();

        static float TakeMinDistance(Vector3 pos) =>
            Player.ActivePlayers.Any() ? Player.ActivePlayers
                .Select(pair => Vector3.Distance(pos, pair.Value.PlayerLastPositionData.ToUnity()))
                .Min() : 0;

        static void InserAwaitForUpdateObjectSorted(GroupSyncerComponent component)
        {
            if (!s_awaitsForUpdateObjects.Any())
            {
                s_awaitsForUpdateObjects.AddLast((component, TakeMinDistance(component.gameObject.transform.position)));
                return;
            }

            var dist = TakeMinDistance(component.transform.position);

            var current = s_awaitsForUpdateObjects.First;
            while (current != null && current.Value.distance < dist)
                current = current.Next;

            if (current == null)
                s_awaitsForUpdateObjects.AddLast((component, dist));
            else
                s_awaitsForUpdateObjects.AddBefore(current, (component, dist));
        }

        private static List<SyncerTask> s_tasks = new()
        {
            new() // Resort everything once in a second
            {
                TimeBetweenUpdates = 1f,
                ActualUpdate = groups =>
                {
                    var tlist = s_awaitsForUpdateObjects.ToList();
                    tlist.Sort((a, b) => a.Item2 < b.Item2 ? -1 : 1);
                    s_awaitsForUpdateObjects = new(tlist);
                }
            },
            new() // All data update
            {
                TimeBetweenUpdates = 1f/100f,
                ActualUpdate = (groups) =>
                {
                    if (!s_awaitsForUpdateObjects.Any())
                    {
                        Log.Information("No forced sync required. Collecting all groups...");

                        var tlist = groups.Select(obj => (obj, TakeMinDistance(obj.transform.position))).ToList();
                        tlist.Sort((a, b) => a.Item2 < b.Item2 ? -1 : 1);
                        s_awaitsForUpdateObjects = new(tlist);

                        Log.Information($"Collected {tlist.Count} groups");
                    }

                    if (!s_awaitsForUpdateObjects.Any()) return;

                    var nodeForUpdate = s_awaitsForUpdateObjects.First;

                    while (nodeForUpdate != null && nodeForUpdate.Value.component == null)
                        nodeForUpdate = nodeForUpdate.Next;

                    if (nodeForUpdate == null)
                        return;

                    var elementForUpdate = nodeForUpdate.Value.component;

                    elementForUpdate.RBM?.Apply(rbm =>
                        Network.Send(new GroupUpdateMessage()
                        {
                            GroupData = Network.GetGroupValues(rbm),
                            GroupUID = elementForUpdate.UID,
                        })
                    );

                    s_awaitsForUpdateObjects.RemoveFirst();


                        /*Log.Information("Collecting all groups...");

                        var objectsDistances = groups
                            .Select(obj => (obj, Player.ActivePlayers
                                .Select(pair => Vector3.Distance(obj.gameObject.transform.position, pair.Value.PlayerLastPositionData.ToUnity()))
                                .Min()))
                            // .Where((pair) => pair.Item2 < MaxDistanceForUpdate)
                            .ToList();

                        objectsDistances.Sort((a, b) => a.Item2 < b.Item2 ? -1 : 1);

                        s_awaitsForUpdateObjects = new(objectsDistances.Select(obj => obj.obj));

                        Log.Information($"Collected {objectsDistances.Count} groups.");*/

                    /*if (!s_awaitsForUpdateObjects.Any()) return;

                    var elementForUpdate = s_awaitsForUpdateObjects[0];

                    if (elementForUpdate == null) return;

                    elementForUpdate.RBM?.Apply(rbm => 
                        Network.Send(new GroupUpdateMessage()
                        {
                            GroupData = Network.GetGroupValues(rbm),
                            GroupUID = elementForUpdate.UID,
                        })
                    );

                    s_awaitsForUpdateObjects.RemoveAt(0);*/
                }
            },
            new() // Quick update (position + rotation + velocity + angular velocity)
            {
                TimeBetweenUpdates = 0f,
                ActualUpdate = groups =>
                {
                    var sorted = ((IEnumerable<GroupSyncerComponent>)groups)
                        .Select(obj => (obj, Player.ActivePlayers
                            .Select(pair => Vector3.Distance(obj.gameObject.transform.position, pair.Value.PlayerLastPositionData.ToUnity()))
                            .Min()))
                        .Where(obj => obj.Item2 < WorldQuickUpdateDistance)
                        .Select(obj => obj.obj)
                        .Select(obj => (obj, obj.GetComponent<RigidbodyManager>()))
                        .Where(obj => obj.Item2 != null);

                    foreach (var e in sorted)
                        Network.Send(Network.GetQuickGroupValues(e.Item2).Apply(values => values.ObjectUID = e.obj.UID));

                    return;
                }
            }
        };

        public static void GlobalUpdate()
        {
            if (!Network.IsConnected) return;

            if (Player.ActivePlayers.Count <= 0) return;

            foreach (var task in s_tasks)
            {
                task.TimePasses += Time.deltaTime;

                if (task.TimePasses > task.TimeBetweenUpdates)
                {
                    if (task.TimeBetweenUpdates > 0)
                        task.TimePasses %= task.TimeBetweenUpdates;

                    var groups = GameObject.FindObjectsOfTypeAll(Il2CppType.Of<GroupSyncerComponent>())
                        .Select(obj => obj.Cast<GroupSyncerComponent>())
                        .Where(obj => obj.HostID == Network.ID);

                    task.ActualUpdate(groups);
                }
            }
        }
    }
}
