   M S B A M L   `   `   `  ����A  =Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:ClientClient  5   	       Client.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:Client     5   	     � Title$  
MainWindow��5   	   $	��450��6   $	��800��6)   .��5
      �  � RowDefinitions 5   
   �� 5      $
��auto �6   �� 5      $
��auto �6   �� 5      $
��auto �6   �� 5      $
��auto �6   .+�5   
   � $��0��6   .+�5      �� $��Center=�6   $	��100��65   
��� ���
����  �$	��Top=�6^   $	��800��6v   �� 5      $��Syracuse P2P Network��6   $��Center=�63   $��Center=�6W   $��Center=�6t   $	��444��6�   $
��Bold"�6�   $
��20px(�6�   5      � 5   
   $��1��6   $��Center=�6   $��0,20,0,0q�69    � ColumnDefinitions 5      �� 5      $
��auto �6#   �� 5      $
��auto �6#   .+�5!      �� $��Username��6   $��0��6'   $
��Left=�67   $
��20px(�6R   $��40��6b   $	��125��6n   �� 5"      -   5         ��Name$ txtUserName��5"      $��1��6+   $��Center=�6;   $
��Left=�6]    �� TextWrapping$
 Wrap=�6x   $��30��6�   $	��200��6�   5#      � 5%   
   $��2��6   $��Center=�6    5'      �� 5(      $
��auto �6#   �� 5)      $
��auto �6#   .+�5,      �� $��Password��6   $��0��6'   $
��Left=�67   $
��20px(�6R   $��40��6b   $	��125��6n   G� 5-      -   5         G�Name$ txtPassword��5-      $��1��6/   $��Center=�6?   $
��Left=�6a   $��30��6|   $	��200��6�   5.      � 50   
   $��3��6   $��Center=�6    52      �� 53      $
��auto �6#   �� 54      $
��auto �6#   .+�57      �� -   5        $��Log In��57      $��100px��6&   $��16(�64   $��0,15,0,0q�6B   $	��Top=�6T   6l   58      5:      5;      