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

namespace Zombie_Pickup.Handlers {
    public class EventHandlers {
        private Plugin plugin = new();

        private Dictionary<Player, CoroutineHandle> currentCoroutines { get; } = new();

        public EventHandlers(Plugin main) {
            plugin = main;
        }

        public void OnRoleChange(ChangingRoleEventArgs ev) {
            if (ev.NewRole == RoleTypeId.Scp0492) {
                ev.Player.ShowHint(plugin.Config.spawnMessage);
            } else {
                if (currentCoroutines.TryGetValue(ev.Player, out var currentCoroutine)) {
                    Timing.KillCoroutines(currentCoroutine);
                    currentCoroutines.Remove(ev.Player);
                }
            }
        }

        public void OnNoClipActivate(TogglingNoClipEventArgs ev) {
            if (ev.Player.Role == RoleTypeId.Scp0492) {
                if (!ev.Player.HasItem(ItemType.SCP1344)) {
                    if (Physics.Raycast(ev.Player.CameraTransform.position, ev.Player.CameraTransform.forward, out RaycastHit hit, 5f, 1 << 9)) {
                        GameObject gameObject = hit.collider.transform.root.gameObject;

                        if (gameObject != null) {
                            Pickup pickup = Pickup.Get(gameObject);

                            if (pickup != null && plugin.Config.items.Contains((int)pickup.Type)) {
                                if (currentCoroutines.TryGetValue(ev.Player, out var currentCoroutine)) {
                                    Timing.KillCoroutines(currentCoroutine);
                                    currentCoroutines.Remove(ev.Player);
                                    ev.Player.DisableEffect(EffectType.Ensnared);
                                }

                                currentCoroutines.Add(ev.Player, Timing.RunCoroutine(PickupItem(pickup, ev.Player)));
                            }
                        }
                    } else {
                        if (currentCoroutines.TryGetValue(ev.Player, out var currentCoroutine)) {
                            Timing.KillCoroutines(currentCoroutine);
                            currentCoroutines.Remove(ev.Player);
                            ev.Player.DisableEffect(EffectType.Ensnared);
                            ev.Player.ShowHint(plugin.Config.disableMessage);
                        }
                    }
                } else {
                    ev.Player.ShowHint(plugin.Config.scp1344Message);
                }
            }
        }

        private IEnumerator<float> PickupItem(Pickup pickup, Player player) {
            player.DropItems();

            float pickupTime = pickup.PickupTimeForPlayer(player) * plugin.Config.pickupMultiplier;
            player.EnableEffect(EffectType.Ensnared, pickupTime);
            player.ShowHint(string.Format(plugin.Config.startMessage, pickup.Type), pickupTime);
            yield return Timing.WaitForSeconds(pickupTime);

            currentCoroutines.Remove(player);

            var item = player.AddItem(pickup, InventorySystem.Items.ItemAddReason.PickedUp);
            player.CurrentItem = item;
            pickup.Destroy();
        }
    }
}
