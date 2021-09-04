using CourseManager.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace CourseManager.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}