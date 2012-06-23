using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Tarantula.MVP.View.Impl
{
    public partial class LoadingControl : UserControl, ILoadingView
    {
        private readonly Storyboard _pulse;
        private readonly Storyboard _fadeOut;

        public LoadingControl()
        {
            InitializeComponent();
            _pulse = (Storyboard)FindName("pulse");
            _fadeOut = (Storyboard)FindName("fadeOut");
        }

        public void ShowLoading()
        {
            SetValue(VisibilityProperty, Visibility.Visible);
            _pulse.Begin();
        }

        public void HideLoading()
        {
            _pulse.Stop();
            _fadeOut.Begin();
        }

        private void FadeOut_Completed(object sender, EventArgs e)
        {
            SetValue(VisibilityProperty, Visibility.Collapsed);
        }
    }
}
