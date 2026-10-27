import io

with io.open('Presentacion/Pantalla/MisTrabajos/CtlMisTrabajos.xaml', 'r', encoding='utf-8') as f:
    xaml_content = f.read()

# Binding TareaSeleccionada
xaml_content = xaml_content.replace(
    'SelectionMode="Single"',
    'SelectionMode="Single" SelectedItem="{Binding TareaSeleccionada, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"'
)

# Insert Tareas del Dia button in header
header_btn = '''
                <Button Grid.Column="1" Command="{Binding ImprimirHojaTrabajoCommand}" Padding="10,6" Background="#34495E" Foreground="White" FontWeight="Bold" BorderThickness="0" Margin="0,0,10,0" Cursor="Hand">
                    <StackPanel Orientation="Horizontal">
                        <TextBlock Text="?? Tareas del Día" Margin="5,0"/>
                    </StackPanel>
                    <Button.Resources>
                        <Style TargetType="Border"><Setter Property="CornerRadius" Value="6"/></Style>
                    </Button.Resources>
                </Button>
'''
xaml_content = xaml_content.replace(
    '<TextBlock Text="?? Mis Trabajos" FontSize="24" FontWeight="Bold" Foreground="#2C3E50" VerticalAlignment="Center"/>',
    '<TextBlock Grid.Column="0" Text="?? Mis Trabajos" FontSize="24" FontWeight="Bold" Foreground="#2C3E50" VerticalAlignment="Center"/>\n' + header_btn
)

# Fix Grid.Column definitions if header didn't have them
if '<Grid.ColumnDefinitions>' not in xaml_content.split('<!-- ENCABEZADO -->')[1].split('</Grid>')[0]:
    xaml_content = xaml_content.replace(
        '<Grid Margin="0,0,0,20">',
        '<Grid Margin="0,0,0,20">\n<Grid.ColumnDefinitions><ColumnDefinition Width="*"/><ColumnDefinition Width="Auto"/></Grid.ColumnDefinitions>'
    )

# Insert Historial Clinico button in Footer
footer_btn = '''
                <Button Content="🕒 Imprimir Historial Clínico" Command="{Binding ImprimirHistorialCommand}" Padding="12,8" Background="#8E44AD" Foreground="White" FontWeight="Bold" BorderThickness="0" Margin="0,0,8,0" Cursor="Hand" Visibility="{Binding PuedeImprimirHistorial, Converter={StaticResource BoolToVis}}">
                    <Button.Resources>
                        <Style TargetType="Border">
                            <Setter Property="CornerRadius" Value="6"/>
                        </Style>
                    </Button.Resources>
                </Button>
'''
xaml_content = xaml_content.replace(
    '<StackPanel Orientation="Horizontal" HorizontalAlignment="Right">',
    '<StackPanel Orientation="Horizontal" HorizontalAlignment="Right">\n' + footer_btn
)

with io.open('Presentacion/Pantalla/MisTrabajos/CtlMisTrabajos.xaml', 'w', encoding='utf-8') as f:
    f.write(xaml_content)