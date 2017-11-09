using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TestsFinal.DroidNative
{
    [Activity(Label = "CreateGlobalCalculationActivity")]
    public class CreateGlobalCalculationActivity : Activity
    {

        private EditText _descriptionEditText;
        private EditText _creatorEditText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //SetContentView(Resource.);

            _descriptionEditText = FindViewById<EditText>(Resource.Id.DescriptionEditText);
            Bindings.Add(this.SetBinding(() => ViewModel.Description, () => _descriptionEditText.Text, BindingMode.TwoWay));

            _creatorEditText = FindViewById<EditText>(Resource.Id.CreatorEditText);
            Bindings.Add(this.SetBinding(() => ViewModel.Creator, () => _creatorEditText.Text, BindingMode.TwoWay));

            var saveButton = FindViewById<Button>(Resource.Id.dialogButtonSave);
            saveButton.SetCommand("Click", ViewModel.SaveShoppingListElementCommand);
        }
    }
}