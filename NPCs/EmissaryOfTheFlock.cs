using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using System.Collections.Generic;
using Terraria.Utilities;
using Terraria.ModLoader.IO;
using Platinum.Items.Wearable.Vanity;

namespace Platinum.NPCs {
    // [AutoloadHead] and npc.townNPC are extremely important and absolutely both necessary for any Town NPC to work at all.
    [AutoloadHead]
    public class EmissaryOfTheFlock : ModNPC {
        public override string Texture => "Platinum/NPCs/EmissaryOfTheFlock";
        public int[] questItems = { ItemID.TrashCan };
        public int[] rewards = { ItemType<Omega>() };

        public override bool Autoload(ref string name) {
            name = "Brother of the Merchant of Travels";
            return mod.Properties.Autoload;
        }
        public static List<Item> shopItems = new List<Item>();

        public static void UpdateMerchandise() {
            if (Main.dayTime && Main.time == 0) {
                shopItems = CreateNewShop();
            }
        }
        public static List<Item> CreateNewShop() {
            var itemIds = new List<int>();
            for (int i = 0; i < 30; i++) {
                itemIds.Add(Main.rand.Next(-24, 3930));
            }
            var items = new List<Item>();
            foreach (int itemId in itemIds) {
                Item item = new Item();
                try {
                    item.SetDefaults(itemId);
                } catch {
                    // If the item does not exist, just move on. Suppress error.
                    continue;
                }
                
                items.Add(item);
            }
            return items;
        }

        public override void SetStaticDefaults() {
            // DisplayName automatically assigned from .lang files, but the commented line below is the normal approach.
            DisplayName.SetDefault("Brother of the Merchant of Travels");
            Main.npcFrameCount[npc.type] = 25;
            NPCID.Sets.ExtraFramesCount[npc.type] = 9;
            NPCID.Sets.AttackFrameCount[npc.type] = 4;
            NPCID.Sets.DangerDetectRange[npc.type] = 700;
            NPCID.Sets.AttackType[npc.type] = 0; //throwing
            NPCID.Sets.AttackTime[npc.type] = 90;
            NPCID.Sets.AttackAverageChance[npc.type] = 30;
            NPCID.Sets.HatOffsetY[npc.type] = 4;
        }

        public override void SetDefaults() {
            npc.townNPC = true;
            npc.friendly = true;
            npc.dontTakeDamageFromHostiles = true;
            npc.width = 18;
            npc.height = 40;
            npc.aiStyle = 7;
            npc.damage = 10;
            npc.defense = 15;
            npc.lifeMax = 250;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.knockBackResist = 0.5f;
            animationType = NPCID.Merchant;
            if(shopItems.Count==0)
                shopItems = CreateNewShop();
        }

        public override bool CanTownNPCSpawn(int numTownNPCs, int money) {
            for (int k = 0; k < 255; k++) {
                Player player = Main.player[k];
                if (!player.active) {
                    continue;
                }
            }
            return false;
        }

        public static TagCompound Save() {
            return new TagCompound {
                ["shopItems"] = shopItems
            };
        }

        public static void Load(TagCompound tag) {
            shopItems = tag.Get<List<Item>>("shopItems");
        }

        public override string TownNPCName() {
            switch( WorldGen.genRand.Next(8) ) {
                case 0:
                    return "Galactic Knight";
                case 1:
                    return "17";
                case 2:
                    return "Shadow bounty hunter";
                case 3:
                    return "Twilight darkness";
                case 5:
                    return "Spaceman sam";
                case 6:
                    return "First engineer officer roger";
                case 7:
                    return "Scientist Terry";
                case 8:
                default:
                    return "Regular old Ed";
            }
        }

