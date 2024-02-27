using EFDocenteMAUI.ViewModels;
using Syncfusion.Maui.Accordion;

namespace EFDocenteMAUI.Views;

public partial class UnitsPage : ContentPage
{
    private AccordionItem myAccordionItem;

    public UnitsPage()
    {
		InitializeComponent();
        myAccordionItem = new AccordionItem();
        myAccordionItem.BindingContext = new UnitsViewModel();
    }
}