using System;

using UnityEngine;

using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;


namespace kOS.Suffixed
{
    public class AddonList : Structure
    {

        public AddonList()
        {
            InitializeSuffixes();
        }

        private void InitializeSuffixes()
        {
            /*
            AddSuffix("KAC", new Suffix<Addon>(() => new AddOns.KerbalAlarmClock.Addon(shared)));
            AddSuffix("RT", new Suffix<Addon>(() => new AddOns.RemoteTech.Addon(shared)));
            AddSuffix("AGX", new Suffix<Addon>(() => new AddOns.ActionGroupsExtended.Addon(shared)));
            AddSuffix("IR", new Suffix<Addon>(() => new AddOns.InfernalRobotics.Addon(shared)));
            */

            /*
            AddAddon ("KAC", typeof(AddOns.KerbalAlarmClock.Addon));
            AddAddon ("RT", typeof(AddOns.RemoteTech.Addon));
            AddAddon ("AGX", typeof(AddOns.ActionGroupsExtended.Addon));
            AddAddon ("IR", typeof(AddOns.InfernalRobotics.Addon));
            */
        }

        /*
         * Expose a method to allow 3rd parties to add Addons 
         */
        public void AddAddon(string[] addonName, ISuffix suffixToAdd) {
            AddSuffix (addonName, suffixToAdd);
        }

        public override string ToString()
        {
            return string.Format("{0} AddonList", base.ToString());
        }
    }
}