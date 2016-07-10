using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SharpMvvmExtension
{
    public static class MetroWindowBehavior
    {

        #region AddFullScreenSupportProperty

        public static bool GetAddFullScreenSupport(MetroWindow metroWindow)
        {
            return (bool)metroWindow.GetValue(AddFullScreenSupportProperty);
        }

        public static void SetAddFullScreenSupport(MetroWindow metroWindow, bool value)
        {
            metroWindow.SetValue(AddFullScreenSupportProperty, value);
            ApplyFullScreenSupport(metroWindow, value);
        }

        public static readonly DependencyProperty AddFullScreenSupportProperty = DependencyProperty.RegisterAttached
        (
            "AddFullScreenSupport",
            typeof(bool),
            typeof(MetroWindow),
            new PropertyMetadata(true)
        );

        #endregion

        #region AddTopMostSupportProperty

        public static bool GetAddTopMostSupport(MetroWindow metroWindow)
        {
            return (bool)metroWindow.GetValue(AddTopMostSupportProperty);
        }

        public static void SetAddTopMostSupport(MetroWindow metroWindow, bool value)
        {
            metroWindow.SetValue(AddTopMostSupportProperty, value);
            ApplyTopMostSupport(metroWindow, value);
        }

        public static readonly DependencyProperty AddTopMostSupportProperty = DependencyProperty.RegisterAttached
        (
            "AddTopMostSupport",
            typeof(bool),
            typeof(MetroWindow),
            new PropertyMetadata(true)
        );

        #endregion

        #region FullScreenActiveProperty

        public static bool GetFullScreenActive(MetroWindow metroWindow)
        {
            return (bool)metroWindow.GetValue(FullScreenActiveProperty);
        }

        public static void SetFullScreenActive(MetroWindow metroWindow, bool value)
        {
            metroWindow.SetValue(FullScreenActiveProperty, value);
        }

        public static readonly DependencyProperty FullScreenActiveProperty = DependencyProperty.RegisterAttached
        (
            "FullScreenActive",
            typeof(bool),
            typeof(MetroWindow)
        );


        #endregion

        #region FullScreenToggle

        private static void ApplyFullScreenSupport(MetroWindow metroWindow, bool? value)
        {
            if (metroWindow != null && value.HasValue)
            {
                if (value.Value)
                {
                    metroWindow.Initialized += MetroWindow_FullScreenSupport_Initialized;
                    metroWindow.KeyDown += MetroWindow_FullScreenSupport_KeyDown;
                }
            }
        }

        private static void MetroWindow_FullScreenSupport_Initialized(object sender, EventArgs eventArgs)
        {
            var metroWindow = sender as MetroWindow;
            if (metroWindow != null)
            {
                var fullScreenGrid = new Grid();
                var fullScreenButton = new Button();

                fullScreenButton.ToolTip = "Vollbildmodus (F11)";
                fullScreenButton.Content = "Vollbild";

                fullScreenButton.Click += FullScreenButton_Click;

                fullScreenGrid.Children.Add(fullScreenButton);

                if (metroWindow.RightWindowCommands == null)
                    metroWindow.RightWindowCommands = new WindowCommands();

                metroWindow.RightWindowCommands.Items.Add(fullScreenGrid);
            }
        }

        private static void FullScreenButton_Click(object sender, RoutedEventArgs eventArgs)
        {
            var button = sender as Button;
            if (button != null)
            {
                var metroWindow = MetroWindow.GetWindow(button) as MetroWindow;
                OnFullScreen(metroWindow);
            }
        }

        private static void OnFullScreen(MetroWindow metroWindow)
        {
            if (metroWindow != null)
            {
                metroWindow.WindowState = WindowState.Maximized;
                metroWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                metroWindow.UseNoneWindowStyle = true;
                metroWindow.IgnoreTaskbarOnMaximize = true;
                OnTopMostChanged(metroWindow, true);
                metroWindow.ShowTitleBar = false;

                SetFullScreenActive(metroWindow, true);
            }
        }

        private static void OnNormalScreen(MetroWindow metroWindow)
        {
            if (metroWindow != null)
            {
                metroWindow.WindowState = WindowState.Normal;
                metroWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                metroWindow.UseNoneWindowStyle = false;
                metroWindow.IgnoreTaskbarOnMaximize = false;
                OnTopMostChanged(metroWindow, false);
                metroWindow.ShowTitleBar = true;

                SetFullScreenActive(metroWindow, false);
            }
        }

        private static void MetroWindow_FullScreenSupport_KeyDown(object sender, KeyEventArgs eventArgs)
        {
            OnFullScreenSupportKeyDown(sender as MetroWindow, eventArgs);
        }

        private static void OnFullScreenSupportKeyDown(MetroWindow metroWindow, KeyEventArgs eventArgs)
        {
            if (metroWindow != null)
            {
                bool fullScreenActive = GetFullScreenActive(metroWindow);
                switch (eventArgs.Key)
                {
                    case Key.F11:
                        {
                            if (fullScreenActive)
                            {
                                OnNormalScreen(metroWindow);
                            }
                            else
                            {
                                OnFullScreen(metroWindow);
                            }
                            eventArgs.Handled = true;
                            break;
                        }
                    case Key.Escape:
                        {
                            if (fullScreenActive)
                            {
                                OnNormalScreen(metroWindow);
                                eventArgs.Handled = true;
                            }
                            break;
                        }
                }
            }
        }

        #endregion

        #region TopMostToggle

        private static CheckBox topMostCheckBox;

        private static void ApplyTopMostSupport(MetroWindow metroWindow, bool? value)
        {
            if (metroWindow != null && value.HasValue)
            {
                if (value.Value)
                {
                    metroWindow.Initialized += MetroWindow_TopMostSupport_Initialized;
                    metroWindow.KeyDown += MetroWindow_TopMostSupport_KeyDown;
                    metroWindow.IsKeyboardFocusWithinChanged += MetroWindow_TopMostSupport_IsKeyboardFocusWithinChanged;
                }
            }
        }

        private static void MetroWindow_TopMostSupport_Initialized(object sender, EventArgs e)
        {
            var metroWindow = sender as MetroWindow;
            if (metroWindow != null)
            {
                var topMostGrid = new Grid();
                topMostCheckBox = new CheckBox();

                topMostCheckBox.ToolTip = "Immer im Vordergrund halten (F12)";
                topMostCheckBox.Content = "Vordergrund";

                topMostCheckBox.Checked += TopMostCheckBox_Checked;
                topMostCheckBox.Unchecked += TopMostCheckBox_Unchecked;

                topMostGrid.Children.Add(topMostCheckBox);

                if (metroWindow.RightWindowCommands == null)
                    metroWindow.RightWindowCommands = new WindowCommands();

                metroWindow.RightWindowCommands.Items.Add(topMostGrid);
            }
        }

        private static void TopMostCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            OnTopMostChanged(sender as DependencyObject, false);
        }

        private static void TopMostCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            OnTopMostChanged(sender as DependencyObject, true);
        }

        private static void OnTopMostChanged(DependencyObject dependencyObject, bool topMost)
        {
            if (dependencyObject != null)
            {
                Window window = dependencyObject as Window;
                if (window == null)
                    window = Window.GetWindow(dependencyObject);

                if (window != null)
                {
                    if (topMostCheckBox != null)
                        topMostCheckBox.IsChecked = topMost;

                    Window mainWindow = window;
                    while (mainWindow.Owner != null)
                        mainWindow = mainWindow.Owner;

                    mainWindow.Topmost = topMost;
                }
            }
        }

        private static void MetroWindow_TopMostSupport_KeyDown(object sender, KeyEventArgs eventArgs)
        {
            OnTopMostSupportKeyDown(sender as MetroWindow, eventArgs);
        }

        private static void OnTopMostSupportKeyDown(MetroWindow metroWindow, KeyEventArgs eventArgs)
        {
            if (metroWindow != null)
            {
                bool fullScreenActive = GetFullScreenActive(metroWindow);
                switch (eventArgs.Key)
                {
                    case Key.F12:
                        {
                            OnTopMostChanged(metroWindow, !metroWindow.Topmost);
                            eventArgs.Handled = true;
                            break;
                        }
                    case Key.LWin:
                        {
                            if (fullScreenActive)
                            {
                                OnTopMostChanged(metroWindow, false);
                                eventArgs.Handled = true;
                            }
                            break;
                        }
                }
            }
        }

        private static void MetroWindow_TopMostSupport_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            var checkBox = sender as CheckBox;
            if (checkBox != null)
            {
                var metroWindow = MetroWindow.GetWindow(checkBox) as MetroWindow;
                if (metroWindow != null)
                {
                    var isKeyboardFocusWithin = eventArgs.NewValue as bool?;
                    if (isKeyboardFocusWithin.HasValue)
                    {
                        bool fullScreenActive = GetFullScreenActive(metroWindow);
                        if (fullScreenActive && isKeyboardFocusWithin.Value)
                        {
                            OnTopMostChanged(metroWindow, true);
                        }
                    }
                }
            }
        }

        #endregion

    }
}
