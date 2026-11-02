using System.Windows;
using System.Windows.Controls;
using Morimens.WPF.Services;

namespace Morimens.WPF;

public partial class MainWindow : Window
{
    private readonly ApiClient _api = new();

    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        var awakeners = await _api.GetAwakeners();
        AwakenerCombo.ItemsSource       = awakeners;
        AwakenerCombo.DisplayMemberPath = "Name";
        AwakenerCombo.SelectedValuePath = "Id";
    }

    private void AwakenerCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (AwakenerCombo.SelectedItem is AwakenerDto awakener)
        {
            SkillCombo.ItemsSource       = awakener.Skills;
            SkillCombo.DisplayMemberPath = "Name";
            SkillCombo.SelectedValuePath = "Id";
            SkillCombo.SelectedIndex     = 0;
        }
    }

    private async void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        if (AwakenerCombo.SelectedValue is not int awakenerId ||
            SkillCombo.SelectedValue     is not int skillId)
        {
            MessageBox.Show("Please select an Awakener and a Skill.", "Missing Input",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!decimal.TryParse(DefTxt.Text,       out var def))        def        = 0;
        if (!decimal.TryParse(DefFactorTxt.Text,  out var defFactor)) defFactor  = 1.0m;
        if (!decimal.TryParse(CritMultTxt.Text,   out var critMult))  critMult   = 1.5m;
        if (!int    .TryParse(ShieldHpTxt.Text,   out var shieldHp))  shieldHp   = 0;

        var req = new DamageCalculationRequestDto
        {
            AwakenerId         = awakenerId,
            SkillId            = skillId,
            IsCritical         = IsCritChk.IsChecked == true,
            BaseCritMultiplier = critMult,
            Target             = new TargetDto
            {
                Def       = (int)def,
                ShieldHp  = shieldHp > 0 ? shieldHp : null,
                DefFactor = defFactor,
            }
        };

        try
        {
            var result = await _api.Calculate(req);

            DisplayedDmgTxt.Text  = result.DisplayedDamage.ToString("N0");
            ActualDmgTxt.Text     = result.FinalDamage.ToString("N0");
            BreakdownGrid.ItemsSource = result.Breakdown;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"API error: {ex.Message}", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
