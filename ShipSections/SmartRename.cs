using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JKorTech.ShipSections
{
    public class SmartRename : PartModule, IVesselAutoRename
    {
        [KSPField(guiActive = false, isPersistant = true)]
        private bool init;

        [KSPField(guiActive = false, isPersistant = true)]
        private string initialName;

        public override void OnAwake()
        {
            base.OnAwake();
        }

        void OnDestroy()
        {
            GameEvents.onVesselRename.Remove(OnRenameVessel);
        }

        private void OnRenameVessel(GameEvents.HostedFromToAction<Vessel, string> data)
        {
            UnityEngine.Debug.Log("SmartRename: " + part.name);
            if( vessel.id == data.host.id && data.from == initialName)
            {
                UnityEngine.Debug.Log($"[{nameof(ShipSections)}] [{nameof(SmartRename)}] Updating initial vessel name to {data.to}");
                initialName = data.to;
            }
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            if (HighLogic.LoadedSceneIsFlight)
            {
                GameEvents.onVesselRename.Add(OnRenameVessel);
                if (!init)
                {
                    UnityEngine.Debug.Log($"[{nameof(ShipSections)}] Initializing {nameof(SmartRename)}");
                    UnityEngine.Debug.Log($"[{nameof(ShipSections)}] [{nameof(SmartRename)}] Initial vessel name is {vessel.vesselName}");
                    initialName = vessel.vesselName;
                    init = true;
                }
            }
        }

        public string GetVesselName()
        {
            return $"{initialName} {part.FindModuleImplementing<SectionInfo>().section} " +
                (vessel.vesselType == VesselType.Debris ? nameof(VesselType.Debris) : string.Empty);
        }

        public VesselType GetVesselType()
        {
            return vessel.vesselType;
        }
    }
}
