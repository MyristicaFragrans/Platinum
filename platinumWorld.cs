using Platinum.NPCs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Platinum {
    class platinumWorld : ModWorld {
		public override void Initialize() {
		}
		public override TagCompound Save() {

			return new TagCompound {
				["emissary"] = EmissaryOfTheFlock.Save()
			};
		}
		public override void Load(TagCompound tag) {
			EmissaryOfTheFlock.Load(tag.GetCompound("emissary"));
		}
		public override void PreUpdate() {
			EmissaryOfTheFlock.UpdateMerchandise();
		}
	}
}
