using Tripolygon.UModelerX.Editor;

namespace Tripolygon.UModelerX.Runtime.ProBuilderChecker
{
    public class InputSystemChecker
    {
#if INPUTSYSTEM && UNITY_EDITOR
        private static UnityEngine.InputSystem.Pen pen;

        // 0.20.6 버전에서 미작동 처리
        [UnityEditor.InitializeOnLoadMethod]
        private static void InputSystemCheckerIniitialize()
        {
            //Tripolygon.UModelerX.Editor.PenController.Enable();
            Tripolygon.UModelerX.Editor.ExternPackages.ExternLibarary.Add("InputSystemEnabled", InputSystemEnabled);
            Tripolygon.UModelerX.Editor.ExternPackages.ExternLibarary.Add("ReadPenInput", ReadPenInput);
        }

        private static bool InputSystemEnabled()
        {
            if (UnityEngine.InputSystem.InputSystem.devices.Count > 0)
            {
#if UNITY_EDITOR_WIN
                PenController.DetectTabletDevicesOnWindow();
#elif UNITY_EDITOR_OSX
                PenController.DetectTabletDevicesOnOSX();
#elif UNITY_EDITOR_LINUX
                PenController.DetectTabletDevicesOnLinux();
#endif
                return true;
            }
            return false;
        }

        private static bool ReadPenInput()
        {
            if (pen == null || PaintBrushSettings.PenConnected == false)
            {
                var penDevice = UnityEngine.InputSystem.InputSystem.GetDevice("Pen");
                pen = penDevice as UnityEngine.InputSystem.Pen;
            }

            if (pen != null)
            {
                Tripolygon.UModelerX.Editor.PenController.Update(pen.press.ReadValue(), pen.pressure.ReadValue(), pen.position.ReadValue());
            }

            return pen != null;
        }

        private static string OnFindLayoutForDevice(ref UnityEngine.InputSystem.Layouts.InputDeviceDescription description, string matchedLayout, UnityEngine.InputSystem.LowLevel.InputDeviceExecuteCommandDelegate executeDeviceCommand)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"Device Connected: {description.product}");
            sb.AppendLine($"Manufacturer: {description.manufacturer}");
            sb.AppendLine($"Interface: {description.interfaceName}");
            sb.AppendLine($"Matched Layout: {matchedLayout ?? "None"}");

            // Unsupported 장치 탐지
            if (string.IsNullOrEmpty(matchedLayout))
            {
                sb.AppendLine("This is an Unsupported device.");
            }
            UnityEngine.Debug.Log(sb.ToString());
            return null; // Unity의 기본 레이아웃 매칭 진행
        }
#endif
    }
}
