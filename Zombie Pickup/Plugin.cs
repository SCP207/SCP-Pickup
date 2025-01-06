using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using Zombie_Pickup.Handlers;

namespace Zombie_Pickup
{
    public class Plugin : Plugin<Config>
    {
        public override string Author { get; } = "SCP-207";
        public override string Name { get; } = "Zombie Pickup";
        public override string Prefix { get; } = "ZP";
        public override PluginPriority Priority { get; } = PluginPriority.Default;
        public override Version RequiredExiledVersion { get; } = new(9, 2, 2);
        public override Version Version { get; } = new(1, 0, 0);

        public static Plugin Singleton { get; private set; }

        public override void OnEnabled()
        {
            Singleton = this;
            RegisterCommandsAndEvents();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            UnregisterCommandsAndEvents();

            base.OnDisabled();
        }

        private void RegisterCommandsAndEvents()
        {
            EventHandlers.Register();
        }

        private void UnregisterCommandsAndEvents()
        {
            EventHandlers.Unregister();
        }
    }
}