# Technical Design: Ctl Views UI Standardization

## Architecture & Design Pattern
This change is purely visual XAML refactoring for WPF `UserControl` views. All existing `x:Name`, `Binding` paths, and event handler signatures (`Click="Btn..."`) in code-behind files will remain completely unchanged.

## UI Component Layout Template

```xml
<Grid Margin="10">
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/> <!-- Row 0: Header -->
        <RowDefinition Height="Auto"/> <!-- Row 1: Search & New Bar -->
        <RowDefinition Height="*"/>    <!-- Row 2: DataGrid Card -->
        <RowDefinition Height="Auto"/> <!-- Row 3: Footer Actions -->
    </Grid.RowDefinitions>

    <!-- Header -->
    <Grid Grid.Row="0" Margin="0,0,0,16">
        <StackPanel Orientation="Horizontal" VerticalAlignment="Center">
            <TextBlock Text="[Icon] [Title]" FontSize="22" FontWeight="Bold" Foreground="#2C3E50"/>
        </StackPanel>
    </Grid>

    <!-- Search & New Card -->
    <Border Grid.Row="1" Background="White" CornerRadius="12" Padding="16" Margin="0,0,0,16">
        <Border.Effect>
            <DropShadowEffect BlurRadius="10" ShadowDepth="2" Direction="270" Color="#000000" Opacity="0.08"/>
        </Border.Effect>
        <Grid>
            <!-- Search TextBox on Left, "➕ Nuevo" Button (#27AE60) on Right -->
        </Grid>
    </Border>

    <!-- DataGrid Card -->
    <Border Grid.Row="2" Background="White" CornerRadius="12" Padding="20">
        <Border.Effect>
            <DropShadowEffect BlurRadius="12" ShadowDepth="2" Direction="270" Color="#000000" Opacity="0.08"/>
        </Border.Effect>
        <DataGrid ... />
    </Border>

    <!-- Footer Actions Card -->
    <Border Grid.Row="3" Background="White" CornerRadius="12" Padding="12,16" Margin="0,16,0,0">
        <Border.Effect>
            <DropShadowEffect BlurRadius="10" ShadowDepth="2" Direction="270" Color="#000000" Opacity="0.08"/>
        </Border.Effect>
        <StackPanel Orientation="Horizontal" HorizontalAlignment="Right">
            <!-- "✏️ Editar" Button (#F39C12) -->
            <!-- "🗑️ Eliminar" Button (#C0392B) -->
        </StackPanel>
    </Border>
</Grid>
```
