namespace MiroonOS.MiroonUtils.UIUtils
{
    /// <summary>
    /// 基本面板类
    /// </summary>
    public class BasePanel
    {
        /// <summary>
        /// 每帧更新的函数
        /// </summary>
        public Action Update;

        /// <summary>
        /// 更新前的函数
        /// </summary>
        public Func<bool> PreUpdate;

        /// <summary>
        /// 每帧更新后的函数
        /// </summary>
        public Action PostUpdate;

        /// <summary>
        /// 面板位置
        /// </summary>
        public Vector2 Pos;

        /// <summary>
        /// 正常时的纹理
        /// </summary>
        public Texture2D PanelTex;

        /// <summary>
        /// 面板名称
        /// </summary>
        public string PanelName;

        /// <summary>
        /// 子按钮列表
        /// </summary>
        public List<BaseButton> SubButtonsList = [];

        /// <summary>
        /// 是否占满屏幕绘制（默认false）
        /// </summary>
        public bool FullScreen = false;

        /// <summary>
        /// 面板大小
        /// </summary>
        public Vector2 Size;

        /// <summary>
        /// 面板颜色
        /// </summary>
        public Color Panelcolor;

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Visible = true;

        /// <summary>
        /// 面板角度
        /// </summary>
        public float PanelRot = 0;

        /// <summary>
        /// 是否可以看见
        /// </summary>
        public bool CanDraw = true;

        /// <summary>
        /// 是否活跃（不活跃就被销毁）
        /// </summary>
        public bool Active = true;

        /// <summary>
        /// 绘制面板
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            Color color = Visible ? Panelcolor : Color.Gray;
            spriteBatch.Draw(PanelTex, Pos, null, color, PanelRot, new Vector2(PanelTex.Width / 2, PanelTex.Height / 2), Size, SpriteEffects.None, 1f);
            foreach (BaseButton baseButton in SubButtonsList)
            {
                if (baseButton != null && baseButton.ButtonActive && baseButton.ButtonCanDraw)
                {
                    baseButton.Draw(spriteBatch);
                }
            }
        }

        /// <summary>
        /// 绘制子按钮
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawSubButton(SpriteBatch spriteBatch)
        {
            foreach (BaseButton baseButton in SubButtonsList)
            {
                if (baseButton != null && baseButton.ButtonActive && baseButton.ButtonCanDraw)
                {
                    baseButton.Draw(spriteBatch);
                }
            }
        }

        /// <summary>
        /// 寻找一个按钮的方法
        /// </summary>
        /// <param name="PanelName">面板名字</param>
        /// <returns>找到了就返回实例，没找到就返回 Null</returns>
        public BaseButton FindSubButton(string ButtonName)
        {
            foreach (BaseButton baseButton in SubButtonsList)
            {
                if (baseButton is not null && baseButton.ButtonName == ButtonName)
                {
                    return baseButton;
                }
            }
            return null;
        }

        /// <summary>
        /// 添加一个新的按钮
        /// </summary>
        /// <param name="Size">大小</param>
        /// <param name="color">颜色</param>
        /// <param name="Name">名称</param>
        /// <param name="ShowText">显示的文本</param>
        /// <param name="CanDraw">是否绘制</param>
        /// <param name="Visible">是否启用</param>
        /// <param name="PlayerSound">碰到时播放的声音</param>
        /// <param name="Rot">朝向</param>
        /// <param name="texture">正常时的贴图</param>
        /// <param name="texture_Hover">悬停时的贴图</param>
        /// <returns>成功返回实例，不成功返回 null 并在终端留下痕迹</returns>
        public BaseButton NewButton(Vector2 Size, Color color, string Name, string ShowText, bool CanDraw, bool Visible, string PlayerSound, float Rot, Texture2D texture, Texture2D texture_Hover)
        {
            if (FindSubButton(Name) is null)
            {
                Console.WriteLine($"来自 {this} 的错误：添加新面板“{PanelName}”时发现其 UI 列表“{this}”中已存在同名面板");
                return null;
            }

            BaseButton baseButton = new()
            {
                ButtonActive = true,
                ButtonCanDraw = CanDraw,
                ButtonVisible = Visible,
                ButtonName = Name,
                ButtonShowText = ShowText,
                ButtonSize = Size,
                ButtonColor = color,
                ButtonPlaySound = PlayerSound,
                ButtonRot = Rot,
                ButtonTex = texture,
                ButtonTex_Hover = texture_Hover
            };
            SubButtonsList.Add(baseButton);
            return baseButton;
        }
    }
}
