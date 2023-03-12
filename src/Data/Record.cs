namespace H2MassPercentPlotter.Data;

public class Record
{
    public required double Pre_Dehydrogenation_PA { get; set; }
    public required double T_AC_dehy_Kelvin { get; set; } = PhysicalConstants.KelvinOffset;
    public required double Post_Dehydrogenation_PA { get; set; }
    public required double T_Res_Kelvin { get; set; } = PhysicalConstants.KelvinOffset;
    public string? Name { get; set; }

}