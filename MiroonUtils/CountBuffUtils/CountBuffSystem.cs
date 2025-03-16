using System.Linq;
using Terraria.DataStructures;

namespace MiroonOS.MiroonUtils.CountBuffUtils
{
    public class CountBuffGlobalNPC : GlobalNPC
    {
        public static Dictionary<NPC, CountBuffList> CountBuffs = [];

        public override bool InstancePerEntity => true;

        public static void AddBuff<T>(NPC target, int time, T buff, int count) where T : ModCountBuff
        {
            if (target == null || !target.active)
                return;

            var buffType = ModCountBuff.GetModBuffType(buff.GetType());
            if (CountBuffs.TryGetValue(target, out var existingList))
            {
                foreach (var existingBuff in existingList.BuffType.OfType<T>())
                {
                    if (ModCountBuff.GetModBuffType(existingBuff.GetType()) == buffType)
                    {
                        if (existingBuff.MaxBuffCount == -1 || existingBuff.BuffCount + count <= existingBuff.MaxBuffCount)
                        {
                            existingBuff.BuffCount += count;
                            existingBuff.Time = time;
                            existingBuff.TimePerCount = time;
                            return;
                        }
                        else
                        {
                            existingBuff.Time = time;
                            return;
                        }
                    }
                }
            }
            buff.Active = true;
            buff.Time = time;
            buff.BuffCount = count;
            buff.TimePerCount = time;
            buff.OnAddNPC(target);

            if (!CountBuffs.TryGetValue(target, out var list))
            {
                list = new CountBuffList(target);
                CountBuffs[target] = list;
            }
            list.BuffType.Add(buff);
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!CountBuffs.TryGetValue(npc, out var buffList))
                return;

            for (int i = buffList.BuffType.Count - 1; i >= 0; i--)
            {
                var modbuff = buffList.BuffType[i];
                if (modbuff == null || !modbuff.Active)
                {
                    buffList.BuffType.RemoveAt(i);
                    continue;
                }

                if (modbuff.Time <= 0)
                {
                    if (modbuff.BuffCount > 1)
                    {
                        if (modbuff.PreCountPerEnd(npc))
                        {
                            modbuff.OnCountPerEnd(npc);
                            modbuff.Time = modbuff.TimePerCount;
                            modbuff.BuffCount--;
                            modbuff.PostCountPerEnd(npc);
                        }
                    }
                    else
                    {
                        if (modbuff.PreEnd(npc))
                        {
                            modbuff.End(npc);
                            modbuff.Active = false;
                            modbuff.PostEnd(npc);
                            buffList.BuffType.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    modbuff.Update(npc, modbuff.Time);
                    modbuff.Time--;
                    modbuff.Draw(npc, i, buffList.BuffType.Count);
                }
            }
        }
    }

    public class PlayerCountBuff : ModPlayer
    {
        public List<ModCountBuff> PlayerCountBuffs = new();

        public void AddBuff<T>(T buff, int time, int count) where T : ModCountBuff
        {
            if (buff == null)
                return;

            var buffType = ModCountBuff.GetModBuffType(buff.GetType());
            foreach (var existingBuff in PlayerCountBuffs.OfType<T>())
            {
                if (ModCountBuff.GetModBuffType(existingBuff.GetType()) == buffType)
                {
                    if (existingBuff.MaxBuffCount == -1 || existingBuff.BuffCount + count <= existingBuff.MaxBuffCount)
                    {
                        existingBuff.BuffCount += count;
                        existingBuff.Time = time;
                        existingBuff.TimePerCount = time;
                        return;
                    }
                    else
                    {
                        existingBuff.Time = time;
                        return;
                    }
                }
            }
            buff.Active = true;
            buff.Time = time;
            buff.BuffCount = count;
            buff.TimePerCount = time;
            PlayerCountBuffs.Add(buff);
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            for (int i = PlayerCountBuffs.Count - 1; i >= 0; i--)
            {
                var countBuff = PlayerCountBuffs[i];
                if (countBuff == null)
                    continue;

                if (countBuff.Active)
                {
                    if (countBuff.Time <= 0)
                    {
                        if (countBuff.BuffCount >= 1)
                        {
                            if (countBuff.PreCountPerEnd(Player))
                            {
                                countBuff.OnCountPerEnd(Player);
                                countBuff.Time = countBuff.TimePerCount;
                                countBuff.BuffCount--;
                                countBuff.PostCountPerEnd(Player);
                            }
                        }
                        else
                        {
                            countBuff.Active = false;
                        }
                    }
                    else
                    {
                        countBuff.Update(Player, countBuff.Time);
                        countBuff.Draw(Player, i, PlayerCountBuffs.Count);
                        countBuff.Time--;
                    }
                }
                else
                {
                    PlayerCountBuffs.RemoveAt(i);
                }
            }
        }
    }

    public class CountBuffSysuem : ModSystem
    {
        public static List<Type> modCountBuffs = new();

        public static void AddBuffType(Type buffType)
        {
            if (!modCountBuffs.Contains(buffType))
                modCountBuffs.Add(buffType);
        }

        public override void Load()
        {
            var derivedTypes = typeof(ModCountBuff).Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(ModCountBuff)) && !t.IsAbstract);

            int nextId = 1;
            foreach (var type in derivedTypes)
            {
                ModCountBuff.RegisterBuffType(type, nextId++);
                Main.NewText($"Registered Buff {type.Name} with ID: {ModCountBuff.GetModBuffType(type)}");
            }
        }
    }
}