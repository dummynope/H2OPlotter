namespace H2MassPercentPlotter;

public class UIRecord
{
    public UIRecord(double Pre_Dehydrogenation_PA, double T_AC_dehy_Kelvin, double Post_Dehydrogenation_PA, double T_Res_Kelvin, string? name)
    {
        this.Pre_Dehydrogenation_PA = Pre_Dehydrogenation_PA;
        this.T_AC_dehy_Kelvin = T_AC_dehy_Kelvin;
        this.Post_Dehydrogenation_PA = Post_Dehydrogenation_PA;
        this.T_Res_Kelvin = T_Res_Kelvin;
        this.name = name;
    }

    public UIRecord()
    {

    }
    public double Pre_Dehydrogenation_PA { get; private set; }
    public double Pre_Dehydrogenation
    {
        get => Pre_Dehydrogenation_PA / PhysicalConstants.BarToPA;
        set => Pre_Dehydrogenation_PA = value * PhysicalConstants.BarToPA;
    }

    public double T_AC_dehy_Kelvin { get; private set; } = PhysicalConstants.KelvinOffset;

    /// <summary>
    /// In Celsius
    /// </summary>
    public double T_AC_dehy
    {
        get => T_AC_dehy_Kelvin - PhysicalConstants.KelvinOffset;
        set => T_AC_dehy_Kelvin = value + PhysicalConstants.KelvinOffset;
    }

    public double Post_Dehydrogenation_PA { get; private set; }
    public double Post_Dehydrogenation
    {
        get => Post_Dehydrogenation_PA / PhysicalConstants.BarToPA;
        set => Post_Dehydrogenation_PA = value * PhysicalConstants.BarToPA;
    }


    public double T_Res_Kelvin { get; private set; } = PhysicalConstants.KelvinOffset;

    /// <summary>
    /// In Celsius
    /// </summary>
    public double T_Res
    {
        get => T_Res_Kelvin - PhysicalConstants.KelvinOffset;
        set => T_Res_Kelvin = value + PhysicalConstants.KelvinOffset;
    }

    private string? name;
    public string? Name
    {
        get => name;
        set => name = value;
    }
}