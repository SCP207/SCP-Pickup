using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Player;
using MEC;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using PlayerHandlers = Exiled.Events.Handlers.Player;

namespace SCP_Pickup.Handlers {
    public static class EventHandlers {
        private static Dictionary<Player, (CoroutineHandle, Pickup)> currentCoroutines { get; } = new();

        public static void Register() {
            PlayerHandlers.ChangingRole += OnRoleChange;
            PlayerHandlers.InteractingDoor += OnDoorInteract;
            PlayerHandlers.TogglingNoClip += OnNoClipActivate;
        }

        public static void Unregister() {
            PlayerHandlers.ChangingRole -= OnRoleChange;
            PlayerHandlers.InteractingDoor -= OnDoorInteract;
            PlayerHandlers.TogglingNoClip -= OnNoClipActivate;
        }

        #region Player Events
        private static void OnRoleChange(ChangingRoleEventArgs ev) {
            // Checks if the player spawns in as an SCP that can pick up items and shows them the hint //
            if (ev.NewRole.IsScp() && Plugin.Singleton.Config.scpRoles.Contains(ev.NewRole)) {
                ev.Player.ShowHint(Plugin.Singleton.Config.spawnMessage);
            } else {
                // Removes them from the dictionary if tey were previously picking up an item //
                if (currentCoroutines.TryGetValue(ev.Player, out var currentCoroutine)) {
                    Timing.KillCoroutines(currentCoroutine.Item1);
                    currentCoroutines.Remove(ev.Player);

                    currentCoroutine.Item2.IsLocked = false;
                }
            }
        }

        private static void OnDoorInteract(InteractingDoorEventArgs ev) {
            if (ev.Player.IsScp && ev.Door.IsCheckpoint)
                ev.Door.IsOpen = (ev.Player.CurrentItem != null) ? true : false;
        }

        private static void OnNoClipActivate(TogglingNoClipEventArgs ev) {
            // Checks if the player is SCP or if thier role is in the list //
            if (!ev.Player.IsScp || !Plugin.Singleton.Config.scpRoles.Contains(ev.Player.Role)) return;

            // Checks if the player has SCP-1344 //
            if (ev.Player.HasItem(ItemType.SCP1344)) {
                // Shows the hint given when they attempt to pick up an item with SCP-1344 //
                ev.Player.ShowHint(Plugin.Singleton.Config.scp1344Message);
                return;
            }

            // Casts a Raycast to only check for Pickups //
            if (Physics.Raycast(ev.Player.CameraTransform.position, ev.Player.CameraTransform.forward, out var hit,
                    5f,  1 << 9)) {

                // Picks up the item if it isn't null and is in the item list //
                var gameObject = hit.collider.gameObject;

                while (gameObject.transform.parent is not null
                    && !gameObject.name.Contains("Pickup")) {

                    gameObject = gameObject.transform.parent.gameObject;
                }

                if (gameObject == null) return;
                var pickup = Pickup.Get(gameObject);

                if (pickup is null || pickup.IsLocked || !Plugin.Singleton.Config.items.Contains(pickup.Type)) return;
                if (currentCoroutines.TryGetValue(ev.Player, out var currentCoroutine)) {
                    Timing.KillCoroutines(currentCoroutine.Item1);
                    currentCoroutines.Remove(ev.Player);
                    ev.Player.DisableEffect(EffectType.Ensnared);

                    currentCoroutine.Item2.IsLocked = false;
                }

                currentCoroutines.Add(ev.Player, (Timing.RunCoroutine(PickupItem(pickup, ev.Player)), pickup));
            } else {
                // Cancels the pickup if they are picking up an item //
                if (currentCoroutines.TryGetValue(ev.Player, out var currentCoroutine)) {
                    Timing.KillCoroutines(currentCoroutine.Item1);
                    currentCoroutines.Remove(ev.Player);
                    ev.Player.DisableEffect(EffectType.Ensnared);
                    ev.Player.ShowHint(string.Format(Plugin.Singleton.Config.disableMessage, currentCoroutine.Item2.Type));

                    // Unlocked the item, allowing it to be picked up //
                    currentCoroutine.Item2.IsLocked = false;
                }
            }
        }
        #endregion

        #region Coroutines
        private static IEnumerator<float> PickupItem(Pickup pickup, Player player) {
            // Drops the players items //
            player.DropItems();

            // Prevents the item ftom being picked up by someone else //
            pickup.IsLocked = true;

            // Picks up the item with visual queues //
            var pickupTime = pickup.PickupTimeForPlayer(player) * Plugin.Singleton.Config.pickupMultiplier;
            player.EnableEffect(EffectType.Ensnared, pickupTime);
            player.ShowHint(string.Format(Plugin.Singleton.Config.startMessage, pickup.Type), pickupTime);
            yield return Timing.WaitForSeconds(pickupTime);

            // Adds the item to the inventory //
            var item = player.AddItem(pickup, InventorySystem.Items.ItemAddReason.PickedUp);
            player.CurrentItem = item;
            pickup.UnSpawn();

            // Removes them from the courotines so they can no longer cancel the pickup //
            currentCoroutines.Remove(player);
        }
        #endregion
    }
}