		public override string GetChat()
		{
			WeightedRandom<string> chat = new WeightedRandom<string>();
			/*chat.Add("The birds. They are coming.");
			chat.Add("Ravens. Robins. Gulls.");
			chat.Add("They are calling for you.");
			chat.Add("The flock guides us all");*/ //Shifting from Emissary if the flock
            int guide = NPC.FindFirstNPC(NPCID.Guide);
            if(guide>-1) {
                chat.Add("You know I honestly feel bad for the guide: voodoo dolls that are being tossed into the fire, I mean come on those are perfect for shop displays, i could see signs saying, \"New fashionable doll in stores today\", you know?");
            }
            int nurse = NPC.FindFirstNPC(NPCID.Nurse);
            if(nurse>-1) {
                chat.Add("you know I don’t need the nurse to help because I have my . . . uh, well \"ways\"");
                if(Vector2.Distance(Main.npc[nurse].Center,this.npc.Center) < 30*16) { //*16 to convert from 30 tile coordinates to 480 world coordinates
                    chat.Add($"Hey {Main.npc[nurse].GivenName}, how much for the healing potions. Not for sale? Well, what about I give you 5 platinum coins then.");
                }
            }
            int merchant = NPC.FindFirstNPC(NPCID.Merchant);
            if(merchant>-1) {
                chat.Add("Hey did you know I heard that the reason the merchant can sell infinite piggy banks is that he has a vortex in his pockets, but don’t ask me how I know this, because a collector never tells his secrets, unless for a really high price.");
            }
			return chat; // chat is implicitly cast to a string. You can also do "return chat.Get();" if that makes you feel better
		}

        public override void SetChatButtons(ref string button, ref string button2) {
            button = Language.GetTextValue("LegacyInterface.28");
            button2 = "Quest";
        }
        public override void OnChatButtonClicked(bool firstButton, ref bool shop) {
            if (firstButton) {
                shop = true;
            } else {
                handleQuest();
            }
        }

        private void handleQuest() {
            platinumPlayer player = Main.player[Main.myPlayer].GetModPlayer<platinumPlayer>();
            if(player.hasQuest==false) {
                //give new quest
                int item = WorldGen.genRand.Next<int>(questItems);
                Main.npcChatText = $"So you see, I've been itching to get my hands on a {Lang.GetItemNameValue(item)} [i:{item}]. I'll get you somthing special if you can get me one.";
                player.myQuestItem = item;
                player.hasQuest = true;
            } else if(player.player.HasItem(player.myQuestItem)) {//complete current quest
                Main.npcChatText = $"Oh my! You found one! I am very greatful. Here! Here is your reward. I am going to have a lot of fun with this.";
                int questItem = Main.LocalPlayer.FindItem(player.myQuestItem);
                player.player.inventory[questItem].TurnToAir();
                player.player.QuickSpawnItem(WorldGen.genRand.Next<int>(rewards));
                player.hasQuest = false;
            } else {
                Main.npcChatText = $"Have you found a {Lang.GetItemNameValue(player.myQuestItem)} [i:{player.myQuestItem}]? No? Find me when you have one!";
            }
        }

        public override void SetupShop(Chest shop, ref int nextSlot) {
            foreach(Item item in shopItems) {
                if (item == null || item.type == ItemID.None)
                    continue;
                shop.item[nextSlot].SetDefaults(item.type);
                shop.item[nextSlot].shopCustomPrice = 5000000;
                nextSlot++;
            }
        }

        // Make this Town NPC teleport to the King and/or Queen statue when triggered.
        public override bool CanGoToStatue(bool toKingStatue) {
            return false;
        }

        // Create a square of pixels around the NPC on teleport.
        public void StatueTeleport() {
            for (int i = 0; i < 30; i++) {
                Vector2 position = Main.rand.NextVector2Square(-20, 21);
                if (Math.Abs(position.X) > Math.Abs(position.Y)) {
                    position.X = Math.Sign(position.X) * 20;
                }
                else {
                    position.Y = Math.Sign(position.Y) * 20;
                }
            }
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback) {
            damage = 20;
            knockback = 4f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown) {
            cooldown = 30;
            randExtraCooldown = 30;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
		{
			projType = ProjectileID.CopperCoinsFalling;
			attackDelay = 1;
		}

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset) {
            multiplier = 12f;
            randomOffset = 2f;
        }
    }
}