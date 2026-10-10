import io

xaml_content = '''<Window x:Class="TP_ControlVehicular.Presentacion.Pantalla.Rol.FrmRolPermiso"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Administrar Permisos" Height="450" Width="400"
        ResizeMode="NoResize"
        WindowStartupLocation="CenterOwner">

    <Grid Margin="24">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <!-- Cabecera -->
        <StackPanel Grid.Row="0" Margin="0,0,0,16">
            <TextBlock x:Name="lblTitulo" Text="Permisos del Rol" FontSize="20" FontWeight="Bold" Foreground="#2C3E50"/>
            <TextBlock x:Name="lblSubtitulo" Text="Seleccione los permisos permitidos" FontSize="12" Foreground="#7F8C8D" Margin="0,2,0,0"/>
        </StackPanel>

        <!-- Listado de Permisos -->
        <Border Grid.Row="1" Background="White" CornerRadius="8" Padding="10" BorderBrush="#BDC3C7" BorderThickness="1">
            <ScrollViewer VerticalScrollBarVisibility="Auto">
                <ItemsControl x:Name="icPermisos">
                    <ItemsControl.ItemTemplate>
                        <DataTemplate>
                            <CheckBox Content="{Binding Nombre}" 
                                      IsChecked="{Binding Asignado, Mode=TwoWay}" 
                                      Margin="0,5,0,5"
                                      FontSize="14"/>
                        </DataTemplate>
                    </ItemsControl.ItemTemplate>
                </ItemsControl>
            </ScrollViewer>
        </Border>

        <!-- Botones -->
        <StackPanel Orientation="Horizontal" HorizontalAlignment="Right" Grid.Row="2" Margin="0,16,0,0">
            <Button Content="Guardar" Width="95" Height="34" IsDefault="True" Click="BtnGuardar_Click" 
                    Background="#27AE60" Foreground="White" FontWeight="Bold" BorderThickness="0" Margin="0,0,12,0" Cursor="Hand">
                <Button.Resources>
                    <Style TargetType="Border">
                        <Setter Property="CornerRadius" Value="6"/>
                    </Style>
                </Button.Resources>
            </Button>
            <Button Content="Cancelar" Width="95" Height="34" Click="BtnCancelar_Click" 
                    Background="#C0392B" Foreground="White" FontWeight="Bold" BorderThickness="0" Cursor="Hand">
                <Button.Resources>
                    <Style TargetType="Border">
                        <Setter Property="CornerRadius" Value="6"/>
                    </Style>
                </Button.Resources>
            </Button>
        </StackPanel>
    </Grid>
</Window>
'''

cs_content = '''using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TP_ControlVehicular.Negocio.DTOs;
using TP_ControlVehicular.Negocio.Handlers;

namespace TP_ControlVehicular.Presentacion.Pantalla.Rol
{
    public partial class FrmRolPermiso : Window
    {
        private readonly int _rolId;
        private readonly RolDto _rolDto;
        private List<PermisoDto> _permisosDisponibles = new();

        public FrmRolPermiso(RolDto rol)
        {
            InitializeComponent();
            _rolId = rol.IdRol;
            _rolDto = rol;
            lblTitulo.Text = $"Permisos de: {rol.Nombre}";
            Owner = Application.Current.MainWindow;
            Loaded += FrmRolPermiso_Loaded;
        }

        private async void FrmRolPermiso_Loaded(object sender, RoutedEventArgs e)
        {
            var handler = App.ServiceProvider.GetRequiredService<ObtenerPermisosHandler>();
            _permisosDisponibles = await handler.HandleAsync();

            // Marcar los que el rol ya tiene
            foreach (var p in _permisosDisponibles)
            {
                if (_rolDto.Permisos != null && _rolDto.Permisos.Contains(p.Nombre))
                {
                    p.Asignado = true;
                }
            }

            icPermisos.ItemsSource = _permisosDisponibles;
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            var seleccionados = _permisosDisponibles.Where(p => p.Asignado).Select(p => p.IdPermiso).ToList();
            var handler = App.ServiceProvider.GetRequiredService<ActualizarPermisosRolHandler>();
            
            bool result = await handler.HandleAsync(_rolId, seleccionados);
            if (result)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Ocurrió un error al guardar los permisos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
'''

import os
os.makedirs('Presentacion/Pantalla/Rol', exist_ok=True)

with io.open('Presentacion/Pantalla/Rol/FrmRolPermiso.xaml', 'w', encoding='utf-8') as f:
    f.write(xaml_content)

with io.open('Presentacion/Pantalla/Rol/FrmRolPermiso.xaml.cs', 'w', encoding='utf-8') as f:
    f.write(cs_content)
