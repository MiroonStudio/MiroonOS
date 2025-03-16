namespace MiroonOS.MiroonUtils.UIUtils
{
    public class BaseUI
    {
        /// <summary>
        /// 界面的子面板面板
        /// </summary>
        public List<BasePanel> SubPanels = [];

        /// <summary>
        /// UI的名字
        /// </summary>
        public string UIName;

        /// <summary>
        /// 绘制子面板
        /// </summary>
        public void DrawSubPanels(SpriteBatch spriteBatch)
        {
            foreach (BasePanel basePanel in SubPanels)
            {
                if (basePanel != null)
                {
                    if(basePanel.CanDraw)
                    {
                        basePanel.Draw(spriteBatch);
                    }
                    basePanel.DrawSubButton(spriteBatch);
                }
            }
        }

        /// <summary>
        /// 添加一个新面板
        /// </summary>
        /// <param name="texture">贴图</param>
        /// <param name="PanleRot">朝向</param>
        /// <param name="Pos">位置</param>
        /// <param name="Size">大小</param>
        /// <param name="Visible">启用？</param>
        /// <param name="CanDraw">绘制？</param>
        /// <param name="FullScreen">铺满全屏？</param>
        /// <param name="PanelName">名字</param>
        /// <param name="DrawColor">颜色</param>
        /// <returns>成功返回实例，不成功返回 null 并在终端留下痕迹</returns>
        public BasePanel NewPanel(Texture2D texture, float PanleRot, Vector2 Pos, Vector2 Size, bool Visible, bool CanDraw, bool FullScreen, string PanelName, Color DrawColor)
        {
            if (FindSubPanels(PanelName) != null)
            {
                Console.WriteLine($"来自 {this} 的错误：添加新面板“{PanelName}”时发现其 UI 列表“{this}”中已存在同名面板");
                return null;
            }
            BasePanel basePanel = new()
            {
                Active = true,
                CanDraw = CanDraw,
                FullScreen = FullScreen,
                Panelcolor = DrawColor,
                PanelSize = Size,
                Visible = Visible,
                PanelCenter = Pos,
                PanelName = PanelName,
                PanelRot = PanleRot,
                PanelTex = texture,
            };
            SubPanels.Add(basePanel);
            return basePanel;
        }

        /// <summary>
        /// 寻找一个面板的方法
        /// </summary>
        /// <param name="PanelName">面板名字</param>
        /// <returns>找到了就返回实例，没找到就返回Null</returns>
        public BasePanel FindSubPanels(string PanelName)
        {
            foreach (BasePanel basePanel in SubPanels)
            {
                if (basePanel is not null && basePanel.PanelName == PanelName)
                {
                    return basePanel;
                }
            }
            return null;
        }
    }
}
