using MiroonOS.MiroonUtils.UIUtils;
using System;
using Terraria;

namespace MiroonOS.MiroonUtils.CountBuffUtils
{
    /// <summary>
    /// 一个抽象类，用于继承然后制作一个可以叠加层数的 Buff
    /// </summary>
    public abstract class ModCountBuff
    {
        /// <summary>
        /// 派生出的Type
        /// </summary>
        private static Dictionary<string, int> buffTypeMap = [];

        /// <summary>
        /// 设置属性时被调用一次
        /// </summary>
        public virtual void OnSetDefaults()
        {

        }

        /// <summary>
        /// 用于快速设置属性的方法
        /// </summary>
        /// <param name="_TimePerCount">每层时间</param>
        /// <param name="_MaxBuffCount">最大层数</param>
        public void SetDefaults(int _TimePerCount, int _MaxBuffCount)
        {
            TimePerCount = _TimePerCount;
            MaxBuffCount = _MaxBuffCount;
        }
        /// <summary>
        /// 获取 Buff 的类型 ID
        /// </summary>
        /// <param name="type">派生类的类型</param>
        /// <returns>BuffType</returns>
        public static int GetModBuffType(Type type)
        {
            if (buffTypeMap.TryGetValue(type.Name, out int buffType))
            {
                return buffType;
            }
            return -1; // 默认值表示未注册
        }

        /// <summary>
        /// 注册 BuffType
        /// </summary>
        /// <param name="type">派生类的类型</param>
        /// <param name="id">分配的 BuffType ID</param>
        public static void RegisterBuffType(Type type, int id)
        {
            buffTypeMap[type.Name] = id;
        }

        /// <summary>
        /// 在添加到 NPC 身上时触发一次
        /// </summary>
        public virtual void OnAddNPC(NPC npc) { }
        /// <summary>
        /// 在添加到 Player 身上时触发一次
        /// </summary>
        /// <param name="player"></param>
        public virtual void OnAddPlayer(Player player) { }


        /// <summary>
        /// 每层结束前（返回 false 可以阻止结算）
        /// </summary>
        /// <param name="npc"></param>
        public virtual bool PreCountPerEnd(NPC npc) { return true; }
        /// <summary>
        /// 在每层结束时（结算前，但是还没更新数据）
        /// </summary>
        /// <param name="npc"></param>
        public virtual void OnCountPerEnd(NPC npc) { }
        /// <summary>
        /// 每层结束后（这里已经结算并更新数据）
        /// </summary>
        /// <param name="npc"></param>
        public virtual void PostCountPerEnd(NPC npc) { }


        /// <summary>
        /// 每层结束前（返回 false 可以阻止结算）
        /// </summary>
        /// <param name="Player "></param>
        public virtual bool PreCountPerEnd(Player Player) { return true; }
        /// <summary>
        /// 在每层结束时（结算前，但是还没更新数据）
        /// </summary>
        /// <param name="Player "></param>
        public virtual void OnCountPerEnd(Player Player) { }
        /// <summary>
        /// 每层结束后（这里已经结算并更新数据）
        /// </summary>
        /// <param name="Player "></param>
        public virtual void PostCountPerEnd(Player Player) { }

        /// <summary>
        /// 在结束之前触发
        /// </summary>
        /// <returns></returns>
        public virtual bool PreEnd(NPC npc)
        {
            return true;
        }

        /// <summary>
        /// 在结束时触发
        /// </summary>
        /// <returns></returns>
        public virtual void End(NPC npc)
        {

        }

        /// <summary>
        /// 在结束后触发（这个时候 Buff 数据基本都被清空，但是还没被完全删除（没有被踢出列表））
        /// </summary>
        /// <returns></returns>
        public virtual void PostEnd(NPC npc)
        {

        }

        /// <summary>
        /// 获取 Buff 的每层时间
        /// </summary>
        /// <returns></returns>
        public int GetTimePerCount()
        {
            return TimePerCount;
        }

        /// <summary>
        /// 是否活跃
        /// </summary>
        public bool Active;

        /// <summary>
        /// 模组 buff 实例
        /// </summary>
        public ModCountBuff Buff;

