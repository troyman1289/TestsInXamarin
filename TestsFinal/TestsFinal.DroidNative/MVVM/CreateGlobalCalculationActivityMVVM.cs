using System;
using Android.App;
using Android.OS;
using Android.Widget;
using Backend.Model;
using GalaSoft.MvvmLight.Helpers;
using ViewModels;

namespace TestsFinal.DroidNative.MVVM
{
    [Activity(Label = "CreateGlobalCalculationActivityMVVM", Theme = "@style/MainTheme")]
    public class CreateGlobalCalculationActivityMVVM : BaseActivity<CreateGlobalCalculationViewModel>
    {

        private EditText _labelEditText;
        private EditText _startOperandEditText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.CreateGlobalCalculation);

            _labelEditText = FindViewById<EditText>(Resource.Id.LabelEditText);
            Bindings.Add(this.SetBinding(() => ViewModel.GlobalCalculation.Label, () => _labelEditText.Text, BindingMode.TwoWay));

            _startOperandEditText = FindViewById<EditText>(Resource.Id.StartOperandEditText);
            Bindings.Add(this.SetBinding(() => ViewModel.StartOperand, () => _startOperandEditText.Text, BindingMode.TwoWay));

            var saveButton = FindViewById<Button>(Resource.Id.ButtonSave);
            saveButton.SetCommand("Click", ViewModel.SaveCalculationCommand);
        }
    }
}