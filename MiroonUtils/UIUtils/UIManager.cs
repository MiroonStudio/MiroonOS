using Terraria.Audio;

namespace MiroonOS.MiroonUtils.UIUtils
{
    public class UIManager : ModSystem
    {
        /// <summary>
        /// 所有基本 UI
        /// </summary>
        public static List<BaseUI> BaseUIs = []; // 初始化列表

        // 静态变量用于保存当前悬停的按钮和被点击的按钮
        public static BaseButton HoverButton = null;
        public static BaseButton ClickedButton = null;

        // 两个布尔变量用于追踪鼠标左键和右键的状态
        /// <summary>
        /// 上一次左键未按下状态
        /// </summary>
        private static bool LastLeftUnPressed = false; 
        /// <summary>
        /// 当前帧是否发生了左键点击
        /// </summary>
        public static bool LeftClicked = false;
        /// <summary>
        /// 上一次右键未按下状态
        /// </summary>
        private static bool LastRightUnPressed = false;
        /// <summary>
        /// 当前帧是否发生了右键点击
        /// </summary>
        public static bool RightClicked = false; 

        /// <summary>
        /// 绘制界面的方法，遍历所有界面元素并调用它们的DrawSubPanels方法进行绘制。
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch实例，用于图形渲染</param>
        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            // 使用foreach循环遍历每一个界面元素，并安全地调用其DrawSubPanels方法
            foreach (var baseUI in BaseUIs)
            {
                baseUI?.DrawSubPanels(spriteBatch); // 如果baseUI不为null，则调用其绘制方法
            }
        }

        /// <summary>
        /// 检测点击事件的方法
        /// </summary>
        /// <param name="lastUnPressed">引用参数，表示上一次按键未按下的状态</param>
        /// <param name="currentRelease">当前帧按键是否释放的状态</param>
        /// <returns>返回true如果检测到一次有效的点击，否则返回false</returns>
        public static bool DetectClick(ref bool lastUnPressed, bool currentRelease)
        {
            // 如果当前帧按键已释放，则重置lastUnPressed标志位并返回false表示没有新的点击发生
            if (currentRelease)
            {
                lastUnPressed = true;
                return false;
            }
            else
            {
                // 如果按键未释放且之前已经记录了一次未按下，则认为发生了一次点击
                bool clicked = lastUnPressed;
                lastUnPressed = false; // 重置标志位
                return clicked; // 返回是否发生了点击
            }
        }
        public override void UpdateUI(GameTime gameTime)
        {
            LeftClicked = DetectClick(ref LastLeftUnPressed, Main.mouseLeftRelease);
            RightClicked = DetectClick(ref LastRightUnPressed, Main.mouseRightRelease);

            foreach (var baseUI in BaseUIs)
            {
                foreach (var basePanel in baseUI.SubPanels)
                {
                    UpdatePanels(basePanel);
                    foreach (var baseButton in basePanel.SubButtonsList)
                    {
                        if (baseButton == null) continue;
                        if(baseButton.ButtonActive && baseButton.ButtonVisible)
                        {
                            UpdateButtons(baseButton);
                            if (baseButton.ButtonCanDraw)
                            {
                                bool isClicked = baseButton.Clicked();
                                bool isHovered = baseButton.Hovered();

                                if (isClicked || isHovered)
                                {
                                    if (isClicked)
                                    {
                                        ClickedButton = baseButton;
                                        baseButton.ButtonClickedEven?.Invoke();
                                        if(baseButton.ButtonPlaySound != null)
                                        {
                                            SoundEngine.PlaySound(new SoundStyle(baseButton.ButtonPlaySound) { Volume = 1f }, Main.LocalPlayer.Center);
                                        }
                                    }

                                    if (isHovered)
                                    {
                                        HoverButton = baseButton;
                                    }

                                    goto FoundButton;
                                }
                            }
                        }
                    }
                }
            }

        FoundButton:;
        }

        /// <summary>
        /// 用于更新面板的方法
        /// </summary>
        /// <param name="basePanel"></param>
        public static void UpdatePanels(BasePanel basePanel)
        {
            if (basePanel != null && basePanel.Active && basePanel.Visible)
            {
                if (basePanel.PreUpdate != null && (bool)basePanel.PreUpdate.Invoke())
                {
                    if (basePanel.Update != null)
                    {
                        basePanel.Update?.Invoke();
                    }
                }
                if (basePanel.PostUpdate != null)
                {
                    basePanel.PostUpdate?.Invoke();
                }
            }
        }
        /// <summary>
        /// 用于更新按钮的方法
        /// </summary>
        /// <param name="baseButton"></param>
        public static void UpdateButtons(BaseButton baseButton)
        {
            if (baseButton != null && baseButton.ButtonActive && baseButton.ButtonVisible)
            {
                if (baseButton.PreUpdate != null && (bool)baseButton.PreUpdate.Invoke() == true)
                {
                    if(baseButton.Update != null)
                    {
                        baseButton.Update?.Invoke();
                    }
                }
                if (baseButton.PostUpdate != null)
                {
                    baseButton.PostUpdate?.Invoke();
                }
            }
        }


        /// <summary>
        /// 创建一个新的 UI
        /// </summary>
        /// <param name="UIName">UI 名字</param>
        /// <returns>成功返回实例，不成功返回 null 并在终端留下痕迹</returns>
        public static BaseUI NewUI(string UIName)
        {
            if (FindUI(UIName) != null)
            {
                Console.WriteLine($"来自 UIManager 的错误：添加新面板“{UIName}”时发现其 UIManager 列表中已存在同名 UI ");
                return null;
            }
            BaseUI baseUI = new()
            {
                UIName = UIName
            };
            BaseUIs.Add(baseUI);
            return baseUI;
        }

        /// <summary>
        /// 寻找一个UI的方法
        /// </summary>
        /// <param name="UIName">UI名字</param>
        /// <returns>找到了就返回实例，没找到就返回Null</returns>
        public static BaseUI FindUI(string UIName)
        {
            foreach (BaseUI baseUI in BaseUIs)
            {
                if (baseUI is not null && baseUI.UIName == UIName)
                {
                    return baseUI;
                }
            }
            return null;
        }
    }
}
