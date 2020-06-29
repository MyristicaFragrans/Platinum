using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Platinum {
    class platinumPlayer : ModPlayer {
        public int myQuestItem = -1;
        public bool hasQuest = false;
        public override void PreUpdate() {
        }

        //I have no clue if this is necessary (Ronan)
        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) {
            ModPacket packet = mod.GetPacket();
            packet.Write((byte)player.whoAmI);
            packet.Send(toWho, fromWho);
        }
		public override TagCompound Save() {
			return new TagCompound {
				{nameof(myQuestItem), myQuestItem},
                {nameof(hasQuest), hasQuest},
            };
		}

		public override void Load(TagCompound tag) {
			myQuestItem = tag.GetInt(nameof(myQuestItem));
            hasQuest = tag.GetBool(nameof(hasQuest));
        }
	}
}
