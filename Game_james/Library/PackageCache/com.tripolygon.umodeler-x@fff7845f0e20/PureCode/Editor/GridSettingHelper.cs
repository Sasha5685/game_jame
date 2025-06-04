namespace Packages.Unity_UModelerX.PureCode.Editor
{
    public class GridSettingHelper
    {
        [UnityEditor.InitializeOnLoadMethod]
        private static void InitGridSnapSetting()
        {
            Tripolygon.UModelerX.Editor.Views.GridSnapping.GridSnapSettings.SetSnapEnable += SetSnapEnable;
        }

        private static void SetSnapEnable(bool enable)
        {
#if UNITY_2023_2_OR_NEWER
            UnityEditor.EditorSnapSettings.snapEnabled = enable;
#else
            UnityEditor.EditorSnapSettings.gridSnapEnabled = enable;
#endif
        }
    }
}
