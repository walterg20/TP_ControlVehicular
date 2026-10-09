import os

path = r'e:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular\Presentacion\Pantalla\OrdenServicio\CtlOrdenServicioForm.xaml'
with open(path, 'r', encoding='utf-8-sig') as f:
    content = f.read()

target = '''<Button x:Name="btnGuardar" Content="Guardar" Grid.Column="1" Background="#27AE60" Foreground="White" FontWeight="Bold" Padding="20,8" Margin="10,0,0,0" Cursor="Hand" Click="BtnGuardar_Click" IsDefault="True">
                    <Button.Resources>
                        <Style TargetType="Border">
                            <Setter Property="CornerRadius" Value="6"/>
                        </Style>
                    </Button.Resources>
                </Button>'''

repl = '''<Button x:Name="btnGuardar" Grid.Column="1" Background="#27AE60" Foreground="White" FontWeight="Bold" Padding="20,8" Margin="10,0,0,0" Cursor="Hand" Click="BtnGuardar_Click" IsDefault="True" IsEnabled="{Binding IsNotLoading}">
                    <Button.Style>
                        <Style TargetType="Button">
                            <Setter Property="Content" Value="Guardar"/>
                            <Setter Property="Template">
                                <Setter.Value>
                                    <ControlTemplate TargetType="Button">
                                        <Border Background="{TemplateBinding Background}" CornerRadius="6" BorderBrush="{TemplateBinding BorderBrush}" BorderThickness="{TemplateBinding BorderThickness}">
                                            <ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center"/>
                                        </Border>
                                    </ControlTemplate>
                                </Setter.Value>
                            </Setter>
                            <Style.Triggers>
                                <DataTrigger Binding="{Binding IsLoading}" Value="True">
                                    <Setter Property="Content" Value="Guardando..."/>
                                    <Setter Property="Opacity" Value="0.7"/>
                                </DataTrigger>
                            </Style.Triggers>
                        </Style>
                    </Button.Style>
                </Button>'''

content = content.replace(target, repl)
with open(path, 'w', encoding='utf-8-sig') as f:
    f.write(content)
