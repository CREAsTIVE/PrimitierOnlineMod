using Il2Cpp;
using Il2CppInterop.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YuchiGames.POM.Shared;
using YuchiGames.POM.Shared.DataObjects;

namespace YuchiGames.POM.Client.Managers
{
    
    public class GroupSyncerComponent : MonoBehaviour
    {
        public GroupSyncerComponent(IntPtr ptr) : base(ptr) { }

        public int HostID = Network.ID;
        public ObjectUID UID = Network.NextObjectUID();
        float _timePass = 0f;
        static float NeedTime => 0.25f;

        public RigidbodyManager RBM = null!;

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
        }

        public void OnDestroy()
        {
            if (Network.ID < 0)
                return;
            Network.Send(new GroupDestroyedMessage()
            {
                GroupID = UID,
            });
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
                

            _timePass += Time.deltaTime;
            if (_timePass < NeedTime) return;
            _timePass %= NeedTime;

            if (HostID != Network.ID) return;

            Network.Send(new GroupUpdateMessage()
            {
                GroupData = Network.GetGroupValues(RBM),
                GroupUID = UID,
            });
        }
    }
}
