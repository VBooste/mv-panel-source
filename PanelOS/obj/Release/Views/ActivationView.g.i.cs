﻿#pragma checksum "..\..\..\Views\ActivationView.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "E85F7FCD2A23840BBEFCB09C6D079DD20881FFC70CA45C78F199882B4EBC96FB"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Converters;
using MaterialDesignThemes.Wpf.Transitions;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace PanelOS.Views {
    
    
    /// <summary>
    /// ActivationView
    /// </summary>
    public partial class ActivationView : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 41 "..\..\..\Views\ActivationView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button MinimizeWindowButton;
        
        #line default
        #line hidden
        
        
        #line 65 "..\..\..\Views\ActivationView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CloseWindowButton;
        
        #line default
        #line hidden
        
        
        #line 90 "..\..\..\Views\ActivationView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal MaterialDesignThemes.Wpf.Snackbar activationWindowPopup;
        
        #line default
        #line hidden
        
        
        #line 114 "..\..\..\Views\ActivationView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label HWIDLabel;
        
        #line default
        #line hidden
        
        
        #line 172 "..\..\..\Views\ActivationView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ActivateButton;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/PanelOS;component/views/activationview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Views\ActivationView.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.MinimizeWindowButton = ((System.Windows.Controls.Button)(target));
            
            #line 50 "..\..\..\Views\ActivationView.xaml"
            this.MinimizeWindowButton.Click += new System.Windows.RoutedEventHandler(this.MinimizeWindowButton_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.CloseWindowButton = ((System.Windows.Controls.Button)(target));
            
            #line 74 "..\..\..\Views\ActivationView.xaml"
            this.CloseWindowButton.Click += new System.Windows.RoutedEventHandler(this.CloseWindowButton_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.activationWindowPopup = ((MaterialDesignThemes.Wpf.Snackbar)(target));
            return;
            case 4:
            this.HWIDLabel = ((System.Windows.Controls.Label)(target));
            
            #line 125 "..\..\..\Views\ActivationView.xaml"
            this.HWIDLabel.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.HWIDLabel_MouseLeftButtonUp);
            
            #line default
            #line hidden
            return;
            case 5:
            this.ActivateButton = ((System.Windows.Controls.Button)(target));
            
            #line 179 "..\..\..\Views\ActivationView.xaml"
            this.ActivateButton.Click += new System.Windows.RoutedEventHandler(this.ActivateButton_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

