using CommunityToolkit.Maui.Views;
using Syncfusion.Maui.Picker;

namespace EFDocenteMAUI.Views.Popups;

public partial class CalendarPopup : Popup
{
	public CalendarPopup()
	{
		InitializeComponent();
        SfTimePicker timePicker = new SfTimePicker();
	}

    private void Button_ClickedIni(object sender, System.EventArgs e)
    {
        this.timePicker.IsOpen = true;
    }
    private void Button_ClickedFin(object sender, System.EventArgs e)
    {
        this.timePicker.IsOpen = true;
    }

}