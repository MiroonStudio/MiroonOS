namespace MiroonOS.MiroonUtils.CountBuffUtils
{
    /// <summary>
    /// 指向一个实例的一个类，用于存储所携带的可以叠加层级的 Buff
    /// </summary>
    public class CountBuffList
    {
        /// <summary>
        /// 携带的所有可以叠加层级的 Buff
        /// </summary>
        public List<ModCountBuff> BuffType = [];

        /// <summary>
        /// 这个数组的拥有者索引
        /// </summary>
        public Entity Target;

        /// <summary>
        /// 添加索引
        /// </summary>
        /// <param name="target">索引</param>
        public CountBuffList(NPC target)
        {
            Target = target;
        }

        public CountBuffList()
        {
        }
    }
}
