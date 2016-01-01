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

        private class InstallableAddon {
            
            private Addon _addon;

            public Type objType;
            public String name;

            public Action<Boolean> callback;

            public Boolean isInstalled = false;

            public InstallableAddon(String kname, Type ktype) {
                
                this.name = kname;
                this.objType = ktype;
            }

            public Addon addon {

                get {
                    return _addon;
                }

            }

            public Addon CreateInstance(SharedObjects shared) {
                
                _addon = (Addon)Activator.CreateInstance (objType, shared);
                return _addon;
            }

        }

        private static readonly AddonManager _instance = new AddonManager();

        private AddonList _addonList = new AddonList ();
        private SharedObjects _shared = null;

        private List<InstallableAddon> _installedAddons = new List<InstallableAddon>();
        private List<InstallableAddon> _queuedAddons = new List<InstallableAddon>();

        private readonly List<String> _protectedAddonNames= new List<String>() {
            "LIST",
            "KAC",
            "RT",
            "AGX",
            "IR",
        };
     
            
        private AddonManager() {}

        public static AddonManager Instance
        {
            get 
            {
                return _instance; 
            }
        }

        public static bool isAvailable() {

            return AddonManager.Instance.isReady ();

        }

        public bool isReady() {
            
            return (_addonList != null) && (_shared != null);

        }

        public void initialize(SharedObjects shared) {

            _shared = shared;

            // SUFFIXES
            _addonList.AddSuffix("LIST", new Suffix<ListValue>(GetAddonList, "List installed addons"));

            // ADDONS
            _shared.BindingMgr.AddGetter("ADDONS", () => _addonList);

                _installAddon ("KAC", typeof(AddOns.KerbalAlarmClock.Addon), true);
                _installAddon ("RT", typeof(AddOns.RemoteTech.Addon), true);
                _installAddon ("AGX", typeof(AddOns.ActionGroupsExtended.Addon), true);
                _installAddon ("IR", typeof(AddOns.InfernalRobotics.Addon), true);

            // INSTALL QUEUED ADDONS
            _installQueuedAddons();

        }

        private void _installQueuedAddons() {

            if (_queuedAddons.Count > 0) {
                UnityEngine.Debug.Log ("kOS: Installing queued addons...");
            }

            foreach (InstallableAddon installable in _queuedAddons) {

                if (!installable.isInstalled) {
                    
                    try {
                        
                        installable.CreateInstance (_shared);
                        _addonList.AddAddon (new[]{ installable.name }, new Suffix<Addon> (() => installable.addon));
                        installable.isInstalled = true;

                        _installedAddons.Add(installable);
                        UnityEngine.Debug.Log ("kOS: Addon '" + installable.name + "' installed");

                        if(installable.callback != null) {
                            installable.callback(true);
                        }

                    } catch(Exception e) {

                        UnityEngine.Debug.Log ("kOS: Addon '" + installable.name + "' could not be installed, an error occurred:");
                        UnityEngine.Debug.Log (e.ToString ());

                        if(installable.callback != null) {
                            installable.callback(false);
                        }

                    }

                }

            }

        }

        private ListValue GetAddonList()
        {
            ListValue list = new ListValue();

            foreach(InstallableAddon addon in _installedAddons) {
                if (addon.isInstalled) {
                    list.Add (addon.name);
                }
            }

            return list;
        }

        public static Boolean install(String addonName, Type objType) {

            return Instance._installAddon (addonName, objType, false);

        }

        private Boolean _addonNameExists(String addonName) {

            // Installed addons
            foreach(InstallableAddon installableAddon in _installedAddons) {

                if(installableAddon.name.Equals(addonName)) {
                    return true;
                }
            }

            // Queued Addons
            foreach(InstallableAddon installableAddon in _queuedAddons) {

                if(installableAddon.name.Equals(addonName)) {
                    return true;
                }
            }

            return false;

        }

        /*
         * Queue an Addon to be Installed when kOS is ready
         */
        public static Boolean installWhenReady(String addonName, Type objType) {

            if (!Instance._addonNameExists (addonName)) {
                
                InstallableAddon installable = new InstallableAddon (addonName, objType);
                Instance._queuedAddons.Add (installable);

                return true;

            }

            return false;

        }

        /*
         * Queue an Addon to be Installed when kOS is ready
         * 
         * Include a callback to be executed when installed
         * 
         */
        public static Boolean installWhenReady(String addonName, Type objType, Action<Boolean> callback) {

            if (!Instance._addonNameExists (addonName)) {

                InstallableAddon installable = new InstallableAddon (addonName, objType);
                installable.callback = callback;
                Instance._queuedAddons.Add (installable);

                return true;

            }

            return false;

        }

        private Boolean _installAddon(String addonName, Type objType, Boolean forceful) {
            
            if ( !forceful && (_addonNameExists(addonName) || _protectedAddonNames.Contains(addonName)) ) {
            
                UnityEngine.Debug.Log ("kOS: Addon '" + addonName + "' is already installed");
                return false;
            
            } else {


                try {

                    // "Install"
                    InstallableAddon installable = new InstallableAddon(addonName, objType);

                    installable.CreateInstance (_shared);
                    _addonList.AddAddon (new[]{ installable.name }, new Suffix<Addon> (() => installable.addon));
                    installable.isInstalled = true;

                    _installedAddons.Add(installable);
                    UnityEngine.Debug.Log ("kOS: Addon '" + installable.name + "' installed");

                    return true;


                } catch(Exception e) {

                    UnityEngine.Debug.Log ("kOS: Addon '" + addonName + "' could not be installed, an error occurred:");
                    UnityEngine.Debug.Log (e.ToString ());

                    return false;

                }

            }

        }

    }

}

