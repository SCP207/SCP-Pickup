using Exiled.API.Features.Pickups;
using Exiled.API.Interfaces;
using System.ComponentModel;
using System.Linq;

namespace Zombie_Pickup
{
    public class Config : IConfig
    {
        [Description("Is this plugin enabled?")]
        public bool IsEnabled { get; set; } = true;

        [Description("Is debug mode enabled?")]
        public bool Debug { get; set; } = false;

        [Description("The message to show for when a player role changes to be SCP-049-2")]
        public string spawnMessage { get; set; } = "Press <color=#50C878>Left Alt</color> to pick up items";

        [Description("The message for starting to pick up an item. Use \"{0}\" for the item name")]
        public string startMessage { get; set; } = "Picking up <color=#00B7EB>{0}</color>";

        [Description("The message for disabling the picking up of an item")]
        public string disableMessage { get; set; } = "<color=#C50000>You have stopped picking up an item</color>";

        [Description("Message for it the zombie is holding SCP-1344")]
        public string scp1344Message { get; set; } = "<color=#C50000>You can't pick up items with SCP-1344</color>";

        public int[] itemIds => items.Select(e => (int)e).ToArray();

        [Description("The item types of the items zombies can pick up (Enum)")]
        public ItemType[] items { get; set; }

        [Description("A multiplier for how long it takes to pick up an item")]
        public float pickupMultiplier { get; set; } = 1.5f;
    }
}