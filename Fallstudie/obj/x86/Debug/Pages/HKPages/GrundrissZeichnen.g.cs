﻿#pragma checksum "C:\Users\Ermin\Desktop\GitHub\Fallstudie-V1.1.1-1\Fallstudie\Pages\HKPages\GrundrissZeichnen.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "3B7BEAD57D00EA3BD24354F792B3A5A5"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Fallstudie.Pages.HKPages
{
    partial class GrundrissZeichnen : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        internal class XamlBindingSetters
        {
            public static void Set_Microsoft_Labs_InkToolbarControl_InkToolbar_TargetInkCanvas(global::Microsoft.Labs.InkToolbarControl.InkToolbar obj, global::Windows.UI.Xaml.Controls.InkCanvas value, string targetNullValue)
            {
                if (value == null && targetNullValue != null)
                {
                    value = (global::Windows.UI.Xaml.Controls.InkCanvas) global::Windows.UI.Xaml.Markup.XamlBindingHelper.ConvertValue(typeof(global::Windows.UI.Xaml.Controls.InkCanvas), targetNullValue);
                }
                obj.TargetInkCanvas = value;
            }
        };

        private class GrundrissZeichnen_obj1_Bindings :
            global::Windows.UI.Xaml.Markup.IComponentConnector,
            IGrundrissZeichnen_Bindings
        {
            private global::Fallstudie.Pages.HKPages.GrundrissZeichnen dataRoot;
            private bool initialized = false;
            private const int NOT_PHASED = (1 << 31);
            private const int DATA_CHANGED = (1 << 30);

            // Fields for each control that has bindings.
            private global::Microsoft.Labs.InkToolbarControl.InkToolbar obj2;

            public GrundrissZeichnen_obj1_Bindings()
            {
            }

            // IComponentConnector

            public void Connect(int connectionId, global::System.Object target)
            {
                switch(connectionId)
                {
                    case 2:
                        this.obj2 = (global::Microsoft.Labs.InkToolbarControl.InkToolbar)target;
                        break;
                    default:
                        break;
                }
            }

            // IGrundrissZeichnen_Bindings

            public void Initialize()
            {
                if (!this.initialized)
                {
                    this.Update();
                }
            }
            
            public void Update()
            {
                this.Update_(this.dataRoot, NOT_PHASED);
                this.initialized = true;
            }

            public void StopTracking()
            {
            }

            // GrundrissZeichnen_obj1_Bindings

            public void SetDataRoot(global::Fallstudie.Pages.HKPages.GrundrissZeichnen newDataRoot)
            {
                this.dataRoot = newDataRoot;
            }

            public void Loading(global::Windows.UI.Xaml.FrameworkElement src, object data)
            {
                this.Initialize();
            }

            // Update methods for each path node used in binding steps.
            private void Update_(global::Fallstudie.Pages.HKPages.GrundrissZeichnen obj, int phase)
            {
                if (obj != null)
                {
                    if ((phase & (NOT_PHASED | (1 << 0))) != 0)
                    {
                        this.Update_inkCanvas(obj.inkCanvas, phase);
                    }
                }
            }
            private void Update_inkCanvas(global::Windows.UI.Xaml.Controls.InkCanvas obj, int phase)
            {
                if((phase & ((1 << 0) | NOT_PHASED )) != 0)
                {
                    XamlBindingSetters.Set_Microsoft_Labs_InkToolbarControl_InkToolbar_TargetInkCanvas(this.obj2, obj, null);
                }
            }
        }
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 3:
                {
                    this.inkCanvas = (global::Windows.UI.Xaml.Controls.InkCanvas)(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            switch(connectionId)
            {
            case 1:
                {
                    global::Windows.UI.Xaml.Controls.Page element1 = (global::Windows.UI.Xaml.Controls.Page)target;
                    GrundrissZeichnen_obj1_Bindings bindings = new GrundrissZeichnen_obj1_Bindings();
                    returnValue = bindings;
                    bindings.SetDataRoot(this);
                    this.Bindings = bindings;
                    element1.Loading += bindings.Loading;
                }
                break;
            }
            return returnValue;
        }
    }
}

