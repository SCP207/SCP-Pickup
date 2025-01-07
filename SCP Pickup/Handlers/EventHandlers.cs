using Exiled.Events.EventArgs.Player;
using Exiled.API.Features;
using UnityEngine;
using MEC;
using System.Collections.Generic;
using Exiled.API.Enums;
using PlayerRoles;
using Exiled.API.Features.Pickups;
using System.Net.Http.Headers;
using System.Linq;
using CustomPlayerEffects;
using Exiled.API.Features.Items;

namespace Zombie_Pickup.Handlers
{
    public static class EventHandlers
    {
        private static Dictionary<Player, CoroutineHandle> currentCoroutines { get; } = new();

        public static void Register()
        {
            Exiled.Events.Handlers.Player.ChangingRole += OnRoleChange;
            Exiled.Events.Handlers.Player.TogglingNoClip += OnNoClipActivate;
        }

        public static void Unregister()
        {
            Exiled.Events.Handlers.Player.ChangingRole -= OnRoleChange;
            Exiled.Events.Handlers.Player.TogglingNoClip -= OnNoClipActivate;
        }

        public static void OnRoleChange(ChangingRoleEventArgs ev)
        {
            if (ev.Player.IsScp && Plugin.Singleton.Config.scpRoles.Contains(ev.NewRole))
            {
                ev.Player.ShowHint(Plugin.Singleton.Config.spawnMessage);
            }
            else
            {
                if (currentCoroutines.TryGetValue(ev.Player, out var currentCoroutine))
                {
                    Timing.KillCoroutines(currentCoroutine);
                    currentCoroutines.Remove(ev.Player);
                }
            }
        }

        public static void OnNoClipActivate(TogglingNoClipEventArgs ev)
        {
            if (!ev.Player.IsScp && !Plugin.Singleton.Config.scpRoles.Contains(ev.Player.Role)) return;
            if (!ev.Player.HasItem(ItemType.SCP1344))
            {
                if (Physics.Raycast(ev.Player.CameraTransform.position, ev.Player.CameraTransform.forward, out var hit,
                        5f, 1 << 9))
                {
                    var gameObject = hit.collider.transform.root.gameObject;

                    if (gameObject == null) return;
                    var pickup = Pickup.Get(gameObject);

                    if (pickup == null || !Plugin.Singleton.Config.items.Contains(pickup.Type)) return;
                    if (currentCoroutines.TryGetValue(ev.Player, out var currentCoroutine))
                    {
                        Timing.KillCoroutines(currentCoroutine);
                        currentCoroutines.Remove(ev.Player);
                        ev.Player.DisableEffect(EffectType.Ensnared);
                    }

                    currentCoroutines.Add(ev.Player, Timing.RunCoroutine(PickupItem(pickup, ev.Player)));
                }
                else
                {
                    if (currentCoroutines.TryGetValue(ev.Player, out var currentCoroutine))
                    {
                        Timing.KillCoroutines(currentCoroutine);
                        currentCoroutines.Remove(ev.Player);
                        ev.Player.DisableEffect(EffectType.Ensnared);
                        ev.Player.ShowHint(Plugin.Singleton.Config.disableMessage);
                    }
                }
            }
            else
            {
                ev.Player.ShowHint(Plugin.Singleton.Config.scp1344Message);
            }
        }

        private static IEnumerator<float> PickupItem(Pickup pickup, Player player)
        {
            player.DropItems();

            var pickupTime = pickup.PickupTimeForPlayer(player) * Plugin.Singleton.Config.pickupMultiplier;
            player.EnableEffect(EffectType.Ensnared, pickupTime);
            player.ShowHint(string.Format(Plugin.Singleton.Config.startMessage, pickup.Type), pickupTime);
            yield return Timing.WaitForSeconds(pickupTime);

            currentCoroutines.Remove(player);

            var item = player.AddItem(pickup, InventorySystem.Items.ItemAddReason.PickedUp);
            player.CurrentItem = item;
            pickup.Destroy();
        }
    }
}