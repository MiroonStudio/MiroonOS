using System.Collections.Concurrent;
using System.Linq;

namespace MiroonOS.MiroonUtils.CountBuffUtils
{
    public class CountBuffGlobalNPC : GlobalNPC
    {
        /// <summary>
        /// NPC 携带的可叠加 Buff 词典
        /// </summary>
        public static ConcurrentDictionary<NPC, CountBuffList> CountBuffs = [];

        public override bool InstancePerEntity => true;

        public static void AddBuff<T>(NPC target, int Time, T buff) where T : ModCountBuff
        {
            // 检查目标 NPC 是否有效
            if (target == null || !target.active)
            {
                return;
            }

            if (CountBuffs.TryGetValue(target, out CountBuffList existingBuffList))
            {
                foreach (ModCountBuff existingBuff in existingBuffList.BuffType)
                {
                    if (ModCountBuff.GetModBuffType(existingBuff.GetType()) == ModCountBuff.GetModBuffType(buff.GetType()) && (existingBuff.MaxBuffCount > existingBuff.BuffCount + 1 || existingBuff.MaxBuffCount == -1))
                    {
                        buff.TimePerCount = Time;
                        existingBuff.Time = Time;
                        existingBuff.BuffCount++;
                        return;
                    }
                    else if (ModCountBuff.GetModBuffType(existingBuff.GetType()) == ModCountBuff.GetModBuffType(buff.GetType()))
                    {
                        buff.TimePerCount = Time;
                        existingBuff.Time = Time;
                        return;
                    }

                }
                buff.TimePerCount = Time;
                buff.Active = true;
                buff.Time = Time;
                buff.BuffCount = 1;
                existingBuffList.BuffType.Add(buff);
                buff.OnAddNPC(target);
            }
            else
            {
                CountBuffList newBuffList = new CountBuffList(target);
                buff.Active = true;
                buff.Time = Time;
                buff.BuffCount = 1;
                buff.TimePerCount = Time;
                newBuffList.BuffType.Add(buff);
                CountBuffs[target] = newBuffList;
            }
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (CountBuffs.TryGetValue(npc, out CountBuffList bufflist))
            {
                for (int i = 0; i < bufflist.BuffType.Count; i++)
                {
                    ModCountBuff modbuff = bufflist.BuffType[i];
                    if (modbuff.Time <= 0)
                    {
                        if (modbuff.BuffCount > 1)
                        {
                            modbuff.Time = modbuff.GetTimePerCount();
                            modbuff.BuffCount--;
                        }
                        else
                        {
                            if (modbuff.PreEnd(npc))
                            {
                                modbuff.End(npc);
                                modbuff.Time = 0;
                                modbuff.BuffCount = 0;
                                modbuff.Active = false;
                                modbuff.PostEnd(npc);
                                bufflist.BuffType.RemoveAt(i);
                            }
                        }
                    }
                    else
                    {
                        modbuff.Active = true;
                        modbuff.Draw(npc, spriteBatch, i, bufflist.BuffType.Count);
                        modbuff.Update(npc, modbuff.Time);
                        modbuff.Time--;
                    }
                }
            }
        }
    }

    public class CountBuffSysuem : ModSystem
    {
        /// <summary>
        /// 用于存储所有的 Buff 类
        /// </summary>
        public static List<Type> modCountBuffs = [];

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="Buff"></param>
        public static void AddBuffType(Type Buff)
        {
            if (!modCountBuffs.Contains(Buff))
            {
                modCountBuffs.Add(Buff);
            }
        }


        public override void Load()
        {
            // 获取当前程序集中所有派生类
            var derivedTypes = typeof(ModCountBuff).Assembly.GetTypes()
                .Where(type => type.IsSubclassOf(typeof(ModCountBuff)) && !type.IsAbstract);

            int nextId = 1;
            foreach (var derivedType in derivedTypes)
            {
                // 注册每个派生类的 BuffType
                ModCountBuff.RegisterBuffType(derivedType, nextId++);
                Console.WriteLine("TerrCorp 注册 BUff " + derivedType.Name + " with ID: " + ModCountBuff.GetModBuffType(derivedType));
            }
        }
    }
}