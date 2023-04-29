using System;
using System.Collections.Generic;

using UnityEngine;

namespace MelonRajce.Features.Combat
{
    internal class WeaponMods : Feature
    {
        private Dictionary<WeaponManager.Weapon, Dictionary<string, object>> OldValues = new Dictionary<WeaponManager.Weapon, Dictionary<string, object>>();

        public override string Name { get; protected set; } = "Weapon Mods";
        public override string Description { get; protected set; } = "Modifies your weapon";
        public override bool IsKeyBindable { get; protected set; } = false;
        public override KeyCode BindedKey { get; set; }

        public bool DisableRecoil = false;
        public bool DisableSpread = false;

        public bool FireRateEnabled = false;
        public float FireRate = -1;

        public bool AllAuto = false;
        public bool InstantReload = false;

        public void ModWeapons()
        {
            if (!m_bIsActive)
                return;

            if (!m_bIsConnected)
                return;

            WeaponManager wpnManager = PlayerManager.localPlayer.GetComponent<WeaponManager>();
            if (wpnManager == null)
            {
                RajceMain.logger.Msg("Failed to get weapon manager");
                return;
            }

            foreach (WeaponManager.Weapon wpn in wpnManager.weapons)
            {
                // Sets the old gun data so we can restore it after its disabled
                Dictionary<string, object> data;
                {
                    if (!OldValues.TryGetValue(wpn, out data))
                        data = new Dictionary<string, object>();

                    if (!data.ContainsKey("firerate"))
                        data["firerate"] = wpn.shotsPerSecond;
                    if (!data.ContainsKey("spread"))
                        data["spread"] = wpn.unfocusedSpread;
                    if (!data.ContainsKey("recoil"))
                        data["recoil"] = wpn.recoil;
                    if (!data.ContainsKey("reload"))
                        data["reload"] = wpn.reloadingTime;
                    if (!data.ContainsKey("fullauto"))
                        data["fullauto"] = wpn.allowFullauto;
                    OldValues[wpn] = data;
                }

                if (DisableRecoil)
                    wpn.recoil = new RecoilProperties(); // Zeros out the recoil
                else wpn.recoil = (RecoilProperties)data["recoil"]; // Reverts the changes back to normal

                if (DisableSpread)
                    wpn.unfocusedSpread = 0; // Sets the spread radius to 0
                else wpn.unfocusedSpread = (float)data["spread"]; // Set the spread back to normal

                if (FireRateEnabled && FireRate != -1)
                {
                    // Check if the selected firerate is 0 cause if would have shot we wouldn't be able to shoot ever again
                    if (FireRate == 0)
                        FireRate = 1;

                    wpn.shotsPerSecond = FireRate; 
                }
                else wpn.shotsPerSecond = (float)data["firerate"]; // Set the firerate back to its normal value

                if (AllAuto)
                    wpn.allowFullauto = true;
                else wpn.allowFullauto = (bool)data["fullauto"];

                if (InstantReload)
                    wpn.reloadingTime = 0;
                else wpn.reloadingTime = (float)data["reload"];
            }
        }


        public override void OnEnable() => ModWeapons();
        public override void OnDisable()
        {
            if (!m_bIsConnected)
                return;

            WeaponManager wpnManager = PlayerManager.localPlayer.GetComponent<WeaponManager>();

            // Sets back values of guns
            foreach (WeaponManager.Weapon wpn in wpnManager.weapons)
            {
                if (!OldValues.TryGetValue(wpn, out var data))
                    continue;

                wpn.shotsPerSecond = (float)data["firerate"];
                wpn.unfocusedSpread = (float)data["spread"];
                wpn.recoil = (RecoilProperties)data["recoil"];
                wpn.reloadingTime = (float)data["reload"];
                wpn.allowFullauto = (bool)data["fullauto"];
            }
        }

        public override void OnConnect()
        {
            
        }
    }
}
