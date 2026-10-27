import os

path = r'e:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular\Presentacion\Pantalla\OrdenServicio\CtlOrdenServicio.xaml'
with open(path, 'r', encoding='utf-8-sig') as f:
    content = f.read()

target = '''            <DataGrid x:Name="dgOrdenes"'''

repl = '''            <Grid>
                <StackPanel VerticalAlignment="Center" HorizontalAlignment="Center" Visibility="{Binding IsListEmpty, Converter={StaticResource BoolToVis}}" Panel.ZIndex="10" Margin="0,50,0,0">
                    <TextBlock Text="🔍" FontSize="48" HorizontalAlignment="Center" Foreground="#BDC3C7"/>
                    <TextBlock Text="No se encontraron órdenes de servicio" FontSize="16" FontWeight="SemiBold" Foreground="#7F8C8D" Margin="0,10,0,0" HorizontalAlignment="Center"/>
                    <TextBlock Text="Crea una nueva o intentá con otra búsqueda." FontSize="13" Foreground="#95A5A6" HorizontalAlignment="Center" Margin="0,5,0,0"/>
                </StackPanel>
            <DataGrid x:Name="dgOrdenes"'''

content = content.replace(target, repl)

target_end = '''            </DataGrid>
        </Border>'''

repl_end = '''            </DataGrid>
            </Grid>
        </Border>'''

content = content.replace(target_end, repl_end)

with open(path, 'w', encoding='utf-8-sig') as f:
    f.write(content)
