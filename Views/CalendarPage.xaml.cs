namespace EFDocenteMAUI.Views;
using Syncfusion.Maui.Picker;
public partial class CalendarPage : ContentPage
{
	public CalendarPage()
	{
		InitializeComponent();
        SfDatePicker datePicker = new SfDatePicker()
        {
            Mode = PickerMode.Dialog
        };
    }
    private void Button_ClickedIni(object sender, System.EventArgs e)
    {
        this.pickerIni.IsOpen = true;
    }

    private void Button_ClickedFin(object sender, System.EventArgs e)
    {
        this.pickerFin.IsOpen = true;
    }

    private void pickerFin_OkButtonClicked(object sender, EventArgs e)
    {
        this.pickerFin.IsOpen = false;
    }

    private void pickerIni_CancelButtonClicked(object sender, EventArgs e)
    {
        this.pickerIni.IsOpen = false;
    }
}