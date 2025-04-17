using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;

namespace MejorAppTG1.Views;

public partial class AboutPopup : Popup
{
    #region Variables
    private bool _buttonPressed = false;
    #endregion

    #region Constructores
    public AboutPopup()
    {
        InitializeComponent();
    }
    #endregion

    #region Eventos
    private void BtnClose_Clicked(object sender, EventArgs e)
    {
        if (_buttonPressed) return;
        _buttonPressed = true;
        try {
            Close();
        }
        finally {
            _buttonPressed = false;
        }
    }

    private void Expander_ExpandedChanged(object sender, EventArgs e)
    {
        if (sender is Expander expander && expander.Header is Layout headerLayout) {
            var image = headerLayout.GetVisualTreeDescendants().OfType<Image>().FirstOrDefault();

            if (image != null) {
                image.RotateTo(expander.IsExpanded ? 180 : 0, 200, Easing.CubicInOut);
            }
        }
    }

    #endregion
}