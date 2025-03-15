using Terraria.Audio;
using Terraria;

namespace MiroonOS.MiroonUtils.UIUtils
{
    /// <summary>
    /// 基本按钮类
    /// </summary>
    public class BaseButton
    {
        /// <summary>
        /// 委托参数，用于执行按下的事件
        /// </summary>
        public Action ButtonClickedEven = null;

        /// <summary>
        /// 每帧更新的函数
        /// </summary>
        public Action Update;

        /// <summary>
        /// 更新前的函数
        /// </summary>
        public Func<bool> PreUpdate;

        public Action<SpriteBatch> DrawOther;

        /// <summary>
        /// 每帧更新后的函数
        /// </summary>
        public Action PostUpdate;

        /// <summary>
        /// 按钮位置
        /// </summary>
        public Vector2 ButtonCenter;

        /// <summary>
        /// 按钮播放的声音
        /// </summary>
        public string ButtonPlaySound;

        /// <summary>
        /// 正常时的纹理
        /// </summary>
        public Texture2D ButtonTex;

        /// <summary>
        /// 悬停时的纹理
        /// </summary>
        public Texture2D ButtonTex_Hover;

        /// <summary>
        /// 按钮大小
        /// </summary>
        public Vector2 ButtonSize = new Vector2(1f,1f);

        /// <summary>
        /// 名字
        /// </summary>
        public string ButtonName;

        /// <summary>
        /// 按钮是否被启用
        /// </summary>
        public bool ButtonVisible = true;

        /// <summary>
        /// 是否活跃（不活跃就被销毁）
        /// </summary>
        public bool ButtonActive;

        /// <summary>
        /// 是否被悬停过
        /// </summary>
        public bool HasHovered = false;

        /// <summary>
        /// 是否被悬停过
        /// </summary>
        public bool IsHove = false;

        /// <summary>
        /// 是否可以看见
        /// </summary>
        public bool ButtonCanDraw;

        /// <summary>
        /// 绘制颜色
        /// </summary>
        public Color ButtonColor;

        /// <summary>
        /// 旋转角度
        /// </summary>
        public float ButtonRot = 0f;

        /// <summary>
        /// 检测鼠标是否悬停在按钮上
        /// </summary>
        /// <returns></returns>
        public bool Hovered()
        {
            // 创建矩形区域
            Rectangle buttonRectangle = new(
                (int)(ButtonCenter.X - ButtonTex.Width / 2), 
                (int)(ButtonCenter.Y - ButtonTex.Height / 2),
                ButtonTex.Width,
                ButtonTex.Height
            );

            // 获取鼠标位置
            Point mousePosition = new(Main.mouseX, Main.mouseY);

            // 检测鼠标是否在矩形区域内
            return buttonRectangle.Contains(mousePosition);
        }

        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            Color color = ButtonVisible ? ButtonColor : Color.Gray;
            Texture2D texture = Hovered() ? ButtonTex_Hover : ButtonTex;

            Vector2 drawPosition = new(
                ButtonCenter.X,
                ButtonCenter.Y
            );

            EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
            Main.spriteBatch.Draw(
                texture,
                drawPosition,
                null,
                color,
                ButtonRot,
                new Vector2(texture.Width / 2, texture.Height / 2),
                ButtonSize,
                SpriteEffects.None,
                1f
            );
            EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
        }

        /// <summary>
        /// 检查点击
        /// </summary>
        /// <returns></returns>
        public bool Clicked()
        {
            if (!ButtonVisible || !ButtonActive)
                return false;

            if (Hovered() && UIManager.LeftClicked)
            {
                Main.LocalPlayer.controlUseItem = false;
                return true;
            }
            return false;
        }
    }
}
