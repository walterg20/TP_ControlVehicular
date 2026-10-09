import os
import re

path = r'e:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular\Presentacion\Pantalla\Reporte\CtlReporteOrdenes.xaml'
with open(path, 'r', encoding='utf-8') as f:
    content = f.read()

# Remove the buttons from the header
content = re.sub(
    r'<StackPanel Orientation="Horizontal" HorizontalAlignment="Right" Margin="0,0,45,0">.*?</StackPanel>', 
    '', 
    content, 
    flags=re.DOTALL
)

replacement = '''            <Grid>
                <Grid.RowDefinitions>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="*"/>
                </Grid.RowDefinitions>

                <Button Grid.Row="0" Content="📄 Imprimir PDF" 
                        Command="{Binding ImprimirPdfCommand}" 
                        HorizontalAlignment="Left"
                        Margin="0,0,0,15"
                        Padding="14,8" 
                        Background="#E74C3C" 
                        Foreground="White" 
                        FontWeight="Bold" 
                        BorderThickness="0" 
                        Cursor="Hand">
                    <Button.Resources>
                        <Style TargetType="Border">
                            <Setter Property="CornerRadius" Value="8"/>
                        </Style>
                    </Button.Resources>
                </Button>

                <DataGrid Grid.Row="1" x:Name="dgReporte"'''

content = content.replace('<DataGrid x:Name="dgReporte"', replacement)
content = content.replace('</DataGrid>\n        </Border>', '</DataGrid>\n            </Grid>\n        </Border>')

with open(path, 'w', encoding='utf-8') as f:
    f.write(content)
