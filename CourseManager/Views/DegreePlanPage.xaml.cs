using CourseManager.Models;
using CourseManager.ViewModels;
using CourseManager.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CourseManager.Views
{
    public partial class DegreePlanPage : ContentPage
    {
        public DegreePlanPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            var model = BindingContext as DegreePlanViewModel;
            if(model.TermGroups.Count == 0)
            {
                DisplayAlert("Term required", "You must add a term first.", "Ok");
            }
        }

        private void TermsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((CollectionView)sender).SelectedItem = null;
        }
    }
}