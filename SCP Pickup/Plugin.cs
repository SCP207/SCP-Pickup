using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using SCP_Pickup.Handlers;

namespace SCP_Pickup {
    public class Plugin : Plugin<Config> {
        public override string Author { get; } = "SCP-207";
        public override string Name { get; } = "SCP Pickup";
        public override string Prefix { get; } = "SP";
        public override PluginPriority Priority { get; } = PluginPriority.Default;
        public override Version RequiredExiledVersion { get; } = new(9, 3, 0);
        public override Version Version { get; } = new(2, 1, 1);

        public static Plugin Singleton { get; private set; }

        public override void OnEnabled() {
            Singleton = this;
            RegisterCommandsAndEvents();

            base.OnEnabled();
        }

        public override void OnDisabled() {
            Singleton = null;
            UnregisterCommandsAndEvents();

            base.OnDisabled();
        }

        private void RegisterCommandsAndEvents() => EventHandlers.Register();

        private void UnregisterCommandsAndEvents() => EventHandlers.Unregister();
    }
}