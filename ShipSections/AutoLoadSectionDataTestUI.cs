using UnityEngine;

namespace JKorTech.ShipSections
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class AutoLoadSectionDataTestUI : KSPPluginFramework.MonoBehaviourWindow
    {
        private const int WindowWidth = 400, WindowHeight = 200;

        private readonly string AppLauncherIconLocation = "ShipSections/ShipSections";

        internal override void DrawWindow(int id)
        {
            if (API.AnyCurrentVesel)
            {
                DrawSectionWindow();
            }
            else
            {
                GUILayout.Label("Create a craft to use ShipSections.");
            }
        }

        private void DrawSectionWindow()
        {
            using (new GuiLayout(GuiLayout.Method.Vertical))
            using (new GuiLayout(GuiLayout.Method.ScrollView, ref scrollPos))
            {
                foreach (var section in API.SectionNames)
                {
                    using (new GuiLayout(GuiLayout.Method.Horizontal))
                    {
                        GUILayout.Label(section);
                    }
                    using (new GuiLayout(GuiLayout.Method.Horizontal))
                    {
                        GUILayout.Label("Data:");
                        GUILayout.Label(API.GetSectionDataForMod<SectionDataWithInfo>(section).value.ToString());
                    }
                    using (new GuiLayout(GuiLayout.Method.Horizontal))
                    {
                        GUILayout.Label("Data:");
                        GUILayout.Label(API.GetSectionDataForMod<SectionDataWithInfoNoTemplate>(section).value.ToString());
                    }
                }
            }
        }

        internal override void Start()
        {
            DragEnabled = true;
            WindowRect.Set((Screen.width - WindowWidth) / 4, (Screen.height - WindowHeight) / 2, WindowWidth, WindowHeight);
            WindowCaption = nameof(ShipSections);
            if (ApplicationLauncher.Instance != null && ApplicationLauncher.Ready)
                OnAppLauncherReady();
            else
                GameEvents.onGUIApplicationLauncherReady.Add(OnAppLauncherReady);
        }

        private ApplicationLauncherButton button;
        private Vector2 scrollPos;

        private void OnAppLauncherReady()
        {
            if (button != null)
            {
                ApplicationLauncher.Instance.RemoveModApplication(button);
                button = null;
            }
            button = ApplicationLauncher.Instance.AddModApplication(
                () => Visible = true,
                () => Visible = false,
                null,
                null,
                null,
                null,
                ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH,
                GameDatabase.Instance.GetTexture(AppLauncherIconLocation, false));
        }

        internal override void OnDestroy()
        {
            ApplicationLauncher.Instance.RemoveModApplication(button);
        }
    }
}
