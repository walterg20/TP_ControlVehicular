import os
import re

xaml_path = r'e:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular\Presentacion\Pantalla\OrdenServicio\CtlOrdenServicioForm.xaml'
with open(xaml_path, 'r', encoding='utf-8-sig') as f:
    xaml_content = f.read()

xaml_content = xaml_content.replace(
    'SelectedValuePath="IdUsuario" DisplayMemberPath="Nombre" />', 
    'SelectedValuePath="IdUsuario" DisplayMemberPath="NombreCompletoYDni" IsEnabled="{Binding DataContext.EsRecepcionista, RelativeSource={RelativeSource AncestorType=UserControl}}" />'
)
xaml_content = xaml_content.replace(
    '<CheckBox IsChecked="{Binding Realizado, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" HorizontalAlignment="Center"/>', 
    '<CheckBox IsChecked="{Binding Realizado, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" HorizontalAlignment="Center" Checked="CheckBoxRevisado_Changed" Unchecked="CheckBoxRevisado_Changed"/>'
)

with open(xaml_path, 'w', encoding='utf-8-sig') as f:
    f.write(xaml_content)

cs_path = r'e:\UNNE\Taller de Programación II\Proyecto\TP_ControlVehicular\Presentacion\Pantalla\OrdenServicio\CtlOrdenServicioForm.xaml.cs'
with open(cs_path, 'r', encoding='utf-8-sig') as f:
    cs_content = f.read()

cs_content = cs_content.replace(
    '"El servicio ya se encuentra en el checklist."', 
    '"El servicio ya se encuentra asignado a un mecánico en esta orden."'
)

handler = '''        private void CheckBoxRevisado_Changed(object sender, RoutedEventArgs e)
        {
            if (DataContext is OrdenServicioViewModel vmOrdenServicioForm)
            {
                vmOrdenServicioForm.EvaluarEstadoGeneral();
            }
        }
'''
cs_content = cs_content.replace('        private void dgChecklist_BeginningEdit', handler + '\n        private void dgChecklist_BeginningEdit')

with open(cs_path, 'w', encoding='utf-8-sig') as f:
    f.write(cs_content)
