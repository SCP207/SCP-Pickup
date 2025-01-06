using Exiled.API.Features.Pickups;
using Exiled.API.Interfaces;
using System.ComponentModel;

namespace Zombie_Pickup {
    public class Config : IConfig {
        [Description("Is this plugin enabled?")]
        public bool IsEnabled { get; set; } = true;

        [Description("Is debug mode enabled?")]
        public bool Debug { get; set; } = false;

        [Description("The message to show for when a player spawns as a zombie")]
        public string spawnMessage { get; set; } = "Press <color=#50C878>Left Alt</color> to pick up items";
        [Description("The message for starting to pick up an item. Use \"{0}\" for the item name")]
        public string startMessage { get; set; } = "Picking up <color=#00B7EB>{0}</color>";
        [Description("The message for disabling the picking up of an item")]
        public string disableMessage { get; set; } = "<color=#C50000>You have stopped picking up an item</color>";
        [Description("Message for it the zombie is holding SCP-1344")]
        public string scp1344Message { get; set; } = "<color=#C50000>You can't pick up items with SCP-1344</color>";

        [Description("The item ids of the items zombies can pick up")]
        public int[] items { get; set; } = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59 };

        [Description("A multiplier for how long it takes to pick up an item")]
        public float pickupMultiplier { get; set; } = 1.5f;
    }
}
