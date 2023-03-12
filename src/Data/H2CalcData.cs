namespace H2MassPercentPlotter.Data;

public class H2CalcData
{
    public required double R_H2 { get; set; }

    public required double mass_MgFe { get; set; }

    public required double vol_AC { get; set; }

    public required double vol_Res { get; set; }

    public required Record[] Records { get; set; }
}