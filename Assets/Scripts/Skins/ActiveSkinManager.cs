using Skins.Data;
using UnityEngine;

namespace Skins
{
    public class ActiveSkinManager : MonoBehaviour
    {
        public static ActiveSkinManager Instance { get; private set; }
        
        public SkinData CurrentSkin { get; private set; }

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void SetActiveSkin(SkinData skinData) => CurrentSkin = skinData;
    }
}