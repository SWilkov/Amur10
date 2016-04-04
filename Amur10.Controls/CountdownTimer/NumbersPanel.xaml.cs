using Amur10.Controls.CountdownTimer.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Amur10.Controls.CountdownTimer
{
    public sealed partial class NumbersPanel : UserControl
    {
        #region events
        public event EventHandler<SelectNumberChangedEventArgs> SelectedNumberChanged;

        /// <summary>
        /// Raised event when selected number has changed. Informs subscribers of the old number and the new number.
        /// </summary>
        /// <param name="newNumber"></param>
        /// <param name="oldNumber"></param>
        public void OnSelectedNumberChanged(int newNumber, int oldNumber)
        {
            EventHandler<SelectNumberChangedEventArgs> eh = SelectedNumberChanged;
            if (eh != null)
            {
                SelectedNumberChanged(this, new SelectNumberChangedEventArgs(newNumber, oldNumber));
            }
        }
        #endregion

        #region Dependency properties

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsExpanded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(NumbersPanel), new PropertyMetadata(false));



        public double ItemFontSize
        {
            get { return (double)GetValue(ItemFontSizeProperty); }
            set { SetValue(ItemFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemFontSizeProperty =
            DependencyProperty.Register("ItemFontSize", typeof(double), typeof(NumbersPanel), new PropertyMetadata(Defaults.ITEM_BUTTON_FONTSIZE));



        public List<int> Items
        {
            get { return (List<int>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Items.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(List<int>), typeof(NumbersPanel), new PropertyMetadata(null));



        public List<int> ExtraItems
        {
            get { return (List<int>)GetValue(ExtraItemsProperty); }
            set { SetValue(ExtraItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExtraItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExtraItemsProperty =
            DependencyProperty.Register("ExtraItems", typeof(List<int>), typeof(NumbersPanel), new PropertyMetadata(null));


        public int Selected
        {
            get { return (int)GetValue(SelectedProperty); }
            set { SetValue(SelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Selected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedProperty =
            DependencyProperty.Register("Selected", typeof(int), typeof(NumbersPanel), new PropertyMetadata(0, SelectedChanged));



        public SolidColorBrush ItemButtonBackground
        {
            get { return (SolidColorBrush)GetValue(ItemButtonBackgroundProperty); }
            set { SetValue(ItemButtonBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemButtonBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemButtonBackgroundProperty =
            DependencyProperty.Register("ItemButtonBackground", typeof(SolidColorBrush), typeof(NumbersPanel), new PropertyMetadata(new SolidColorBrush(Defaults.ITEM_BUTTON_BACKGROUND)));



        public double ItemButtonWidth
        {
            get { return (double)GetValue(ItemButtonWidthProperty); }
            set { SetValue(ItemButtonWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NumbersPanelButtonWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemButtonWidthProperty =
            DependencyProperty.Register("ItemButtonWidth", typeof(double), typeof(NumbersPanel), new PropertyMetadata(Defaults.ITEM_BUTTON_WIDTH));


        public double ItemButtonHeight
        {
            get { return (double)GetValue(ItemButtonHeightProperty); }
            set { SetValue(ItemButtonHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NumbersPanelButtonHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemButtonHeightProperty =
            DependencyProperty.Register("ItemButtonHeight", typeof(double), typeof(NumbersPanel), new PropertyMetadata(Defaults.ITEM_BUTTON_HEIGHT));

        public int MinimumValue
        {
            get { return (int)GetValue(MinimumValueProperty); }
            set { SetValue(MinimumValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumValueProperty =
            DependencyProperty.Register("MinimumValue", typeof(int), typeof(NumbersPanel), new PropertyMetadata(0));



        public int MaximumValue
        {
            get { return (int)GetValue(MaximumValueProperty); }
            set { SetValue(MaximumValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaximumValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumValueProperty =
            DependencyProperty.Register("MaximumValue", typeof(int), typeof(NumbersPanel), new PropertyMetadata(12));


        #endregion

        #region Dependency events
        private static void SelectedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var np = sender as NumbersPanel;
            int old = 0;
            if (int.TryParse(e.OldValue.ToString(), out old))
            {
                Debug.WriteLine("Converted int success");
            }
            else
                Debug.WriteLine("Error converting int");

            np.OnSelectedNumberChanged(np.Selected, old);
        }
        #endregion

        public NumbersPanel()
        {
            this.InitializeComponent();
        }
    }
}
