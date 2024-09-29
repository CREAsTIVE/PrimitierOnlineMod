using Il2CppInterop.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace YuchiGames.POM.Client.Managers
{
    
    public class CubeSyncerComponent : MonoBehaviour
    {
        public CubeSyncerComponent(IntPtr ptr) : base(ptr) { }

        public bool Host { get; set; } = true;

        public void FixedUpdate()
        {

        }
    }
}
