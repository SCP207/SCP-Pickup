﻿using Exiled.Events.EventArgs.Player;
using Exiled.API.Features;
using UnityEngine;
using MEC;
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Pickups;
using Exiled.API.Extensions;

namespace SCP_Pickup.Handlers {
    public static class EventHandlers {
        private static Dictionary<Player, CoroutineHandle> currentCoroutines { get; } = new();

        public static void Register() {
            Exiled.Events.Handlers.Player.ChangingRole += OnRoleChange;
            Exiled.Events.Handlers.Player.TogglingNoClip += OnNoClipActivate;
        }

        public static void Unregister() {
            Exiled.Events.Handlers.Player.ChangingRole -= OnRoleChange;
            Exiled.Events.Handlers.Player.TogglingNoClip -= OnNoClipActivate;
        }

        private static void OnRoleChange(ChangingRoleEventArgs ev) {
            // Checks if the player spawns in as an SCP that can pick up items and shows them the hint //
            if (ev.NewRole.IsScp() && Plugin.Singleton.Config.scpRoles.Contains(ev.NewRole)) {
                ev.Player.ShowHint(Plugin.Singleton.Config.spawnMessage);
            } else {
                // Removes them from the dictionary if tey were previously picking up an item //
                if (currentCoroutines.TryGetValue(ev.Player, out var currentCoroutine)) {
                    Timing.KillCoroutines(currentCoroutine);
                    currentCoroutines.Remove(ev.Player);
                }
            }
        }

        private static void OnNoClipActivate(TogglingNoClipEventArgs ev) {
            // Checks if the player is SCP or if thier role is in the list //
            if (!ev.Player.IsScp || !Plugin.Singleton.Config.scpRoles.Contains(ev.Player.Role)) return;

            // Checks if the player has SCP-1344 //
            if (!ev.Player.HasItem(ItemType.SCP1344)) {
                // Casts a Raycast to only check for Pickups //
                if (Physics.Raycast(ev.Player.CameraTransform.position, ev.Player.CameraTransform.forward, out var hit,
                        5f, 1 << 9)) {
                    // Picks up the item if it isn't null and is in the item list //
                    var gameObject = hit.collider.transform.root.gameObject;

                    if (gameObject == null) return;
                    var pickup = Pickup.Get(gameObject);

                    if (pickup == null || pickup.IsLocked || !Plugin.Singleton.Config.items.Contains(pickup.Type)) return;
                    if (currentCoroutines.TryGetValue(ev.Player, out var currentCoroutine)) {
                        Timing.KillCoroutines(currentCoroutine);
                        currentCoroutines.Remove(ev.Player);
                        ev.Player.DisableEffect(EffectType.Ensnared);
                    }

                    currentCoroutines.Add(ev.Player, Timing.RunCoroutine(PickupItem(pickup, ev.Player)));
                } else {
                    // Cancels the pickup if they are picking up an item //
                    if (currentCoroutines.TryGetValue(ev.Player, out var currentCoroutine)) {
                        Timing.KillCoroutines(currentCoroutine);
                        currentCoroutines.Remove(ev.Player);
                        ev.Player.DisableEffect(EffectType.Ensnared);
                        ev.Player.ShowHint(Plugin.Singleton.Config.disableMessage);
                    }
                }
            } else {
                // Shows the hint given when they attempt to pick up an item with SCP-1344 //
                ev.Player.ShowHint(Plugin.Singleton.Config.scp1344Message);
            }
        }

        private static IEnumerator<float> PickupItem(Pickup pickup, Player player) {
            // Drops the players items //
            player.DropItems();

            // Picks up the item with visual queues //
            var pickupTime = pickup.PickupTimeForPlayer(player) * Plugin.Singleton.Config.pickupMultiplier;
            player.EnableEffect(EffectType.Ensnared, pickupTime);
            player.ShowHint(string.Format(Plugin.Singleton.Config.startMessage, pickup.Type), pickupTime);
            yield return Timing.WaitForSeconds(pickupTime);

            // Adds the item to the inventory //
            var item = player.AddItem(pickup, InventorySystem.Items.ItemAddReason.PickedUp);
            player.CurrentItem = item;
            pickup.Destroy();

            // Removes them from the courotines so they can no longer cancel the pickup //
            currentCoroutines.Remove(player);
        }
    }
}