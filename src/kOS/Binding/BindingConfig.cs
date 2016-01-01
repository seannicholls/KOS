using kOS.Safe.Binding;
using kOS.Safe.Utilities;
using kOS.Suffixed;
using kOS.AddOns;

namespace kOS.Binding
{
    [Binding("ksp")]
    public class BindingConfig : Binding
    {
        public override void AddTo(SharedObjects shared)
        {
            shared.BindingMgr.AddGetter("CONFIG", () => SafeHouse.Config);

            AddonManager addonMgr = AddonManager.Instance;
            addonMgr.initialize (shared);
        }
    }
}
