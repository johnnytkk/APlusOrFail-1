using UnityEngine;
using UnityEngine.SceneManagement;

namespace APlusOrFail.Setup.MapPlatform
{
    using MapSwitchCounter;

    public class AutoEnterMap : MonoBehaviour
    {
        public AutoPress autoPress;
        public string targetScenePath;

        private int targetSceneBuildIndex;
        private MapSwitchCounterCanvas.PendingSwitching pendingSwitching;


        private void Start()
        {
            autoPress.onCharacterCount += OnPressingCharacterCountChanged;
            targetSceneBuildIndex = SceneUtility.GetBuildIndexByScenePath(targetScenePath);
            if (targetSceneBuildIndex < 0)
            {
                Debug.LogErrorFormat($"Cannot find scene at path: {targetScenePath}");
            }
        }

        
        private void OnPressingCharacterCountChanged(AutoPress autoPress, int count)
        {
            if (count > 0)
            {
                if (pendingSwitching == null)
                {
                    pendingSwitching = MapSwitchCounterCanvas.instance.ScheduleSwitch(targetSceneBuildIndex);
                }
                pendingSwitching.priority = count;
            }
            else if (count == 0 && pendingSwitching != null)
            {
                pendingSwitching.Cancel();
                pendingSwitching = null;
            }
        }
    }
}
