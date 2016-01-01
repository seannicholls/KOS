using System;

using UnityEngine;

using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;


namespace kOS.Suffixed
{
    public class AddonList : Structure
    {

        public AddonList() {}

        /*
         * Expose a method to allow 3rd parties to install Addons 
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