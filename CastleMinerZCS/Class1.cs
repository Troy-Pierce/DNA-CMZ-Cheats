using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blueberry;
using Blueberry.Forms;
using DNA;
using DNA.CastleMinerZ;
using DNA.CastleMinerZ.AI;
using DNA.CastleMinerZ.Inventory;
using DNA.CastleMinerZ.Net;
using DNA.CastleMinerZ.Terrain;
using DNA.CastleMinerZ.UI;
using DNA.Drawing.UI;
using DNA.Net.GamerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CastleMinerZCS
{
    public class GameMenu
    {
        Dictionary<NetworkGamer, MenuButton> Gamers = new Dictionary<NetworkGamer, MenuButton>();
        String[] songNames = { "Click", "Error", "Award", "Popup", "Teleport", "Reload", "BulletHitHuman", "thunderBig", "craft", "dropitem", "pickupitem", "punch", "punchMiss", "arrow", "AssaultReload", "Shotgun", "ShotGunReload", "Song1", "Song2", "lostSouls", "CreatureUnearth", "HorrorStinger", "Fireball", "Iceball", "DoorClose", "DoorOpen", "Song5", "Song3", "Song4", "Song6", "locator", "Fuse", "LaserGun1", "LaserGun2", "LaserGun3", "LaserGun4", "LaserGun5", "Beep", "SolidTone", "RPGLaunch", "Alien", "SpaceTheme", "GrenadeArm", "RocketWhoosh", "LightSaber", "LightSaberSwing", "GroundCrash", "ZombieDig", "ChainSawIdle", "ChainSawSpinning", "ChainSawCutting", "Birds", "FootStep", "Theme", "Pick", "Place", "Crickets", "Drips", "BulletHitDirt", "GunShot1", "GunShot2", "GunShot3", "GunShot4", "BulletHitSpray", "thunderLow", "Sand", "leaves", "dirt", "Skeleton", "ZombieCry", "ZombieGrowl", "Hit", "Fall", "Douse", "DragonScream", "Explosion", "WingFlap", "DragonFall", "Freeze", "Felguard" };

        public void createPlayerPage(NetworkGamer gamer, Page page)
        {
            Page gamerPage = page.getMenu().CreatePage(gamer.Gamertag, page, false);
            MenuButton gamerButton = page.CreateButton(gamer.Gamertag, TYPE.PAGE);
            gamerButton.page = gamerPage;
            Gamers.Add(gamer, gamerButton);
            Player plr = (Player)gamer.Tag;
            Page inventoryPage = page.getMenu().CreatePage("Inventory", gamerPage, true);
            Page inventoryGivePage = page.getMenu().CreatePage("Give Item", inventoryPage, true);
            foreach (InventoryItemIDs id in Enum.GetValues(typeof(InventoryItemIDs)))
            {
                inventoryGivePage.CreateButton(id.ToString(), TYPE.CODE).activationCode = () => { if (plr.ValidGamer) { InventoryItem item = DNA.CastleMinerZ.Inventory.InventoryItem.CreateItem(id, 1); item.StackCount = item.MaxStackCount; DNA.CastleMinerZ.Net.CreatePickupMessage.Send((LocalNetworkGamer)CastleMinerZGame.Instance.LocalPlayer.Gamer, plr.LocalPosition, Vector3.Zero, DNA.CastleMinerZ.PickupManager.Instance._nextPickupID++, item, true, false); } return true; };
            }
            gamerPage.CreateButton("Teleport", TYPE.CODE).activationCode = () =>
            {
                if (plr.ValidGamer) {
                    CastleMinerZGame.Instance.LocalPlayer.LocalPosition = plr.LocalPosition;
                }
                return true;
            };
            Page localSpawnPage = gamerPage.getMenu().CreatePage("Spawn Enemies", gamerPage, true);
            localSpawnPage.CreateButton("Spawn Horde - 10", TYPE.CODE).activationCode = () =>
            {
                if (plr.ValidGamer)
                {
                    WorldUtils.Mobs.spawnHorde(plr, 10);
                }
                return true;
            };
            localSpawnPage.CreateButton("Spawn Horde - 50", TYPE.CODE).activationCode = () =>
            {
                if (plr.ValidGamer)
                {
                    WorldUtils.Mobs.spawnHorde(plr, 50);
                }
                return true;
            };
            localSpawnPage.CreateButton("Spawn Horde - 100", TYPE.CODE).activationCode = () =>
            {
                if (plr.ValidGamer) 
                {
                    WorldUtils.Mobs.spawnHorde(plr, 100);
                }
                return true;
            };
            localSpawnPage.CreateButton("Spawn Horde - 200", TYPE.CODE).activationCode = () =>
            {
                if (plr.ValidGamer)
                {
                    WorldUtils.Mobs.spawnHorde(plr, 200);
                }
                return true;
            };
            foreach (EnemyTypeEnum enemyType in Enum.GetValues(typeof(EnemyTypeEnum)))
            {
                localSpawnPage.CreateButton(enemyType.ToString(), TYPE.CODE).activationCode = () => { if (plr.ValidGamer) { DNA.CastleMinerZ.AI.EnemyManager.Instance.SpawnEnemy(plr.LocalPosition, enemyType, plr.LocalPosition, 1); }; return true; };
            }
            gamerPage.CreateButton("Explode", TYPE.CODE).activationCode = () =>
            {
                if (plr.ValidGamer)
                {
                    DNA.CastleMinerZ.Net.DetonateExplosiveMessage.Send((LocalNetworkGamer)gamer, (IntVector3)plr.LocalPosition, true, ExplosiveTypes.C4);
                }
                return true;
            };
            gamerPage.CreateButton("Kick", TYPE.CODE).activationCode = () =>
            {
                if (plr.ValidGamer)
                {
                    DNA.CastleMinerZ.Net.KickMessage.Send((LocalNetworkGamer)CastleMinerZGame.Instance.CurrentNetworkSession.Host, gamer, false);
                }
                return true;
            };
            gamerPage.CreateButton("Ban", TYPE.CODE).activationCode = () =>
            {
                if (plr.ValidGamer)
                {
                    DNA.CastleMinerZ.Net.KickMessage.Send((LocalNetworkGamer)CastleMinerZGame.Instance.CurrentNetworkSession.Host, gamer, true);
                }
                return true;
            };

        }
        public GameMenu()
        {
            Menu menu = Blueberry.Main.CreateMenu("CastleMinerZ", null);
            // HOME PAGE
            Page mainPage = menu.CreatePage("Home", null, false);
            Page localPage = menu.CreatePage("LocalPlayer", mainPage, true);
            Page playersPage = menu.CreatePage("Players", mainPage, true);
            Page weaponPage = menu.CreatePage("Weapon", mainPage, true);
            //Page playerPage = menu.CreatePage("Players", mainPage);
            MenuButton _teleport = mainPage.CreateButton("Teleport", TYPE.CODE);
            //Screen _teleportScreen = (Screen) typeof(GameScreen).GetField("_teleportMenu", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(CastleMinerZGame.Instance.GameScreen);
            _teleport.activationCode = () => { CastleMinerZGame.Instance.GameScreen._uiGroup.PushScreen(CastleMinerZGame.Instance.GameScreen._teleportMenu); return true; };
            Page worldPage = menu.CreatePage("World", mainPage, true);
            Page soundPage = menu.CreatePage("Sounds", mainPage, true);
            mainPage.CreateButton("Teleport 10k", TYPE.CODE).activationCode = () =>
            {
                CastleMinerZGame.Instance.LocalPlayer.LocalPosition = new Vector3(CastleMinerZGame.Instance.LocalPlayer.LocalPosition.X+10000, CastleMinerZGame.Instance.LocalPlayer.LocalPosition.Y, CastleMinerZGame.Instance.LocalPlayer.LocalPosition.Z);
                return true;
            };
            mainPage.CreateButton("Leave Server", TYPE.CODE).activationCode = () => { CastleMinerZGame.Instance.EndGame(true); return true; };
            menu.OpenPage(mainPage);
            // LOCALPLAYER PAGE
            MenuButton _god = localPage.CreateButton("God", TYPE.CODE);
            _god.setToggleableButton(true);
            _god.activationCode = () => { CastleMinerZGame.Instance.LocalPlayer.GodMode = true; return true; };
            _god.deactivationCode = () => { CastleMinerZGame.Instance.LocalPlayer.GodMode = false; return true; };

            MenuButton _flight = localPage.CreateButton("Flight", TYPE.CODE);
            _flight.setToggleableButton(true);
            _flight.activationCode = () => { CastleMinerZGame.Instance.LocalPlayer.FlyMode = true; return true; };
            _flight.deactivationCode = () => { CastleMinerZGame.Instance.LocalPlayer.FlyMode = false; return true; };

            // LOCALPLAYER INVENTORY
            Page inventoryPage = menu.CreatePage("Local Inventory", localPage, true);
            Page inventoryGivePage = menu.CreatePage("Give Item", inventoryPage, true);
            foreach(InventoryItemIDs id in Enum.GetValues(typeof(InventoryItemIDs)))
            {
                inventoryGivePage.CreateButton(id.ToString(), TYPE.CODE).activationCode = () => { InventoryItem item = DNA.CastleMinerZ.Inventory.InventoryItem.CreateItem(id, 0); item.StackCount = item.MaxStackCount; DNA.CastleMinerZ.CastleMinerZGame.Instance.LocalPlayer.PlayerInventory.AddInventoryItem(item, false); return true; };
            }

            MenuButton _dropInv = inventoryPage.CreateButton("Drop Inventory", TYPE.CODE);
            _dropInv.activationCode = () => { DNA.CastleMinerZ.CastleMinerZGame.Instance.LocalPlayer.PlayerInventory.DropAll(true); return true; };

            MenuButton _clearInv = inventoryPage.CreateButton("Reset Inventory", TYPE.CODE);
            _clearInv.activationCode = () => { DNA.CastleMinerZ.CastleMinerZGame.Instance.LocalPlayer.PlayerInventory.SetDefaultInventory(); return true; };

            Page animationPage = menu.CreatePage("Animation", localPage, true);
            Dictionary<string, int> animationNames = new Dictionary<string, int>()
            {
                {"Die", 5 }, {"Grenade_Cook", 3}, {"Grenade_Throw", 3}, {"Swim", 0}, {"Stand", 0 }, {"Walk", 0}, {"Run", 0}
            };
            foreach(string name in animationNames.Keys)
            {
                animationPage.CreateButton(name, TYPE.CODE).activationCode = () =>
                {
                    animationNames.TryGetValue(name, out int channelId);
                    CastleMinerZGame.Instance.LocalPlayer.Avatar.Animations.Play(name, channelId, TimeSpan.FromSeconds(1));
                    return true;
                };
            }

            MenuButton _respawn = localPage.CreateButton("Respawn", TYPE.CODE);
            _respawn.activationCode = () => { DNA.CastleMinerZ.UI.InGameHUD.Instance.RespawnPlayer(); return true; };

            // PLAYERS PAGE
            playersPage.onOpenFunc = () =>
            {
                if (CastleMinerZGame.Instance.CurrentNetworkSession != null)
                {
                    foreach (NetworkGamer networkGamer in CastleMinerZGame.Instance.CurrentNetworkSession.AllGamers)
                    {
                        if (!this.Gamers.ContainsKey(networkGamer))
                        {
                            this.createPlayerPage(networkGamer, playersPage);
                        }
                        if (this.Gamers.Count != 0)
                        {
                            if (networkGamer.HasLeftSession || networkGamer == null || networkGamer.IsDisposed)
                            {
                                MenuButton button;
                                if (this.Gamers.TryGetValue(networkGamer, out button))
                                {
                                    playersPage.RemoveButton(button);
                                }
                                this.Gamers.Remove(networkGamer);
                            }
                        }
                    }
                    playersPage.CreateButton("Refresh List", TYPE.CODE).activationCode = () => { playersPage.onOpenFunc(); return true; };
                }
                else
                {
                    if (this.Gamers.Count != 0)
                    {
                        {
                            foreach (NetworkGamer networkGamer in this.Gamers.Keys)
                            {
                                MenuButton button;
                                if (this.Gamers.TryGetValue(networkGamer, out button))
                                {
                                    playersPage.RemoveButton(button);
                                }
                                this.Gamers.Remove(networkGamer);
                            }
                        }
                    }
                }
                return true;
            };
            // WEAPON PAGE
            MenuButton _ammo = weaponPage.CreateButton("Infinite Ammo", TYPE.CODE);
            _ammo.setToggleableButton(true);
            _ammo.activationCode = () => { CastleMinerZGame.Instance.LocalPlayer._infAmmo = true; return true; };
            _ammo.deactivationCode = () => { CastleMinerZGame.Instance.LocalPlayer._infAmmo = false; return true; };
            MenuButton _durability = weaponPage.CreateButton("Infinite Durability", TYPE.CODE);
            _durability.setToggleableButton(true);
            _durability.activationCode = () => { CastleMinerZGame.Instance.LocalPlayer._infDurability = true; return true; };
            _durability.deactivationCode = () => { CastleMinerZGame.Instance.LocalPlayer._infDurability = false; return true; };
            MenuButton _shootRocket = weaponPage.CreateButton("Rocket Ammo", TYPE.CODE);
            _shootRocket.setToggleableButton(true);
            _shootRocket.activationCode = () => { CastleMinerZGame.Instance.LocalPlayer._shootRockets = true; return true; };
            _shootRocket.deactivationCode = () => { CastleMinerZGame.Instance.LocalPlayer._shootRockets = false; return true; };
            MenuButton _noRecoil = weaponPage.CreateButton("No Recoil", TYPE.CODE);
            _noRecoil.setToggleableButton(true);
            _noRecoil.activationCode = () => { CastleMinerZGame.Instance.LocalPlayer._noRecoil = true; return true; };
            _noRecoil.deactivationCode = () => { CastleMinerZGame.Instance.LocalPlayer._noRecoil = false; return true; };
            MenuButton _highAmmo = weaponPage.CreateButton("Max Ammo", TYPE.CODE);
            _highAmmo.activationCode = () =>
            {
                if(CastleMinerZGame.Instance.LocalPlayer.PlayerInventory.TrayManager.GetItemFromCurrentTray(CastleMinerZGame.Instance.LocalPlayer.PlayerInventory.SelectedInventoryIndex) is GunInventoryItem)
                {
                    ((GunInventoryItem)CastleMinerZGame.Instance.LocalPlayer.PlayerInventory.TrayManager.GetItemFromCurrentTray(CastleMinerZGame.Instance.LocalPlayer.PlayerInventory.SelectedInventoryIndex)).RoundsInClip = int.MaxValue;
                };
                return true;
            };
            MenuButton _fireRate = weaponPage.CreateButton("Rapid Fire", TYPE.CODE);
            _fireRate.setToggleableButton(true);
            _fireRate.activationCode = () =>
            {
                CastleMinerZGame.Instance.LocalPlayer._rapidFire = true;
                return true;
            };
            _fireRate.deactivationCode = () => {
                CastleMinerZGame.Instance.LocalPlayer._rapidFire = false;
                return true;
            };
            //InventoryItem.RegisterItemClass(new CustomSMGInventoryItemClass(InventoryItemIDs.COOLSMG, ToolMaterialTypes.BloodStone, "FUCK YOU", "SHOOTS FAST" + ". " + "KILLS SHIT", 100f, 0f, InventoryItem.GetClass(InventoryItemIDs.DiamondBullets)));
            // WORLD PAGE
            worldPage.CreateButton("Time Of Day+", TYPE.CODE).activationCode = () =>
            {
                CastleMinerZGame.Instance.GameScreen.Day = CastleMinerZGame.Instance.GameScreen.Day + 0.1F;
                return true;
            };
            worldPage.CreateButton("Time Of Day-", TYPE.CODE).activationCode = () =>
            {
                CastleMinerZGame.Instance.GameScreen.Day = CastleMinerZGame.Instance.GameScreen.Day - 0.1F;
                return true;
            };
            MenuButton _infiniteResource = worldPage.CreateButton("Infinite Resources", TYPE.CODE);
            _infiniteResource.setToggleableButton(true);
            _infiniteResource.activationCode = () =>
            {
                CastleMinerZGame.Instance.InfiniteResourceMode = true;
                return true;
            };
            _infiniteResource.deactivationCode = () =>
            {
                CastleMinerZGame.Instance.InfiniteResourceMode = false;
                return true;
            };
            Page spawnPage = menu.CreatePage("Spawn Enemy", worldPage, true);
            spawnPage.CreateButton("Spawn Horde - 10", TYPE.CODE).activationCode = () =>
            {
                WorldUtils.Mobs.spawnHorde(CastleMinerZGame.Instance.LocalPlayer, 10);
                return true;
            };
            spawnPage.CreateButton("Spawn Horde - 50", TYPE.CODE).activationCode = () =>
            {
                WorldUtils.Mobs.spawnHorde(CastleMinerZGame.Instance.LocalPlayer, 50);
                return true;
            };
            spawnPage.CreateButton("Spawn Horde - 100", TYPE.CODE).activationCode = () =>
            {
                WorldUtils.Mobs.spawnHorde(CastleMinerZGame.Instance.LocalPlayer, 100);
                return true;
            };
            spawnPage.CreateButton("Spawn Horde - 200", TYPE.CODE).activationCode = () =>
            {
                WorldUtils.Mobs.spawnHorde(CastleMinerZGame.Instance.LocalPlayer, 200);
                return true;
            };
            foreach (EnemyTypeEnum enemyType in Enum.GetValues(typeof(EnemyTypeEnum)))
            {
                spawnPage.CreateButton(enemyType.ToString(), TYPE.CODE).activationCode = () => { DNA.CastleMinerZ.AI.EnemyManager.Instance.SpawnEnemy(CastleMinerZGame.Instance.LocalPlayer.LocalPosition, enemyType, CastleMinerZGame.Instance.LocalPlayer.LocalPosition, 1); return true; };
            }
            Page spawnPageDragon = menu.CreatePage("Spawn Dragon", worldPage, true);
            spawnPageDragon.CreateButton("Kill Dragon", TYPE.CODE).activationCode = () =>
            {
                DNA.CastleMinerZ.Net.KillDragonMessage.Send((LocalNetworkGamer)CastleMinerZGame.Instance.LocalPlayer.Gamer, CastleMinerZGame.Instance.LocalPlayer.LocalPosition, CastleMinerZGame.Instance.LocalPlayer.Gamer.Id, InventoryItemIDs.AssultRifle);
                return true;
            };
            foreach(DragonTypeEnum dragonType in Enum.GetValues(typeof(DragonTypeEnum)))
            {
                spawnPageDragon.CreateButton(dragonType.ToString(), TYPE.CODE).activationCode = () => { DNA.CastleMinerZ.Net.SpawnDragonMessage.Send((LocalNetworkGamer)CastleMinerZGame.Instance.LocalPlayer.Gamer, CastleMinerZGame.Instance.LocalPlayer.Gamer.Id, dragonType, false, -1f); ; return true; };
            };
            worldPage.CreateButton("Force Host", TYPE.CODE).activationCode = () =>
            {
                CastleMinerZGame.Instance.CurrentNetworkSession.AllowHostMigration = true;
                DNA.CastleMinerZ.Net.AppointServerMessage.Send((LocalNetworkGamer) CastleMinerZGame.Instance.LocalPlayer.Gamer, CastleMinerZGame.Instance.LocalPlayer.Gamer.Id);
                return true;
            };
            Page blockEdit = menu.CreatePage("Block Edit", worldPage, true);
            IntVector3 point1 = new IntVector3(0,0, 0), point2 = new IntVector3(0,0,0);
            blockEdit.CreateButton("Set Point 1", TYPE.CODE).activationCode = () => {
                point1 = (IntVector3)CastleMinerZGame.Instance.LocalPlayer.LocalPosition;
                return true;
            };
            blockEdit.CreateButton("Set Point 2", TYPE.CODE).activationCode = () => {
                point2 = (IntVector3)CastleMinerZGame.Instance.LocalPlayer.LocalPosition;
                return true;
            };
            Page editBlocks = menu.CreatePage("Edit", blockEdit, true);

            foreach(BlockTypeEnum blockType in Enum.GetValues(typeof(BlockTypeEnum)))
            {
                editBlocks.CreateButton(blockType.ToString(), TYPE.CODE).activationCode = () => {
                    WorldUtils.Blocks.massEditBlocks((LocalNetworkGamer)CastleMinerZGame.Instance.LocalPlayer.Gamer, point1, point2, blockType);
                    return true;
                };
            }
            MenuButton _allRocket = worldPage.CreateButton("All Rocket", TYPE.CODE);
            _allRocket.setToggleableButton(true);
            _allRocket.activationCode = () =>
            {
                CastleMinerZGame.Instance._allShootRockets = true;
                return true;
            };
            _allRocket.deactivationCode = () =>
            {
                CastleMinerZGame.Instance._allShootRockets = false;
                return false;
            };
            //DNA.CastleMinerZ.AI.EnemyManager.Instance.SpawnEnemy

            // SOUND PAGE
            foreach (String name in songNames)
            {
                soundPage.CreateButton(name, TYPE.CODE).activationCode = () => { CastleMinerZGame.Instance.PlayMusic(name); return true; };
            };
        }
    }
}

