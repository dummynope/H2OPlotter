using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using H2MassPercentPlotter.Data;
#pragma warning disable IDE0047

namespace H2MassPercentPlotter;

public class H2Calc : INotifyPropertyChanged
{
    public double R_H2 { get; set; } = 4.124; //%[J/g*K]

    public double mass_MgFe { get; set; } = 1.0; //%[g]

    public double vol_AC { get; set; } = 0.094; //%[l] //TODO muss berechnet werden

    public double vol_Res { get; set; } = 0.497; //%[l]

    public ObservableCollection<UIRecord> Records { get; set; } = new();

    private const string Filename = "data.json";


    public void Load()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            FileName = Filename
        };
        var selected = openFileDialog.ShowDialog();
        if (selected == null || selected == false)
            return;

        var file = openFileDialog.FileName;
        var jsonString = File.ReadAllText(file);
        H2CalcData? res = JsonSerializer.Deserialize<H2CalcData>(jsonString);

        if (res == null)
            return;

        mass_MgFe = res.mass_MgFe;
        vol_AC = res.vol_AC;
        vol_Res = res.vol_Res;
        R_H2 = res.R_H2;
        OnPropertyChanged(nameof(mass_MgFe));
        OnPropertyChanged(nameof(vol_AC));
        OnPropertyChanged(nameof(vol_Res));
        OnPropertyChanged(nameof(R_H2));


        Records.Clear();
        foreach (var o in res.Records)
        {
            Records.Add(new UIRecord(o.Pre_Dehydrogenation_PA, o.T_AC_dehy_Kelvin, o.Post_Dehydrogenation_PA, o.T_Res_Kelvin, o.Name));
        }
    }


    public void Save()
    {
        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Title = "Save",
            FileName = Filename
        };
        saveFileDialog.ShowDialog();

        var file = saveFileDialog.FileName;
        if (string.IsNullOrEmpty(file))
            return;

        H2CalcData h2CalcData = new H2CalcData()
        {
            mass_MgFe = mass_MgFe,
            vol_AC = vol_AC,
            vol_Res = vol_Res,
            R_H2 = R_H2,
            Records = Records.Select(o => new Record()
            {
                Post_Dehydrogenation_PA = o.Post_Dehydrogenation_PA,
                Pre_Dehydrogenation_PA = o.Pre_Dehydrogenation_PA,
                T_AC_dehy_Kelvin = o.T_AC_dehy_Kelvin,
                T_Res_Kelvin = o.T_Res_Kelvin
            }).ToArray()
        };
        string jsonString = JsonSerializer.Serialize(h2CalcData);
        File.WriteAllText(file, jsonString);
    }

    public IEnumerable<(double desorped_H2, double capacity_H2_des, string log)> Calc()
    {
        for (var index = 0; index < Records.Count; index++)
        {
            var record = Records[index];
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{index}:");
            sb.AppendLine(
                $"{nameof(record.Pre_Dehydrogenation_PA)}: {record.Pre_Dehydrogenation_PA:0.0000}, {nameof(record.Post_Dehydrogenation_PA)}: {record.Post_Dehydrogenation_PA:0.0000} [Pa]");
            sb.AppendLine($"{nameof(record.T_AC_dehy_Kelvin)}: {record.T_AC_dehy_Kelvin:0.0000}, {nameof(record.T_Res_Kelvin)}: {record.T_Res_Kelvin:0.0000} [Pa]");
            var deltPres_dehy = record.Post_Dehydrogenation_PA - record.Pre_Dehydrogenation_PA;
            sb.AppendLine($"{nameof(deltPres_dehy)}: {deltPres_dehy:0.0000}");
            var desorped_H2 = (deltPres_dehy / R_H2) *
                              ((0.001 * vol_AC / record.T_AC_dehy_Kelvin) +
                               (0.001 * vol_Res / record.T_Res_Kelvin)); //%[g]
            sb.AppendLine($"{nameof(desorped_H2)}: {desorped_H2:0.0000} [g]");
            var capacity_H2_des = 100 * (desorped_H2 / (mass_MgFe + desorped_H2)); //%[%]
            sb.AppendLine($"{nameof(capacity_H2_des)}: {capacity_H2_des:0.0000} [%]");
            yield return (desorped_H2, capacity_H2_des, sb.ToString());
        }
    }



    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}