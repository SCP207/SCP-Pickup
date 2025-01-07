using Exiled.API.Features.Pickups;
using Exiled.API.Interfaces;
using PlayerRoles;
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

        [Description("The message to show for when a player role changes to be an SCP")]
        public string spawnMessage { get; set; } = "Press <color=#50C878>Left Alt</color> to pick up items";

        [Description("The message for starting to pick up an item. Use \"{0}\" for the item name")]
        public string startMessage { get; set; } = "Picking up <color=#00B7EB>{0}</color>";

        [Description("The message for disabling the picking up of an item")]
        public string disableMessage { get; set; } = "<color=#C50000>You have stopped picking up an item</color>";

        [Description("Message given when an SCP is holding SCP-1344")]
        public string scp1344Message { get; set; } = "<color=#C50000>You can't pick up items with SCP-1344</color>";

        [Description("A multiplier for how long it takes to pick up an item")]
        public float pickupMultiplier { get; set; } = 1.5f;

        [Description("The SCPs that can pick up items")]
        public RoleTypeId[] scpRoles { get; set; } = {
            RoleTypeId.Scp0492
        };

        [Description("The item types of the items the SCPs can pick up")]
        public ItemType[] items { get; set; } = {
            ItemType.KeycardJanitor,
            ItemType.KeycardScientist,
            ItemType.KeycardResearchCoordinator,
            ItemType.KeycardZoneManager,
            ItemType.KeycardGuard,
            ItemType.KeycardMTFPrivate,
            ItemType.KeycardContainmentEngineer,
            ItemType.KeycardMTFOperative,
            ItemType.KeycardMTFCaptain,
            ItemType.KeycardFacilityManager,
            ItemType.KeycardChaosInsurgency,
            ItemType.KeycardO5,
            ItemType.Radio,
            ItemType.GunCOM15,
            ItemType.Medkit,
            ItemType.Flashlight,
            ItemType.MicroHID,
            ItemType.SCP500,
            ItemType.SCP207,
            ItemType.Ammo12gauge,
            ItemType.GunE11SR,
            ItemType.GunCrossvec,
            ItemType.Ammo556x45,
            ItemType.GunFSP9,
            ItemType.GunLogicer,
            ItemType.GrenadeHE,
            ItemType.GrenadeFlash,
            ItemType.Ammo44cal,
            ItemType.Ammo762x39,
            ItemType.Ammo9x19,
            ItemType.GunCOM18,
            ItemType.SCP018,
            ItemType.SCP268,
            ItemType.Adrenaline,
            ItemType.Painkillers,
            ItemType.Coin,
            ItemType.ArmorLight,
            ItemType.ArmorCombat,
            ItemType.ArmorHeavy,
            ItemType.GunRevolver,
            ItemType.GunAK,
            ItemType.GunShotgun,
            ItemType.SCP330,
            ItemType.SCP2176,
            ItemType.SCP244a,
            ItemType.SCP244b,
            ItemType.SCP1853,
            ItemType.ParticleDisruptor,
            ItemType.GunCom45,
            ItemType.SCP1576,
            ItemType.Jailbird,
            ItemType.AntiSCP207,
            ItemType.GunFRMG0,
            ItemType.GunA7,
            ItemType.Lantern,
            ItemType.SCP1344,
            ItemType.Snowball,
            ItemType.Coal,
            ItemType.SpecialCoal,
            ItemType.SCP1507Tape
        };
    }
}