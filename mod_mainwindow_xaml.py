import os

path = r'e:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular\Presentacion\MainWindow.xaml'
with open(path, 'r', encoding='utf-8-sig') as f:
    content = f.read()

target = '''    </Grid>
</Window>'''

repl = '''        <!-- TOAST NOTIFICATIONS (Snackbar) -->
        <Border x:Name="bdrToast" Grid.Column="1" HorizontalAlignment="Right" VerticalAlignment="Bottom" Margin="0,0,30,30" Padding="20,12" Background="#27AE60" CornerRadius="8" Visibility="Collapsed" Panel.ZIndex="1000">
            <Border.Effect>
                <DropShadowEffect BlurRadius="10" ShadowDepth="2" Direction="270" Color="#000000" Opacity="0.2"/>
            </Border.Effect>
            <StackPanel Orientation="Horizontal">
                <TextBlock Text="✔️" Foreground="White" VerticalAlignment="Center" Margin="0,0,10,0"/>
                <TextBlock x:Name="lblToastMessage" Text="Operación exitosa" Foreground="White" FontWeight="SemiBold" VerticalAlignment="Center"/>
            </StackPanel>
        </Border>
    </Grid>
</Window>'''

content = content.replace(target, repl)
with open(path, 'w', encoding='utf-8-sig') as f:
    f.write(content)
