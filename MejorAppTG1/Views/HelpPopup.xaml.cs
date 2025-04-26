using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;

namespace MejorAppTG1.Views;

public partial class HelpPopup : Popup
{
	#region Variables
	private bool _buttonPressed = false;
    #endregion

    #region Constructores
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="HelpPopup"/>.
    /// </summary>
    public HelpPopup()
	{
		InitializeComponent();
	}
    #endregion

    #region Eventos
    /// <summary>
    /// Maneja el evento de pulsación del botón de Cerrar. Cierra el popup.
    /// </summary>
    /// <param name="sender">El botón pulsado.</param>
    /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
    private void BtnClose_Clicked(object sender, EventArgs e)
    {
        if (_buttonPressed) return;
        _buttonPressed = true;
        try {
            Close();
        } finally {
            _buttonPressed = false;
        }
    }

    /// <summary>
    /// Maneja el evento de abrir/cerrar un Expander.
    /// </summary>
    /// <param name="sender">El Expander que ha cambiado.</param>
    /// <param name="e">La instancia <see cref="EventArgs"/> que contiene los datos del evento.</param>
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