        /// <summary>
        /// 这个 Buff 的层数
        /// </summary>
        public int BuffCount = 0;

        /// <summary>
        /// 这层 Buff 的剩余时间
        /// </summary>
        public int Time;

        /// <summary>
        /// 每层的时间
        /// </summary>
        public int TimePerCount;

        /// <summary>
        /// Buff 最高叠几层（-1就是无限叠，默认 -1）
        /// </summary>
        public int MaxBuffCount = -1;

        /// <summary>
        /// Buff 的贴图
        /// </summary>
        public virtual string Texture => null;

        /// <summary>
        /// 在 NPC 上时每帧调用一次
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="time"></param>
        public virtual void Update(NPC npc, int time) { }

        /// <summary>
        /// 在 NPC 上时每帧调用一次
        /// </summary>
        /// <param name="Player"></param>
        /// <param name="time"></param>
        public virtual void Update(Player Player, int time) { }

        public virtual void OnHover() 
        {
            Vector2 mousePosition = new Vector2(Main.mouseX, Main.mouseY);
            Utils.DrawBorderStringFourWay(
                Main.spriteBatch,
                FontAssets.ItemStack.Value,
                "这层 Buff 剩余时间：" + (Time / 60),
                mousePosition.X,
                mousePosition.Y,
                Color.White,
                Color.Black,
                Vector2.Zero
            );
        }

        public virtual void Draw(NPC npc, int index, int totalBuffs)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            if (texture == null) return;

            int buffWidth = texture.Width;
            int buffHeight = texture.Height;
            int spacing = 10;

            int totalWidth = totalBuffs * buffWidth + (totalBuffs - 1) * spacing;
            float offset = (totalWidth - buffWidth) / 2f;
            float horizontalOffset = index * (buffWidth + spacing) - offset;

            Vector2 position = new Vector2(
                npc.Center.X - Main.screenPosition.X - buffWidth / 2 + horizontalOffset,
                npc.Center.Y - Main.screenPosition.Y + npc.height / 2 + buffHeight
            );
            EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
            Main.spriteBatch.Draw(
                texture,
                position,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0f
            );
            Utils.DrawBorderStringFourWay(
                Main.spriteBatch,
                FontAssets.ItemStack.Value,
                BuffCount.ToString(),
                position.X + buffWidth / 2 + 10,
                position.Y + buffHeight / 2,
                Color.White,
                Color.Black,
                Vector2.Zero
            );

            Vector2 mousePosition = new Vector2(Main.mouseX, Main.mouseY);

            Rectangle buffRect = new Rectangle(
                (int)position.X,
                (int)position.Y,
                buffWidth,
                buffHeight
            );

            if (buffRect.Contains((int)mousePosition.X, (int)mousePosition.Y))
            {
                OnHover();
            }
        }

        public virtual void Draw(Player player, int index, int totalBuffs)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            if (texture == null) return;

            int buffWidth = texture.Width;
            int buffHeight = texture.Height;
            int spacing = 10;

            int totalWidth = totalBuffs * buffWidth + (totalBuffs - 1) * spacing;
            float offset = (totalWidth - buffWidth) / 2f;
            float horizontalOffset = index * (buffWidth + spacing) - offset;

            Vector2 position = new Vector2(
                player.Center.X - Main.screenPosition.X - buffWidth / 2 + horizontalOffset,
                player.Center.Y - Main.screenPosition.Y + player.height / 2 + buffHeight
            );
            EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
            Main.spriteBatch.Draw(
                texture,
                position,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0f
            );

            Utils.DrawBorderStringFourWay(
                Main.spriteBatch,
                FontAssets.ItemStack.Value,
                BuffCount.ToString(),
                position.X + buffWidth / 2 + 10,
                position.Y + buffHeight / 2,
                Color.White,
                Color.Black,
                Vector2.Zero
            );

            Vector2 mousePosition = new Vector2(Main.mouseX, Main.mouseY);

            Rectangle buffRect = new Rectangle(
                (int)position.X,
                (int)position.Y,
                buffWidth,
                buffHeight
            );
            if (buffRect.Contains((int)mousePosition.X, (int)mousePosition.Y))
            {
                OnHover(); 
            }
        }
    }
}