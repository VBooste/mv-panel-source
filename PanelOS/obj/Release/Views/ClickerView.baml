   M S B A M L   `   `   `  ����   PanelOS    PanelOS.Views.ClickerView   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     V RMaterialDesignThemes.Wpf, Version=3.1.0.1244, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  -     � Title$  ClickerView��$	��210��$	��450�����	#FF2D2E30 � Icon$+ %/PanelOS;component/Images/logo256.ico�� � 
ResizeMode$ NoResize=�$��14(�$��Medium"�   MaterialDesignBody#���    � WindowStartupLocation$ Manual=� � WindowStyle$
 None=���:  4MaterialDesignThemes.Wpf.MaterialDesignFontExtension  '  !System.Windows.Shell.WindowChrome   WindowChrome      CaptionHeight$ 0Q�   ResizeBorderThickness$ 0q�.���  � RowDefinitions �� $��20 ��� $��* �.+�� -   $��Header��$��0�����	#FF2E2F32.+��� $��0��$
��Left=�	 �� Orientation$	 
Horizontal=�.+��� $��20��$��
10,0,123,0q�$��0q�$��Center=�$��Center=�$��Center=�$��Center=�$��Clicker��$��12(����	#DDF1F1F1�� -   $��CloseWindowButton��$��30��$��20��$��0q�$��Center=�$��Center=�
 K� ZIndex$
 0��+  %MaterialDesignThemes.Wpf.ButtonAssist   CornerRadius$ 0�����	#FF525356$��0q�  MaterialDesignFlatButton#���  .��'  !MaterialDesignThemes.Wpf.PackIcon  $��16��$��16��$��Center=�$��Center=�$��Center=�$��Center=�
�������   Kind$ Close=�� $��1�� �� $��* ��� $��20 ��� $��30 ��� $��20 ��� $��20 ��� $��20 ��� $��25 ��� $��30 �.+��� -   $��LogsTextBox��$��0��+  %MaterialDesignThemes.Wpf.RippleAssist   
IsDisabled . .  (MaterialDesignThemes.Wpf.TextFieldAssist   DecorationVisibility$ Hidden=����	#FF292929$��12(� �� 
IsReadOnly .  � 	Resources  �� 
TargetType �� Property �� Value��%3   'J�       �� " J�.���  �@��Margin$ 0q��� $��1��$��20��$��0q�$��Stretch=�$��Bottom=�$��Center=�$��Center=�
�������$��0,0,0,1q�$��Auto accept��$��12(����	#DDB4B4B4�� $��2��$	 
Horizontal=�.+��� -   $��StartAutoAcceptButton��$	��105��$��0q����	#FF2E2F32$��Start�����	#DDF1F1F1#���  �� -   $��StopAutoAcceptButton��$	��105��$��0q����	#FF2E2F32$
��Stop�����	#DDF1F1F1{�.  #���  �� $��3��$��20��$��0q�$��Stretch=�$��Center=�$��Center=�$��Center=�
�������$��0,1,0,1q�$��Auto disconnect/reconnect��$��12(����	#DDB4B4B4�� $��4��$��0,5,0,0q�$	 
Horizontal=�.+�	� -   $��DisconnectTop��$��3,0,0,0q�$��0q�$��Center=�$��Center=�$��Center=�$��Center=�$�� Disconnect top��$��10(� d� 	IsChecked$
 TrueY�  MaterialDesignRadioButton#��[ 	� -   $��DisconnectBottom��$��5,0,0,0q�$��0q�$��Center=�$��Center=�$��Center=�$��Center=�$�� Disconnect bottom��$��10(�$ FalseY�#��[ � $��5�� � ColumnDefinitions �� $��1* ��� $��2* ��� $��2* �.+��� $��0��$��20��$��0q�$��Center=�$��Center=�$��Center=�$��Bottom=�$��Repeat��$��10(����	#DDB4B4B4�� $��1��$��20��$��0q�$��Center=�$��Center=�$��Center=�$��Bottom=�$��Loading delay��$��10(����	#DDB4B4B4�� $��2��$��20��$��0q�$��Center=�$��Center=�$��Center=�$��Bottom=�$��Reconnect delay��$��10(����	#DDB4B4B4� $��6�� �� $��1* ��� $��2* ��� $��2* �.+��� -	   $��RepeatTimesTextBox��$��0��$��Stretch=�$��Stretch=�$��Center=�$��Center=�)  #MaterialDesignThemes.Wpf.HintAssist   
Foreground
 �����   UnderlineBrush
 �����$��11(����	#FFA1A1A1  MaterialDesignTextBox#��[ �� -
   $��LoadingDelayTextBox��$��1��$��Stretch=�$��Stretch=�$��Center=�$��Center=�
 �����
 �����$��11(����	#FFA1A1A1#��[ �� -   $��ReconnectDelayTextBox��$��2��$��Stretch=�$��Stretch=�$��Center=�$��Center=�
 �����
 �����$��11(����	#FFA1A1A1#��[ �� $��7��$	 
Horizontal=�.+��� -   $��StartAutoConnectionButton��$	��105��$��0q����	#FF2E2F32$��Start�����	#DDF1F1F1#���  �� -   $��StopAutoConnectionButton��$	��105��$��0q����	#FF2E2F32$
��Stop�����	#DDF1F1F1{�.  #���  