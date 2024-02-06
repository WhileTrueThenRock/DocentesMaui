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



}