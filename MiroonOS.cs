global using Terraria.ModLoader;
global using System;
global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
global using System.Collections.Generic;
global using Terraria;
global using Terraria.GameContent;
using MiroonOS.MiroonUtils.UIUtils;

namespace MiroonOS
{
	/// <summary>
	/// ���� OS ��ģ������
	/// </summary>
	public class MiroonOS : Mod
	{
        public override void Unload()
        {
            UIManager.BaseUIs = null;
        }
    }
}
