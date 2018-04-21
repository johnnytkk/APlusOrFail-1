using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace APlusOrFail.Maps
{
    using Objects;
    using Components.AutoResizeCamera;
    using SceneStates.DefaultSceneState;

    public abstract class MapManagerBehavior : MonoBehaviour, IMapManager
    {
        protected class MapSetting : IMapSetting
        {
            public string name { get; }
            public IReadOnlyList<IRoundSetting> roundSettings { get; }
            public MapArea mapArea { get; }
            public AutoResizeCamera camera { get; }

            public MapSetting(string name, IEnumerable<IRoundSetting> roundSettings, MapArea mapArea, AutoResizeCamera camera)
            {
                this.name = name;
                this.roundSettings = new ReadOnlyCollection<IRoundSetting>(roundSettings.ToList());
                this.mapArea = mapArea;
                this.camera = camera;
            }
        }

        protected class RoundSetting : IRoundSetting
        {
            public string name { get; }
            public int roundScore { get; }
            public IReadOnlyList<ObjectPrefabInfo> usableObjects { get; }
            public ObjectGridPlacer spawnArea { get; }

            public RoundSetting(string name, int roundScore, IEnumerable<ObjectPrefabInfo> usableObjects, ObjectGridPlacer spawnArea)
            {
                this.name = name;
                this.roundScore = roundScore;
                this.usableObjects = new ReadOnlyCollection<ObjectPrefabInfo>(usableObjects.ToList());
                this.spawnArea = spawnArea;
            }
        }


        public DefaultSceneState defaultSceneState;
        public string mapName;
        public MapArea mapArea;
        public AutoResizeCamera camera;
          
        public IMapStat stat { get; protected set; }


        protected virtual void Awake()
        {
            if (((IMapManager)this).Register())
            {
                stat = new MapStat(GetMapSetting());
            }
            else
            {
                Debug.LogErrorFormat("There is another map manager already!");
                Destroy(this);
            }
        }

        protected virtual void Start()
        {
            FindObjectOfType<SceneStateManager>().Push(defaultSceneState, stat);
        }

        protected virtual void OnDestroy()
        {
            ((IMapManager)this).Unregister();
        }

        protected virtual IMapSetting GetMapSetting() => new MapSetting(mapName, GetRoundSettings(), mapArea, camera);

        protected abstract IEnumerable<IRoundSetting> GetRoundSettings();
    }
}
