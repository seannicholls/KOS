using System;
using System.Collections.Generic;

using kOS;
using kOS.Safe.Binding;
using kOS.Safe.Utilities;
using kOS.Suffixed;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;

using UnityEngine;


namespace kOS.AddOns
{
    public sealed class AddonManager
    {
        
        private static readonly AddonManager _instance = new AddonManager();
        private AddonList _addonList = null;
        private SharedObjects _shared;
        private List<String> _installedAddons = new List<String>();
        private readonly List<String> _protectedAddons= new List<String>() {
            "LIST",
            "KAC",
            "RT",
            "AGX",
            "IR",
        };
            
        private AddonManager(){}

        public static AddonManager Instance
        {
            get 
            {
                return _instance; 
            }
        }

        public bool isReady() {
            
            return (_addonList != null);

        }

        public void initialize(SharedObjects shared) {

            _shared = shared;
            _addonList = new AddonList();

            // SUFFIXES
            _addonList.AddSuffix("LIST", new Suffix<ListValue>(GetAddonList, "List installed addons"));

            // ADDONS
            _shared.BindingMgr.AddGetter("ADDONS", () => _addonList);

                addAddon ("KAC", typeof(AddOns.KerbalAlarmClock.Addon), true);
                addAddon ("RT", typeof(AddOns.RemoteTech.Addon), true);
                addAddon ("AGX", typeof(AddOns.ActionGroupsExtended.Addon), true);
                addAddon ("IR", typeof(AddOns.InfernalRobotics.Addon), true);

        }

        private ListValue GetAddonList()
        {
            ListValue list = new ListValue();

            foreach(String elm in _installedAddons) {
                list.Add (elm);
            }

            return list;
        }

        public static Boolean install(String addonName, Type objType) {

            return AddonManager.Instance.addAddon (addonName, objType, false);

        }

        public Boolean addAddon(String addonName, Type objType, Boolean forceful) {

            if(!forceful && _protectedAddons.Contains(addonName)) {
                    
                UnityEngine.Debug.Log ("kOS: Addon '" + addonName + "' is reserved, refusing install.");
                return false;

            } else if (!forceful && _installedAddons.Contains (addonName)) {
            
                UnityEngine.Debug.Log ("kOS: Addon '" + addonName + "' is already installed");
                return false;
            
            } else {

                try {

                    // intantiate the object (addon)
                    Addon theAddon = (Addon)Activator.CreateInstance (objType, _shared);

                    // add it to ADDONS
                    _addonList.AddAddon (new[]{addonName}, new Suffix<Addon> (() => theAddon));

                } catch(Exception e) {

                    UnityEngine.Debug.Log ("kOS: Addon '" + addonName + "' could not be installed");
                    UnityEngine.Debug.Log (e.Message);

                    return false;
                }


                // Success
                UnityEngine.Debug.Log ("kOS: Addon '" + addonName + "' was installed");
                _installedAddons.Add (addonName);
                return true;

            }

        }

    }

}

