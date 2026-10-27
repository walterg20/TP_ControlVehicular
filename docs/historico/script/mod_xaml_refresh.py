import os

path = r'e:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular\Presentacion\Pantalla\OrdenServicio\CtlOrdenServicio.xaml'
with open(path, 'r', encoding='utf-8-sig') as f:
    content = f.read()

target = '''                    <!-- Aquí el botón buscar simplemente actualiza (ya se actualiza solo por el Binding en realidad) pero por consistencia -->
                    <Button Content="Buscar" Background="#2980B9" Foreground="White" Padding="15,6" Margin="10,0,0,0" FontWeight="SemiBold" Cursor="Hand">
                        <Button.Resources>
                            <Style TargetType="Border">
                                <Setter Property="CornerRadius" Value="6"/>
                            </Style>
                        </Button.Resources>
                    </Button>'''

repl = '''                    <Button Content="🔄 Refrescar" Background="#2980B9" Foreground="White" Padding="15,6" Margin="10,0,0,0" FontWeight="SemiBold" Cursor="Hand" Click="BtnRefrescar_Click" ToolTip="Recargar datos de la base de datos">
                        <Button.Resources>
                            <Style TargetType="Border">
                                <Setter Property="CornerRadius" Value="6"/>
                            </Style>
                        </Button.Resources>
                    </Button>'''

content = content.replace(target, repl)
with open(path, 'w', encoding='utf-8-sig') as f:
    f.write(content)
