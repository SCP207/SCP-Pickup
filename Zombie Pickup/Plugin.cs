using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerHandlers = Exiled.Events.Handlers.Player;

namespace Zombie_Pickup {
    public class Plugin : Plugin<Config> {
        public override string Author { get; } = "SCP-207";
        public override string Name { get; } = "Zombie Pickup";
        public override string Prefix { get; } = "ZP";
        public override PluginPriority Priority { get; } = PluginPriority.Default;
        public override Version RequiredExiledVersion { get; } = new(9, 2, 2);
        public override Version Version { get; } = new(1, 0, 0);

        private Handlers.EventHandlers events;

        public override void OnEnabled() {
            RegisterCommands();

            base.OnEnabled();
        }

        public override void OnDisabled() {
            UnregisterCommands();

            base.OnDisabled();
        }

        private void RegisterCommands() {
            events = new(this);

            PlayerHandlers.ChangingRole += events.OnRoleChange;
            PlayerHandlers.TogglingNoClip += events.OnNoClipActivate;
        }

        private void UnregisterCommands() {
            PlayerHandlers.ChangingRole -= events.OnRoleChange;
            PlayerHandlers.TogglingNoClip -= events.OnNoClipActivate;

            events = null;
        }
    }
}
