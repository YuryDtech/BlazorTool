using Microsoft.AspNetCore.Components;
using System;

namespace BlazorTool.Client.Helpers
{
    public static class IconHtmlHelper
    {
        public static MarkupString GetIcon(string iconName, int size = 20)
        {
            return new MarkupString($"<img style='width: {size}px; height: {size}px;' src='icons/{iconName}' />");
        }

        public static MarkupString Save(int size = 20) => GetIcon("save_color.png", size);
        public static MarkupString Cancel(int size = 20) => GetIcon("cancel.png", size);
        public static MarkupString CloseOrder(int size = 20) => GetIcon("success.png", size);
        public static MarkupString Warn(int size = 20) => GetIcon("warn.png", size);
        public static MarkupString Delete(int size = 20) => GetIcon("cross_red.png", size);
        public static MarkupString Edit(int size = 20) => GetIcon("edit_color.png", size);
        public static MarkupString Info(int size = 20) => GetIcon("info.png", size);
        public static MarkupString Refresh(int size = 20) => GetIcon("refresh.png", size);
        public static MarkupString TakeOrder(int size = 20) => GetIcon("hand.png", size);
        public static MarkupString TakeOrderBW(int size = 20) => GetIcon("hand_bw.png", size);
        public static MarkupString AddAction(int size = 20) => GetIcon("action.png", size);
        public static MarkupString AssignToMyself(int size = 20) => GetIcon("assign.png", size);
    }
}
