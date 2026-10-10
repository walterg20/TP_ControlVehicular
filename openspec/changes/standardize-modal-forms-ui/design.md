# Technical Design: Frm Modal Dialogs UI Standardization

## Button Styling Specifications

### Form Action Buttons Template
```xml
<StackPanel Orientation="Horizontal" HorizontalAlignment="Right" Grid.Row="2" Margin="0,20,0,0">
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
```

### Inline Related Entity Add Button Template
```xml
<Button x:Name="btnAgregar[Entity]" Grid.Column="1" Content="➕" Width="30" Height="30"
        ToolTip="Agregar nueva [Entity]" Background="#27AE60" Foreground="White" 
        FontWeight="Bold" BorderThickness="0" Cursor="Hand" Click="BtnAgregar[Entity]_Click">
    <Button.Resources>
        <Style TargetType="Border">
            <Setter Property="CornerRadius" Value="6"/>
        </Style>
    </Button.Resources>
</Button>
```
